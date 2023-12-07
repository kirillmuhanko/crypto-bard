using CryptoHerald.Models;
using CryptoHerald.Repositories.Interfaces;
using Microsoft.Extensions.Options;

namespace CryptoHerald.Repositories;

public class PriceChangeRepository : IPriceChangeRepository
{
    private readonly Dictionary<string, decimal> _lastPriceChangePercentages = new();
    private readonly IOptions<CryptoAnalysisOptions> _options;

    public PriceChangeRepository(IOptions<CryptoAnalysisOptions> options)
    {
        _options = options;
    }

    public decimal GetLastPriceChangePercentage(string symbol)
    {
        _lastPriceChangePercentages.TryGetValue(symbol, out var lastPriceChangePercentage);
        return lastPriceChangePercentage;
    }

    public void UpdateLastPriceChangePercentage(string symbol, decimal priceChangePercentage)
    {
        _lastPriceChangePercentages[symbol] = priceChangePercentage;
    }

    public void ClearAll()
    {
        _lastPriceChangePercentages.Clear();
    }

    public bool IsPriceChanged(string symbol, decimal priceChangePercent)
    {
        var lastPriceChangePercentage = GetLastPriceChangePercentage(symbol);
        var percentDifference = priceChangePercent - lastPriceChangePercentage;

        var result =
            priceChangePercent >= _options.Value.PriceChangedPercent &&
            percentDifference > _options.Value.MinPercentDifferenceForNotification;

        return result;
    }
}