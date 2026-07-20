namespace Cirreum.Services.Serverless.Tests;

using Cirreum.Security;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

/// <summary>
/// Unit tests for <see cref="UserAccessor"/>: anonymous fallbacks, per-invocation
/// caching, and authentication-boundary stamping during user-state assembly
/// (resolver present → stamped; resolver absent → unresolved default, no throw).
/// </summary>
public class UserAccessorTests {

	[Fact]
	public async Task GetUserState_NoFunctionContext_ReturnsAnonymous() {
		var accessor = new UserAccessor(CreateContextAccessor(context: null, principal: null));

		var state = await accessor.GetUserState();

		state.IsAuthenticated.Should().BeFalse();
	}

	[Fact]
	public async Task GetUserState_NoPrincipal_ReturnsAnonymous() {
		var context = CreateFunctionContext(new ServiceCollection().BuildServiceProvider());
		var accessor = new UserAccessor(CreateContextAccessor(context, principal: null));

		var state = await accessor.GetUserState();

		state.IsAuthenticated.Should().BeFalse();
	}

	[Fact]
	public async Task GetUserState_UnauthenticatedPrincipal_ReturnsAnonymous() {
		var context = CreateFunctionContext(new ServiceCollection().BuildServiceProvider());
		var accessor = new UserAccessor(CreateContextAccessor(context, new ClaimsPrincipal(new ClaimsIdentity())));

		var state = await accessor.GetUserState();

		state.IsAuthenticated.Should().BeFalse();
	}

	[Fact]
	public async Task GetUserState_AuthenticatedWithResolver_StampsBoundary() {
		var provider = new ServiceCollection()
			.AddSingleton<IAuthenticationBoundaryResolver, DefaultAuthenticationBoundaryResolver>()
			.BuildServiceProvider();
		var context = CreateFunctionContext(provider);
		var accessor = new UserAccessor(CreateContextAccessor(context, CreateAuthenticatedPrincipal()));

		var state = await accessor.GetUserState();

		state.IsAuthenticated.Should().BeTrue();
		state.AuthenticationBoundary.Should().Be(AuthenticationBoundary.Global);
		state.IsAuthenticationBoundaryResolved.Should().BeTrue();
	}

	[Fact]
	public async Task GetUserState_AuthenticatedWithoutResolver_LeavesBoundaryUnresolved() {
		var context = CreateFunctionContext(new ServiceCollection().BuildServiceProvider());
		var accessor = new UserAccessor(CreateContextAccessor(context, CreateAuthenticatedPrincipal()));

		var state = await accessor.GetUserState();

		state.IsAuthenticated.Should().BeTrue();
		state.AuthenticationBoundary.Should().Be(AuthenticationBoundary.None);
		state.IsAuthenticationBoundaryResolved.Should().BeFalse();
	}

	[Fact]
	public async Task GetUserState_SecondCallOnSameContext_ReturnsCachedInstance() {
		var provider = new ServiceCollection()
			.AddSingleton<IAuthenticationBoundaryResolver, DefaultAuthenticationBoundaryResolver>()
			.BuildServiceProvider();
		var context = CreateFunctionContext(provider);
		var accessor = new UserAccessor(CreateContextAccessor(context, CreateAuthenticatedPrincipal()));

		var first = await accessor.GetUserState();
		var second = await accessor.GetUserState();

		second.Should().BeSameAs(first);
	}

	private static ClaimsPrincipal CreateAuthenticatedPrincipal() => new(
		new ClaimsIdentity(
			[new Claim("sub", "user-1"), new Claim("name", "Test User")],
			authenticationType: "test"));

	private static FunctionContext CreateFunctionContext(IServiceProvider provider) {
		var context = Substitute.For<FunctionContext>();
		context.Items.Returns(new Dictionary<object, object>());
		context.InstanceServices.Returns(provider);
		return context;
	}

	private static IFunctionContextAccessor CreateContextAccessor(
		FunctionContext? context,
		ClaimsPrincipal? principal) {
		var accessor = Substitute.For<IFunctionContextAccessor>();
		accessor.Context.Returns(context);
		accessor.User.Returns(principal);
		return accessor;
	}

}
