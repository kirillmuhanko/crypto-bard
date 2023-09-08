using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBardWorkerService.Interfaces;

public interface IBotCommand
{
    string Name { get; }
    Task ExecuteAsync(ITelegramBotClient botClient, Message message);
}