using Serilog;

namespace CryptoBardWorkerService.IoC;

public static class SerilogExtensions
{
    public static IHostBuilder UseSerilogWithConfiguration(this IHostBuilder hostBuilder)
    {
        return hostBuilder.UseSerilog((context, configuration) =>
        {
            configuration.ReadFrom.Configuration(context.Configuration);
        });
    }
}