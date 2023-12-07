using CryptoBardWorkerService.Models;

namespace CryptoBardWorkerService.Repositories.Interfaces;

public interface IUserRepository
{
    Task AddUserAsync(UserModel userModel);

    Task<UserDataModel> GetUserDataAsync();
}