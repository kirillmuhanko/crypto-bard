using CryptoHerald.Models;
using CryptoHerald.Services.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace CryptoHerald.Services;

public class CryptoDataFetcherService : ICryptoDataFetcherService
{
    private readonly ILogger<CryptoDataFetcherService> _logger;
    private readonly IOptions<CryptoAnalysisOptions> _options;

    public CryptoDataFetcherService(
        ILogger<CryptoDataFetcherService> logger,
        IOptions<CryptoAnalysisOptions> options)
    {
        _logger = logger;
        _options = options;
    }

    public async Task<List<CryptoTicker24HrResponseModel>> RetrieveCryptocurrencyDataAsync()
    {
        var cryptocurrencyDataList = new List<CryptoTicker24HrResponseModel>();
        using var client = new HttpClient();

        foreach (var symbol in _options.Value.BinanceCryptoSymbols)
        {
            var cryptoTickerData = await FetchCryptoTickerDataAsync(symbol, client);

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

    private async Task<CryptoTicker24HrResponseModel?> FetchCryptoTickerDataAsync(
        string cryptocurrencySymbol,
        HttpClient client)
    {
        var apiUrl = $"{_options.Value.BinanceTicker24HrApiUrl}{cryptocurrencySymbol}";
        var response = await client.GetAsync(apiUrl);

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