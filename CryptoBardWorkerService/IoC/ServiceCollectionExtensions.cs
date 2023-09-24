using CryptoBardWorkerService.Commands;
using CryptoBardWorkerService.Detectors;
using CryptoBardWorkerService.Handlers;
using CryptoBardWorkerService.Interfaces;
using CryptoBardWorkerService.Notifiers;
using CryptoBardWorkerService.Options;
using CryptoBardWorkerService.Repositories;
using CryptoBardWorkerService.Services;
using CryptoBardWorkerService.Validators;
using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace CryptoBardWorkerService.IoC;

public static class ServiceCollectionExtensions
{
    public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BinanceOptions>(configuration.GetSection("Binance"));
        services.Configure<TelegramOptions>(configuration.GetSection("Telegram"));

        services.AddSingleton<ITelegramBotClient>(provider =>
        {
            var options = provider.GetRequiredService<IOptions<TelegramOptions>>().Value;
            var botClient = new TelegramBotClient(options.BotToken);
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
        services.AddSingleton<IBotNotifier, BotNotifier>();
        services.AddSingleton<IChatIdRepository, ChatIdRepository>();
        services.AddSingleton<ICommandHandler, CommandHandler>();
        services.AddSingleton<ICryptoService, CryptoService>();
        services.AddSingleton<IDateChangeDetector, DateChangeDetector>();
        services.AddSingleton<IInternetConnectionValidator, InternetConnectionValidator>();
        services.AddSingleton<IPriceChangeRepository, PriceChangeRepository>();
        services.AddSingleton<IWindowsNotifier, WindowsNotifier>();
    }
}