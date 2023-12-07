using CryptoHerald.Commands.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoHerald.Commands;

public class CommandPing : IBotCommand
{
    public string Name => "/ping";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        await botClient.SendTextMessageAsync(message.Chat.Id, "Pong!");
    }
}