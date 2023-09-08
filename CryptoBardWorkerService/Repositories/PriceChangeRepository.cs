namespace CryptoBardWorkerService.Repositories;

public interface IPriceChangeRepository
{
    decimal GetLastPriceChangePercentage(string symbol);
    void UpdateLastPriceChangePercentage(string symbol, decimal priceChangePercentage);
    void ClearAll();
}

public class PriceChangeRepository : IPriceChangeRepository
{
    private readonly Dictionary<string, decimal> _lastPriceChangePercentages = new();

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
}