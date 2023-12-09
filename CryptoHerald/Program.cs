using CryptoHerald.Commands;
using CryptoHerald.Commands.Interfaces;
using CryptoHerald.Detectors;
using CryptoHerald.Detectors.Interfaces;
using CryptoHerald.Models;
using CryptoHerald.Repositories;
using CryptoHerald.Repositories.Interfaces;
using CryptoHerald.Services;
using CryptoHerald.Services.Interfaces;
using CryptoHerald.Workers;
using Serilog;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        configBuilder
            .SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddJsonFile($"appsettings.{hostContext.HostingEnvironment.EnvironmentName}.json", true, true)
            .AddEnvironmentVariables("CryptoHerald__");
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<CryptoPriceMonitorWorker>();

        services
            .AddOptions<CryptoAnalysisOptions>()
            .Bind(hostContext.Configuration.GetSection(CryptoAnalysisOptions.ConfigurationSectionName))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var options = hostContext.Configuration
                .GetSection(CryptoAnalysisOptions.ConfigurationSectionName)
                .Get<CryptoAnalysisOptions>();

            var botClient = new TelegramBotClient(options!.TelegramBotToken);
            var commandHandler = provider.GetRequiredService<ICommandHandler>();

            botClient.StartReceiving(
                async (bot, update, _) =>
                {
                    if (update.Message != null)
                        await commandHandler.HandleCommandAsync(bot, update.Message);
                },
                (_, _, _) => Task.CompletedTask);

            return botClient;
        });

        services.AddSingleton<IBotCommand, CommandPing>();
        services.AddSingleton<IBotCommand, CommandStart>();
        services.AddSingleton<ICommandHandler, CommandHandler>();
        services.AddSingleton<ICryptoDataFetcherService, CryptoDataFetcherService>();
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
    logger.LogCritical(ex, "An unhandled exception occurred.");
}