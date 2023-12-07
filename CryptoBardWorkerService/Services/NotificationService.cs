using CryptoBardWorkerService.Repositories.Interfaces;
using CryptoBardWorkerService.Services.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;
using Telegram.Bot;

namespace CryptoBardWorkerService.Services;

public class NotificationService : INotificationService
{
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<NotificationService> _logger;
    private readonly ITelegramBotClient _telegramBotClient;

    public NotificationService(
        IChatRepository chatRepository,
        ILogger<NotificationService> logger,
        ITelegramBotClient telegramBotClient)
    {
        _chatRepository = chatRepository;
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
        var chatListModel = await _chatRepository.LoadChatListModelAsync();

        foreach (var chatModel in chatListModel.Chats)
            await _telegramBotClient.SendTextMessageAsync(chatModel.ChatId, message, cancellationToken: default);
    }
}