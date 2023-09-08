using CryptoBardWorkerService;
using CryptoBardWorkerService.IoC;

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
    .UseWindowsService()
    .Build();

host.Run();