using BlockchainObservatory.WorkerService.Models;

namespace BlockchainObservatory.WorkerService.Services.Interfaces;

public interface ICryptoPriceFetcherService
{
    Task<List<CryptoTicker24HrResponseModel>> FetchLatestCryptocurrencyDataAsync();
}