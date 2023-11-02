using CryptoBardWorkerService.Repositories;
using Microsoft.Toolkit.Uwp.Notifications;
using Telegram.Bot;

namespace CryptoBardWorkerService.Services;

public interface INotificationService
{
    Task Notify(string message);
}

public class NotificationService : INotificationService
{
    private readonly IChatIdRepository _chatIdRepository;
    private readonly ILogger<NotificationService> _logger;
    private readonly ITelegramBotClient _telegramBotClient;

    public NotificationService(
        IChatIdRepository chatIdRepository,
        ILogger<NotificationService> logger,
        ITelegramBotClient telegramBotClient)
    {
        _chatIdRepository = chatIdRepository;
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
        var chatIds = _chatIdRepository.GetAllChatIds();

        foreach (var chatId in chatIds)
            await _telegramBotClient.SendTextMessageAsync(chatId, message, cancellationToken: default);
    }
}