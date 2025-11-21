namespace Cirreum;

using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

public interface IServerlessDomainApplicationBuilder : IDomainApplicationBuilder {

	/// <summary>
	/// The current <see cref="FunctionsApplicationBuilder"/> instance.
	/// </summary>
	FunctionsApplicationBuilder FunctionsApplicationBuilder { get; }

	/// <summary>
	/// Gets the set of key/value configuration properties.
	/// </summary>
	/// <remarks>
	/// This can be mutated by adding more configuration sources, which will update its current view.
	/// </remarks>
	IConfigurationManager Configuration { get; }

	/// <summary>
	/// Gets the information about the hosting environment an application is running in.
	/// </summary>
	IHostEnvironment Environment { get; }

}