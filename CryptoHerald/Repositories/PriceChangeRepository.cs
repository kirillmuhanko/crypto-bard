using CryptoHerald.Models;
using CryptoHerald.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace CryptoHerald.Repositories;

public class PriceChangeRepository : IPriceChangeRepository
{
    private readonly IOptions<CryptoAnalysisOptions> _options;
    private readonly Dictionary<string, decimal> _priceChangePercentages = new();

    public PriceChangeRepository(IOptions<CryptoAnalysisOptions> options)
    {
        _options = options;
    }

    public decimal GetLatestPriceChangePercentage(string cryptocurrencySymbol)
    {
        _priceChangePercentages.TryGetValue(cryptocurrencySymbol, out var lastPriceChangePercentage);
        return lastPriceChangePercentage;
    }

    public bool IsPriceSignificantlyChanged(string cryptocurrencySymbol, decimal priceChangePercent)
    {
        var latestPriceChangePercentage = GetLatestPriceChangePercentage(cryptocurrencySymbol);
        var priceChangeDifference = priceChangePercent - latestPriceChangePercentage;
        var priceChangeThresholdMet = priceChangePercent >= _options.Value.PriceChangedThresholdPercent;
        var minimumPriceChangeThresholdMet =
            priceChangeDifference >= _options.Value.MinimumPriceChangeForNotificationPercent;
        return priceChangeThresholdMet && minimumPriceChangeThresholdMet;
    }

    public void UpdateLatestPriceChangePercentage(string cryptocurrencySymbol, decimal priceChangePercentage)
    {
        _priceChangePercentages[cryptocurrencySymbol] = priceChangePercentage;
    }

    public void ClearAllPriceChangePercentages()
    {
        _priceChangePercentages.Clear();
    }
}