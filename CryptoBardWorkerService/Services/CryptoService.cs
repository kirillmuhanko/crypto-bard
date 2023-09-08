using CryptoBardWorkerService.Models;
using CryptoBardWorkerService.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CryptoBardWorkerService.Services;

public interface ICryptoService
{
    Task<List<CryptocurrencyModel>> GetCryptocurrencyDataAsync();
}

public class CryptoService : ICryptoService
{
    private readonly IOptions<BinanceOptions> _binanceOptions;
    private readonly ILogger<CryptoService> _logger;

    public CryptoService(
        IOptions<BinanceOptions> binanceOptions, 
        ILogger<CryptoService> logger)
    {
        _binanceOptions = binanceOptions;
        _logger = logger;
    }

    public async Task<List<CryptocurrencyModel>> GetCryptocurrencyDataAsync()
    {
        var cryptoDataList = new List<CryptocurrencyModel>();
        using var client = new HttpClient();

        foreach (var symbol in _binanceOptions.Value.Cryptocurrencies)
            try
            {
                var apiUrl = _binanceOptions.Value.ApiUrl + symbol;
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<CryptocurrencyModel>(json);

                    if (data != null)
                        cryptoDataList.Add(data);
                    else
                        _logger.LogError($"Failed to deserialize data for {symbol}: Received null data.");
                }
                else
                {
                    _logger.LogError($"Error fetching data for {symbol}: {response.ReasonPhrase}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occurred: {ex.Message}");
            }

        cryptoDataList = cryptoDataList.OrderByDescending(t => t.PriceChangePercent).ToList();
        return cryptoDataList;
    }
}