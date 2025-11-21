namespace Cirreum.Security;

using System.Security.Claims;

public interface IFunctionContextAccessor {
	FunctionContext? Context { get; set; }
	ClaimsPrincipal? User { get; }
}