using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoHerald.Commands.Interfaces;

public interface ICommandHandler
{
    Task HandleCommandAsync(ITelegramBotClient botClient, Message message);
}