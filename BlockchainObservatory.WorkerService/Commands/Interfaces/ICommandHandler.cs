using Telegram.Bot;
using Telegram.Bot.Types;

namespace BlockchainObservatory.WorkerService.Commands.Interfaces;

public interface ICommandHandler
{
    Task HandleCommandAsync(ITelegramBotClient botClient, Message message);
}