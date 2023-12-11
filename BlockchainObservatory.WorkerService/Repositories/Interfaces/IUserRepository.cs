using BlockchainObservatory.WorkerService.Models;

namespace BlockchainObservatory.WorkerService.Repositories.Interfaces;

public interface IUserRepository
{
    Task<UserDataJsonModel> GetUserDataAsync();

    Task AddUserAsync(UserJsonModel userJsonModel);
}