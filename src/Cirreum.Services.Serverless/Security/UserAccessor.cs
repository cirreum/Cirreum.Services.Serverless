namespace Cirreum.Security;

sealed class UserAccessor(IFunctionContextAccessor functionContextAccessor) : IUserStateAccessor {

	private const string UserContextKey = "__User_Context_Key";
	private static readonly IUserState AnonymousUserInstance = new ServerlessUser();

	private static readonly ValueTask<IUserState> AnonymousUserValueTaskInstance =
		new ValueTask<IUserState>(AnonymousUserInstance);

	private readonly IFunctionContextAccessor _functionContextAccessor =
		functionContextAccessor ?? throw new ArgumentNullException(nameof(functionContextAccessor));

	public ValueTask<IUserState> GetUser() {

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
		context.Items[UserContextKey] = user;
		return new ValueTask<IUserState>(user);

	}

}