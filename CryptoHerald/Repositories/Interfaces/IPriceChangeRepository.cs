namespace CryptoHerald.Repositories.Interfaces;

public interface IPriceChangeRepository
{
    decimal GetLastPriceChangePercentage(string symbol);

    void UpdateLastPriceChangePercentage(string symbol, decimal priceChangePercentage);

    void ClearAll();
}