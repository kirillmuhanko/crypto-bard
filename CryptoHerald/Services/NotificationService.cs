using CryptoHerald.Repositories.Interfaces;
using CryptoHerald.Services.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;
using Telegram.Bot;

namespace CryptoHerald.Services;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IUserRepository _userRepository;

    public NotificationService(
        IUserRepository userRepository,
        ILogger<NotificationService> logger,
        ITelegramBotClient telegramBotClient)
    {
        _userRepository = userRepository;
        _logger = logger;
        _telegramBotClient = telegramBotClient;
    }

    public async Task Notify(string message)
    {
        try
        {
            NotifyUserInWindows(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying user in Windows");
        }

        try
        {
            await NotifyUsersInTelegram(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying users in Telegram");
        }
    }

    private static void NotifyUserInWindows(string message)
    {
        new ToastContentBuilder()
            .AddText(nameof(NotificationService))
            .AddText(message)
            .Show();
    }

    private async Task NotifyUsersInTelegram(string message)
    {
        var userDataModel = await _userRepository.GetUserDataAsync();

        foreach (var chatModel in userDataModel.Users)
            await _telegramBotClient.SendTextMessageAsync(chatModel.ChatId, message, cancellationToken: default);
    }
}