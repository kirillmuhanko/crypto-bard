using CryptoBardWorkerService.Repositories;
using Telegram.Bot;

namespace CryptoBardWorkerService.Notifiers;

public interface ITelegramNotifier
{
    Task SendToAllChatIdsAsync(string text, CancellationToken cancellationToken = default);
}

public class TelegramNotifier : ITelegramNotifier
{
    private readonly IChatIdRepository _chatIdRepository;
    private readonly ITelegramBotClient _telegramBotClient;

    public TelegramNotifier(
        IChatIdRepository chatIdRepository,
        ITelegramBotClient telegramBotClient)
    {
        _chatIdRepository = chatIdRepository;
        _telegramBotClient = telegramBotClient;
    }

    public async Task SendToAllChatIdsAsync(string message, CancellationToken cancellationToken = default)
    {
        var chatIds = _chatIdRepository.GetAllChatIds();
        var tasks = chatIds.Select(chatId => SendToChatIdAsync(chatId, message, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task SendToChatIdAsync(long chatId, string message, CancellationToken cancellationToken = default)
    {
        await _telegramBotClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }
}