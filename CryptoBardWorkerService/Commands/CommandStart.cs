using CryptoBardWorkerService.Interfaces;
using CryptoBardWorkerService.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace CryptoBardWorkerService.Commands;

public class CommandStart : IBotCommand
{
    private readonly IChatIdRepository _chatIdRepository;

    public CommandStart(IChatIdRepository chatIdRepository)
    {
        _chatIdRepository = chatIdRepository;
    }

    public string Name => "/start";

    public async Task ExecuteAsync(ITelegramBotClient botClient, Message message)
    {
        var chatId = message.Chat.Id;

        if (_chatIdRepository.IsChatIdNew(chatId)) 
            _chatIdRepository.SaveChatId(chatId);

        var responseMessage = $"Welcome, {message.From?.FirstName}!";
        await botClient.SendTextMessageAsync(chatId, responseMessage);
    }
}