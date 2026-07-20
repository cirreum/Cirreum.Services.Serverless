namespace Cirreum.Security;

using System.Security.Claims;

internal sealed class ServerlessUser : UserStateBase {

	public override bool IsAuthenticationComplete { get; } = true;

	internal void SetAuthenticatedPrincipal(ClaimsPrincipal principal) {
		ArgumentNullException.ThrowIfNull(principal);
		this._principal = principal;

		if (this._principal.Identity is not ClaimsIdentity claimsIdentity) {
			throw new InvalidOperationException($"{nameof(principal)} Identity is null or not a ClaimsIdentity.");
		}
		this._identity = claimsIdentity;

		this._isAuthenticated = this._identity.IsAuthenticated;
		if (!this._isAuthenticated) {
			throw new InvalidOperationException("Cannot initialize from an unauthenticated user.");
		}

		this._profile = new UserProfile(this._principal, TimeZoneInfo.Local.Id);
		if (!this.SessionStartTime.HasValue) {
			this.StartSession();
		}
	}

	internal void SetResolvedAuthenticationBoundary(AuthenticationBoundary boundary) {
		this.SetAuthenticationBoundary(boundary);
	}

}