using Telegram.Bot;

namespace CryptoBardWorkerService.Notifiers;

public interface IBotNotifier
{
    Task SendToChatIdsAsync(IEnumerable<long> chatIds, string message, CancellationToken cancellationToken = default);
}

public class BotNotifier : IBotNotifier
{
    private readonly ITelegramBotClient _telegramBotClient;

    public BotNotifier(ITelegramBotClient telegramBotClient)
    {
        _telegramBotClient = telegramBotClient;
    }

    public async Task SendToChatIdsAsync(IEnumerable<long> chatIds, string message, CancellationToken cancellationToken = default)
    {
        var tasks = chatIds.Select(chatId => SendToChatIdAsync(chatId, message, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task SendToChatIdAsync(long chatId, string message, CancellationToken cancellationToken = default)
    {
        await _telegramBotClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }
}