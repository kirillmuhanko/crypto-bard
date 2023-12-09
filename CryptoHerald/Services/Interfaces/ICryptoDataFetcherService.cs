using CryptoHerald.Models;

namespace CryptoHerald.Services.Interfaces;

public interface ICryptoDataFetcherService
{
    Task<List<CryptoTicker24HrResponseModel>> RetrieveCryptocurrencyDataAsync();
}