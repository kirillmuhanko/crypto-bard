using BlockchainObservatory.WorkerService.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BlockchainObservatory.WorkerService.Commands;

public class CommandPing : IBotCommand
{
    public string Name => "/ping";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Pong!");
    }
}