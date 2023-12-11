using BlockchainObservatory.WorkerService.Commands;
using BlockchainObservatory.WorkerService.Commands.Interfaces;
using BlockchainObservatory.WorkerService.Detectors;
using BlockchainObservatory.WorkerService.Detectors.Interfaces;
using BlockchainObservatory.WorkerService.Models;
using BlockchainObservatory.WorkerService.Repositories;
using BlockchainObservatory.WorkerService.Repositories.Interfaces;
using BlockchainObservatory.WorkerService.Services;
using BlockchainObservatory.WorkerService.Services.Interfaces;
using BlockchainObservatory.WorkerService.Workers;
using Serilog;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        configBuilder
            .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables("BlockchainObservatory__");
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<CryptoPriceWatchman>();

        services
            .AddOptions<CryptoPriceWatchmanOptions>()
            .Bind(hostContext.Configuration.GetSection(CryptoPriceWatchmanOptions.ConfigurationSectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var cryptoPriceWatchmanOptions = hostContext.Configuration
                .GetSection(CryptoPriceWatchmanOptions.ConfigurationSectionName)
                .Get<CryptoPriceWatchmanOptions>();

            var botClient = new TelegramBotClient(cryptoPriceWatchmanOptions!.TelegramBotToken);
            var commandHandler = provider.GetRequiredService<ICommandHandler>();

            botClient.StartReceiving(
                (bot, update, _) => update.Message != null
                    ? commandHandler.HandleCommandAsync(bot, update.Message)
                    : Task.CompletedTask,
                (_, _, _) => Task.CompletedTask);

            return botClient;
        });

        services.AddSingleton<IBotCommand, CommandPing>();
        services.AddSingleton<IBotCommand, CommandStart>();
        services.AddSingleton<ICommandHandler, CommandHandler>();
        services.AddSingleton<ICryptoPriceFetcherService, CryptoPriceFetcherService>();
        services.AddSingleton<IDateChangeDetector, DateChangeDetector>();
        services.AddSingleton<IInternetConnectionDetector, InternetConnectionDetector>();
        services.AddSingleton<IPriceChangeRepository, PriceChangeRepository>();
        services.AddSingleton<IUserNotificationService, UserNotificationService>();
        services.AddSingleton<IUserRepository, UserRepository>();
    })
    .UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); })
    .UseWindowsService()
    .Build();

try
{
    host.Run();
}
catch (Exception ex)
{
    var logger = host.Services.GetRequiredService<ILogger<Program>>();
    logger.LogCritical(ex, "Unhandled exception occurred in the application.");
}