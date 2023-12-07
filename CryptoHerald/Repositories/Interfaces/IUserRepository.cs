using CryptoHerald.Models;

namespace CryptoHerald.Repositories.Interfaces;

public interface IUserRepository
{
    Task AddUserAsync(UserModel userModel);

    Task<UserDataModel> GetUserDataAsync();
}