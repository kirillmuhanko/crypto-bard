using System.Text.Json.Serialization;

namespace CryptoHerald.Models;

public class UserModel
{
    [JsonPropertyName("chatId")] public long ChatId { get; set; }

    [JsonPropertyName("userId")] public long UserId { get; set; }

    [JsonPropertyName("username")] public string? Username { get; set; }

    [JsonPropertyName("firstName")] public required string FirstName { get; set; }

    [JsonPropertyName("lastName")] public string? LastName { get; set; }

    [JsonPropertyName("createdAt")] public DateTime? CreatedAt { get; set; }
}

public class UserDataModel
{
    [JsonPropertyName("users")] public List<UserModel> Users { get; set; } = new();
}