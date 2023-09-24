using CryptoBardWorkerService.Repositories;
using Telegram.Bot;

namespace CryptoBardWorkerService.Notifiers;

public interface IBotNotifier
{
    Task NotifyAsync(string text, CancellationToken cancellationToken = default);
}

public class BotNotifier : IBotNotifier
{
    private readonly IChatIdRepository _chatIdRepository;
    private readonly ITelegramBotClient _telegramBotClient;

    public BotNotifier(
        IChatIdRepository chatIdRepository, 
        ITelegramBotClient telegramBotClient)
    {
        _chatIdRepository = chatIdRepository;
        _telegramBotClient = telegramBotClient;
    }

    public async Task NotifyAsync(string text, CancellationToken cancellationToken = default)
    {
        var chatIds = _chatIdRepository.GetAllChatIds();
        var tasks = chatIds.Select(chatId => SendToChatIdAsync(chatId, text, cancellationToken));
        await Task.WhenAll(tasks);
    }

    private async Task SendToChatIdAsync(long chatId, string message, CancellationToken cancellationToken = default)
    {
        await _telegramBotClient.SendTextMessageAsync(chatId, message, cancellationToken: cancellationToken);
    }
}