using CryptoBardWorkerService;
using CryptoBardWorkerService.IoC;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs/log-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        fileSizeLimitBytes: 1024 * 1024)
    .CreateLogger();

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        configBuilder.SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables("CryptoBard_");
    })
    //.ConfigureLogging((_, loggingBuilder) => { loggingBuilder.AddSerilog(); })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddAppServices(hostContext.Configuration);
        services.AddHostedService<Worker>();
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