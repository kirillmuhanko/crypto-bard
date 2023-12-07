using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoHerald.Commands.Interfaces;

public interface IBotCommand
{
    string Name { get; }

    Task ExecuteAsync(ITelegramBotClient botClient, Message message);
}