namespace Cirreum.Security.Middleware;

using Microsoft.Azure.Functions.Worker.Middleware;

public class FunctionContextAccessorMiddleware(IFunctionContextAccessor functionContextAccessor)
	: IFunctionsWorkerMiddleware {

	public async Task Invoke(FunctionContext context, FunctionExecutionDelegate next) {

		// Set the context
		functionContextAccessor.Context = context;

		// Proceed with execution
		await next(context);

	}

}