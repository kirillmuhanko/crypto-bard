using System.Text.Json.Serialization;

namespace CryptoBardWorkerService.Models;

public class ChatListModel
{
    [JsonPropertyName("chats")] public List<ChatModel> Chats { get; set; } = new();
}

public class ChatModel
{
    [JsonPropertyName("chatId")] public long ChatId { get; set; }
}