using CryptoBardWorkerService;
using CryptoBardWorkerService.Helpers;
using CryptoBardWorkerService.IoC;
using Serilog;
using Serilog.Events;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        configBuilder.SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables("CryptoBard_");
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddAppServices(hostContext.Configuration);
        services.AddHostedService<Worker>();
    })
    .ConfigureLogging((_, loggingBuilder) =>
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.File(
                FilePathHelper.CombineWithBaseDirectory("logs/log-.txt"),
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                fileSizeLimitBytes: 1024 * 1024 * 10) // 10 MB file size limit
            .CreateLogger();

        //loggingBuilder.AddSerilog();
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