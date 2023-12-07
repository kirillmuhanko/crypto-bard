namespace CryptoBardWorkerService.Models;

public class CryptoTicker24HrModel
{
    public string Symbol { get; set; } = null!;

    public decimal PriceChangePercent { get; set; }
}