namespace Cirreum.Security;

using Microsoft.Extensions.DependencyInjection;

sealed class UserAccessor(IFunctionContextAccessor functionContextAccessor) : IUserStateAccessor {

	private const string UserContextKey = "__User_Context_Key";

	private readonly IFunctionContextAccessor _functionContextAccessor =
		functionContextAccessor ?? throw new ArgumentNullException(nameof(functionContextAccessor));

	public ValueTask<IUserState> GetUserState() {

		var context = this._functionContextAccessor.Context;
		if (context == null) {
			// No invocation context (startup, background work) — a fresh anonymous
			// state. User states are mutable, so they are never shared across
			// invocations; each caller gets its own instance.
			return new ValueTask<IUserState>(new ServerlessUser());
		}

		// Check if we already have a UserState for this invocation
		if (context.Items.TryGetValue(UserContextKey, out var existingState)
			&& existingState is ServerlessUser cached) {
			return new ValueTask<IUserState>(cached);
		}

		var user = new ServerlessUser();
		var principal = this._functionContextAccessor.User;
		if (principal?.Identity?.IsAuthenticated == true) {
			user.SetAuthenticatedPrincipal(principal);
		}

		// Stamp the caller's AuthenticationBoundary. The scheme is null — a Functions
		// binding context carries no ASP.NET authentication scheme — and a missing
		// resolver leaves the unresolved default (None) rather than failing the call.
		var resolver = context.InstanceServices?.GetService<IAuthenticationBoundaryResolver>();
		if (resolver is not null) {
			user.SetResolvedAuthenticationBoundary(resolver.Resolve(user, authenticationScheme: null));
		}

		context.Items[UserContextKey] = user;
		return new ValueTask<IUserState>(user);

	}

}
