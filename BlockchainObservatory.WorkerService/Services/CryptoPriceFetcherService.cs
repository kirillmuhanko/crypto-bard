using BlockchainObservatory.WorkerService.Models;
using BlockchainObservatory.WorkerService.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlockchainObservatory.WorkerService.Services;

public class CryptoPriceFetcherService : ICryptoPriceFetcherService
{
    private static readonly HttpClient HttpClient = new();
    private readonly ILogger<CryptoPriceFetcherService> _logger;
    private readonly IOptions<CryptoPriceWatchmanOptions> _options;

    public CryptoPriceFetcherService(
        ILogger<CryptoPriceFetcherService> logger,
        IOptions<CryptoPriceWatchmanOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<List<CryptoTicker24HrResponseModel>> FetchLatestCryptocurrencyDataAsync()
    {
        var cryptocurrencyDataList = new List<CryptoTicker24HrResponseModel>();

        foreach (var symbol in _options.Value.BinanceCryptoSymbols)
        {
            var cryptoTickerData = await FetchCryptoTickerDataAsync(symbol);

            if (cryptoTickerData != null)
                cryptocurrencyDataList.Add(cryptoTickerData);
            else
                _logger.LogError($"Failed to fetch data for {symbol}.");
        }

        cryptocurrencyDataList = cryptocurrencyDataList
            .OrderByDescending(t => t.PriceChangePercent)
            .ToList();
        return cryptocurrencyDataList;
    }

    private async Task<CryptoTicker24HrResponseModel?> FetchCryptoTickerDataAsync(string cryptocurrencySymbol)
    {
        var apiUrl = $"{_options.Value.BinanceTicker24HrApiUrl}{cryptocurrencySymbol}";
        var response = await HttpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<CryptoTicker24HrResponseModel>(json);
            return data;
        }

        _logger.LogError($"Error fetching data for {cryptocurrencySymbol}: {response.ReasonPhrase}");
        return null;
    }
}