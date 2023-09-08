using CryptoBardWorkerService.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBardWorkerService.Handlers;

public interface ICommandHandler
{
    Task HandleCommandAsync(ITelegramBotClient botClient, Message message);
}

public class CommandHandler : ICommandHandler
{
    private readonly IEnumerable<IBotCommand> _commands;

    public CommandHandler(IEnumerable<IBotCommand> commands)
    {
        _commands = commands;
    }

    public async Task HandleCommandAsync(ITelegramBotClient botClient, Message message)
    {
        if (message.Text == null)
            return;

        var commandText = message.Text.ToLowerInvariant();

        foreach (var command in _commands)
        {
            if (commandText != command.Name.ToLowerInvariant())
                continue;

            await command.ExecuteAsync(botClient, message);
            return;
        }
    }
}