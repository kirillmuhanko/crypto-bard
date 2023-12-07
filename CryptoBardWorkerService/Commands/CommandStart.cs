using CryptoBardWorkerService.Commands.Interfaces;
using CryptoBardWorkerService.Models;
using CryptoBardWorkerService.Repositories.Interfaces;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBardWorkerService.Commands;

public class CommandStart : IBotCommand
{
    private readonly IChatRepository _chatIdRepository;

    public CommandStart(IChatRepository chatIdRepository)
    {
        _chatIdRepository = chatIdRepository;
    }

    public string Name => "/start";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var chatModel = new ChatModel { ChatId = message.Chat.Id }; 
        await _chatIdRepository.SaveChatModelAsync(chatModel);
        var responseMessage = $"Welcome, {message.From?.FirstName}!";
        await botClient.SendTextMessageAsync(chatModel.ChatId, responseMessage);
    }
}