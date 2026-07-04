namespace Microsoft.Azure.Functions.Worker;

using Cirreum;
using Cirreum.Clock;
using Cirreum.Messaging;
using Cirreum.Security;
using Cirreum.Security.Middleware;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

public static class HostingExtensions {

	/// <summary>
	/// Registers and configures the core services including the
	/// <see cref="FunctionContextAccessorMiddleware"/>.
	/// </summary>
	/// <param name="builder">The <see cref="FunctionsApplicationBuilder"/>.</param>
	/// <returns>The <see cref="FunctionsApplicationBuilder"/>.</returns>
	public static IServerlessDomainApplicationBuilder AddCoreServices(
		this IServerlessDomainApplicationBuilder builder) {

		ArgumentNullException.ThrowIfNull(builder);

		builder.FunctionsApplicationBuilder
			.UseMiddleware<FunctionContextAccessorMiddleware>();

		builder.Services
			.AddSingleton<IFunctionContextAccessor, FunctionContextAccessor>()
			.AddScoped<IUserStateAccessor, UserAccessor>()
			.AddTransient<ICsvFileBuilder, CsvFileBuilder>()
			.AddTransient<ICsvFileReader, CsvFileReader>()
			.AddSingleton<IFileSystem, NotImplementedFileSystem>();

		// The distributed transport publisher is generic per message base type since the reset; register the
		// shipped no-op as an open generic so a serverless host without distributed messaging resolves it for
		// any TBase (TryAdd, so a real transport still wins when one is registered).
		builder.Services
			.TryAddSingleton(typeof(IDistributedTransportPublisher<>), typeof(EmptyTransportPublisher<>));

		builder.Services
			.TryAddSingleton(TimeProvider.System);
		builder.Services
			.TryAddSingleton<IDateTimeClock, DateTimeService>();

		return builder;

	}

}