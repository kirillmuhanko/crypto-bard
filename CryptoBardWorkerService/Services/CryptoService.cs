using CryptoBardWorkerService.Models;
using CryptoBardWorkerService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CryptoBardWorkerService.Services;

public class CryptoService : ICryptoService
{
    private readonly ILogger<CryptoService> _logger;
    private readonly IOptions<CryptoAnalysisOptions> _options;

    public CryptoService(
        ILogger<CryptoService> logger,
        IOptions<CryptoAnalysisOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<List<CryptoTicker24HrModel>> GetCryptocurrencyDataAsync()
    {
        var cryptoDataList = new List<CryptoTicker24HrModel>();
        using var client = new HttpClient();

        foreach (var symbol in _options.Value.Cryptocurrencies)
            try
            {
                var apiUrl = $"{_options.Value.BinanceTicker24HrApiUrl}{symbol}";
                var response = await client.GetAsync(apiUrl);

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JsonConvert.DeserializeObject<CryptoTicker24HrModel>(json);

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