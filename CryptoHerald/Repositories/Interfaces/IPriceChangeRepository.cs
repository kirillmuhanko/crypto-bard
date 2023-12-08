namespace CryptoHerald.Repositories.Interfaces;

public interface IPriceChangeRepository
{
    decimal GetLatestPriceChangePercentage(string cryptocurrencySymbol);
    bool IsPriceSignificantlyChanged(string cryptocurrencySymbol, decimal priceChangePercent);
    void UpdateLatestPriceChangePercentage(string cryptocurrencySymbol, decimal priceChangePercentage);
    void ClearAllPriceChangePercentages();
}