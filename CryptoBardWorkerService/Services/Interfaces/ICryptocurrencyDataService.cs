using CryptoBardWorkerService.Models;

namespace CryptoBardWorkerService.Services.Interfaces;

public interface ICryptocurrencyDataService
{
    Task<List<CryptocurrencyModel>> GetCryptocurrencyDataAsync();
}