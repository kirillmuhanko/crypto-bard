using CryptoBardWorkerService.Models;

namespace CryptoBardWorkerService.Services.Interfaces;

public interface ICryptoService
{
    Task<List<CryptoTicker24HrResponseModel>> GetCryptocurrencyDataAsync();
}