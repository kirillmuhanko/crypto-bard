using System.Text.Json;
using CryptoBardWorkerService.Models;
using CryptoBardWorkerService.Repositories.Interfaces;

namespace CryptoBardWorkerService.Repositories;

public class ChatRepository : IChatRepository
{
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "chatData.json");
    private ChatListModel? _cachedChatListModel;

    public async Task SaveChatModelAsync(ChatModel chatModel)
    {
        var chatListModel = await GetChatListModelAsync();
        var existingChat = chatListModel.Chats.Find(c => c.ChatId == chatModel.ChatId);

        if (existingChat != null)
        {
            // Update existing chat if needed
        }
        else
        {
            chatListModel.Chats.Add(chatModel);
        }

        await SaveToFileAsync(chatListModel);
        _cachedChatListModel = chatListModel;
    }

    public async Task<ChatListModel> LoadChatListModelAsync()
    {
        if (_cachedChatListModel != null)
            return _cachedChatListModel;

        if (!File.Exists(_filePath))
            return new ChatListModel();

        await using var fileStream = File.OpenRead(_filePath);
        var chatListModel = await JsonSerializer.DeserializeAsync<ChatListModel>(fileStream);
        _cachedChatListModel = chatListModel;
        return chatListModel ?? new ChatListModel();
    }

    private async Task SaveToFileAsync(ChatListModel chatListModel)
    {
        await using var fileStream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(fileStream, chatListModel);
    }

    private async Task<ChatListModel> GetChatListModelAsync()
    {
        if (_cachedChatListModel != null)
            return _cachedChatListModel;

        return await LoadChatListModelAsync();
    }
}