using CryptoHerald.Models;

namespace CryptoHerald.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserDataModel> GetUserDataAsync();

    Task AddUserAsync(UserModel userModel);
}