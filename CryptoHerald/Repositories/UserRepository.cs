using System.Text.Json;
using CryptoHerald.Models;
using CryptoHerald.Repositories.Interfaces;

namespace CryptoHerald.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
    private UserDataModel? _cachedUserDataModel;

    public async Task AddUserAsync(UserModel userModel)
    {
        userModel.CreatedAt ??= DateTime.UtcNow;
        var userDataModel = await GetUserDataAsync();

        if (UserExists(userDataModel, userModel.ChatId))
            return;

        userDataModel.Users.Add(userModel);
        await SaveUserDataToFileAsync(userDataModel);
        _cachedUserDataModel = userDataModel;
    }

    public async Task<UserDataModel> GetUserDataAsync()
    {
        return _cachedUserDataModel ??= await LoadUserDataModelFromFileAsync();
    }

    private async Task SaveUserDataToFileAsync(UserDataModel userDataModel)
    {
        await using var fileStream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(fileStream, userDataModel);
    }

    private async Task<UserDataModel> LoadUserDataModelFromFileAsync()
    {
        if (!File.Exists(_filePath))
            return new UserDataModel();

        await using var fileStream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<UserDataModel>(fileStream) ?? new UserDataModel();
    }

    private static bool UserExists(UserDataModel userDataModel, long chatId)
    {
        return userDataModel.Users.Any(existingUser => existingUser.ChatId == chatId);
    }
}