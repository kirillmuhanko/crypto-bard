using CryptoBardWorkerService;
using CryptoBardWorkerService.IoC;
using Serilog;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        configBuilder.SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables("CryptoBard_");
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddCustomServices(hostContext.Configuration);
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging((_, loggingBuilder) =>
    {
        loggingBuilder.AddCustomSerilog();
    })
    .UseWindowsService()
    .Build();

try
{
    host.Run();
}
catch (Exception ex)
{
    Log.Error(ex, "An unhandled exception occurred.");
}
finally
{
    Log.CloseAndFlush();
}