using CryptoHerald.Commands.Interfaces;
using CryptoHerald.Models;
using CryptoHerald.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoHerald.Commands;

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

        var userModel = new UserModel
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