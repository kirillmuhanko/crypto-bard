using CryptoBardWorkerService.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBardWorkerService.Commands;

public class CommandPing : IBotCommand
{
    public string Name => "/ping";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Pong!");
    }
}