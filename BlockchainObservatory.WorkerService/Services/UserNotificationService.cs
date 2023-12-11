using BlockchainObservatory.WorkerService.Repositories.Interfaces;
using BlockchainObservatory.WorkerService.Services.Interfaces;
using Microsoft.Toolkit.Uwp.Notifications;
using Telegram.Bot;

namespace BlockchainObservatory.WorkerService.Services;

public class UserNotificationService : IUserNotificationService
{
    private readonly ILogger<UserNotificationService> _logger;
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly IUserRepository _userRepository;

    public UserNotificationService(
        ILogger<UserNotificationService> logger,
        ITelegramBotClient telegramBotClient,
        IUserRepository userRepository)
    {
        _logger = logger;
        _telegramBotClient = telegramBotClient;
        _userRepository = userRepository;
    }

    public async Task NotifyUsers(string message)
    {
        try
        {
            NotifyUserOnWindows(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying user on Windows");
        }

        try
        {
            await NotifyUsersOnTelegram(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying users on Telegram");
        }
    }

    private static void NotifyUserOnWindows(string message)
    {
        new ToastContentBuilder()
            .AddText(nameof(UserNotificationService))
            .AddText(message)
            .Show();
    }

    private async Task NotifyUsersOnTelegram(string message)
    {
        var userDataModel = await _userRepository.GetUserDataAsync();

        foreach (var userModel in userDataModel.Users)
            await _telegramBotClient.SendTextMessageAsync(userModel.ChatId, message, cancellationToken: default);
    }
}