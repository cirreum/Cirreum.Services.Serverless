namespace Cirreum.Security;

using Microsoft.Extensions.DependencyInjection;

sealed class UserAccessor(IFunctionContextAccessor functionContextAccessor) : IUserStateAccessor {

	private const string UserContextKey = "__User_Context_Key";
	private static readonly IUserState AnonymousUserInstance = new ServerlessUser();

	private static readonly ValueTask<IUserState> AnonymousUserValueTaskInstance =
		new ValueTask<IUserState>(AnonymousUserInstance);

	private readonly IFunctionContextAccessor _functionContextAccessor =
		functionContextAccessor ?? throw new ArgumentNullException(nameof(functionContextAccessor));

	public ValueTask<IUserState> GetUserState() {

		var context = this._functionContextAccessor.Context;
		if (context == null) {
			return AnonymousUserValueTaskInstance;
		}

		// Check if we already have a UserState for this request
		if (context.Items.TryGetValue(UserContextKey, out var existingState)
			&& existingState is ServerlessUser user) {
			return new ValueTask<IUserState>(user);
		}

		var principal = this._functionContextAccessor.User;
		if (principal is null) {
			return AnonymousUserValueTaskInstance;
		}

		if (!principal.Identity?.IsAuthenticated ?? true) {
			context.Items[UserContextKey] = AnonymousUserInstance;
			return AnonymousUserValueTaskInstance;
		}

		user = new ServerlessUser();
		user.SetAuthenticatedPrincipal(principal);

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