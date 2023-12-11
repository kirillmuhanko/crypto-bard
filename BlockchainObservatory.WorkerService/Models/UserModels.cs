using System.Text.Json.Serialization;

namespace BlockchainObservatory.WorkerService.Models;

public class UserJsonModel
{
    [JsonPropertyName("chatId")] public long ChatId { get; set; }

    [JsonPropertyName("userId")] public long UserId { get; set; }

    [JsonPropertyName("username")] public string? Username { get; set; }

    [JsonPropertyName("firstName")] public required string FirstName { get; set; }

    [JsonPropertyName("lastName")] public string? LastName { get; set; }

    [JsonPropertyName("createdAt")] public DateTime? CreatedAt { get; set; }
}

public class UserDataJsonModel
{
    [JsonPropertyName("users")] public List<UserJsonModel> Users { get; set; } = new();
}