using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBardWorkerService.Commands.Interfaces;

public interface ICommandHandler
{
    Task HandleCommandAsync(ITelegramBotClient botClient, Message message);
}