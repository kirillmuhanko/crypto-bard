using CryptoBardWorkerService.Commands;
using CryptoBardWorkerService.Commands.Interfaces;
using CryptoBardWorkerService.Detectors;
using CryptoBardWorkerService.Detectors.Interfaces;
using CryptoBardWorkerService.Models;
using CryptoBardWorkerService.Repositories;
using CryptoBardWorkerService.Repositories.Interfaces;
using CryptoBardWorkerService.Services;
using CryptoBardWorkerService.Services.Interfaces;
using CryptoBardWorkerService.Workers;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;

var host = Host.CreateDefaultBuilder(args)
    .UseSerilog((context, configuration) => { configuration.ReadFrom.Configuration(context.Configuration); })
    .ConfigureAppConfiguration((hostContext, configBuilder) =>
    {
        configBuilder.SetBasePath(hostContext.HostingEnvironment.ContentRootPath)
            .AddJsonFile("appsettings.json", true, true)
            .AddEnvironmentVariables("CryptoBard_");
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddHostedService<Worker>();
        services.Configure<CryptoAnalysisOptions>(hostContext.Configuration.GetSection(CryptoAnalysisOptions.ConfigurationSectionName));

        services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<CryptoAnalysisOptions>>().Value;
            var botClient = new TelegramBotClient(options.TelegramBotToken);
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
        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddSingleton<IDateChangeDetector, DateChangeDetector>();
        services.AddSingleton<IInternetConnectionDetector, InternetConnectionDetector>();
        services.AddSingleton<IPriceChangeRepository, PriceChangeRepository>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddSingleton<IUserRepository, UserRepository>();
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