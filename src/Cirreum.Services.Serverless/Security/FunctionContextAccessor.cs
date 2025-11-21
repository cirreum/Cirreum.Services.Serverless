namespace Cirreum.Security;

using System.Security.Claims;

public class FunctionContextAccessor : IFunctionContextAccessor {

	private static readonly AsyncLocal<FunctionContext?> _currentContext = new();

	public FunctionContext? Context {
		get => _currentContext.Value;
		set => _currentContext.Value = value;
	}

	public ClaimsPrincipal? User =>
		this.Context?.Features.Get<ClaimsPrincipal>();
}