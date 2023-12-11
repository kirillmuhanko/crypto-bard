using BlockchainObservatory.WorkerService.Commands.Interfaces;
using BlockchainObservatory.WorkerService.Models;
using BlockchainObservatory.WorkerService.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BlockchainObservatory.WorkerService.Commands;

public class CommandStart : IBotCommand
{
    private readonly IUserRepository _userRepository;

    public CommandStart(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public string Name => "/start";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var fromUser = message.From ?? new User();

        var userModel = new UserJsonModel
        {
            ChatId = message.Chat.Id,
            UserId = fromUser.Id,
            Username = fromUser.Username,
            FirstName = fromUser.FirstName,
            LastName = fromUser.LastName
        };

        await _userRepository.AddUserAsync(userModel);
        var responseMessage = $"Welcome, {message.From?.FirstName}!";
        await botClient.SendTextMessageAsync(userModel.ChatId, responseMessage);
    }
}