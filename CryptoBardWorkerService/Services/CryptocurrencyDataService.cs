using CryptoBardWorkerService.Models;
using CryptoBardWorkerService.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CryptoBardWorkerService.Services;

public interface ICryptocurrencyDataService
{
    Task<List<CryptocurrencyModel>> GetCryptocurrencyDataAsync();
}

public class CryptocurrencyDataService : ICryptocurrencyDataService
{
    private readonly ILogger<CryptocurrencyDataService> _logger;
    private readonly IOptions<GlobalOptions> _options;

    public CryptocurrencyDataService(
        ILogger<CryptocurrencyDataService> logger,
        IOptions<GlobalOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<List<CryptocurrencyModel>> GetCryptocurrencyDataAsync()
    {
        var cryptoDataList = new List<CryptocurrencyModel>();
        using var client = new HttpClient();

        foreach (var symbol in _options.Value.Cryptocurrencies)
            try
            {
                var apiUrl = $"{_options.Value.BinanceTicker24HrApiUrl}{symbol}";
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