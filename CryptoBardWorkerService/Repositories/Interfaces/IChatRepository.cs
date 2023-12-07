using CryptoBardWorkerService.Models;

namespace CryptoBardWorkerService.Repositories.Interfaces;

public interface IChatRepository
{
    Task SaveChatModelAsync(ChatModel chatModel);

    Task<ChatListModel> LoadChatListModelAsync();
}