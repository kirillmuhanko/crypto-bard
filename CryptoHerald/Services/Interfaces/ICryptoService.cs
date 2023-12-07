using CryptoHerald.Models;

namespace CryptoHerald.Services.Interfaces;

public interface ICryptoService
{
    Task<List<CryptoTicker24HrResponseModel>> GetCryptocurrencyDataAsync();
}