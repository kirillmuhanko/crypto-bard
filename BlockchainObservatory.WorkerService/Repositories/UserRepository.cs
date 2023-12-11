using System.Text.Json;
using BlockchainObservatory.WorkerService.Models;
using BlockchainObservatory.WorkerService.Repositories.Interfaces;

namespace BlockchainObservatory.WorkerService.Repositories;

public class UserRepository : IUserRepository
{
    private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "users.json");
    private UserDataJsonModel? _cachedUserDataModel;

    public async Task<UserDataJsonModel> GetUserDataAsync()
    {
        return _cachedUserDataModel ??= await LoadUserDataModelFromFileAsync();
    }

    public async Task AddUserAsync(UserJsonModel userJsonModel)
    {
        userJsonModel.CreatedAt ??= DateTime.UtcNow;
        var userDataModel = await GetUserDataAsync();

        if (UserExists(userDataModel, userJsonModel.ChatId))
            return;

        userDataModel.Users.Add(userJsonModel);
        await SaveUserDataToFileAsync(userDataModel);
        _cachedUserDataModel = userDataModel;
    }

    private async Task<UserDataJsonModel> LoadUserDataModelFromFileAsync()
    {
        if (!File.Exists(_filePath))
            return new UserDataJsonModel();

        await using var fileStream = File.OpenRead(_filePath);
        return await JsonSerializer.DeserializeAsync<UserDataJsonModel>(fileStream) ?? new UserDataJsonModel();
    }

    private async Task SaveUserDataToFileAsync(UserDataJsonModel userDataJsonModel)
    {
        await using var fileStream = File.Create(_filePath);
        await JsonSerializer.SerializeAsync(fileStream, userDataJsonModel);
    }

    private static bool UserExists(UserDataJsonModel userDataJsonModel, long chatId)
    {
        return userDataJsonModel.Users.Any(existingUser => existingUser.ChatId == chatId);
    }
}