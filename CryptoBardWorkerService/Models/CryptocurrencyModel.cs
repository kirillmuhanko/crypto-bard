namespace CryptoBardWorkerService.Models;

public class CryptocurrencyModel
{
    public string Symbol { get; set; } = null!;
    public decimal PriceChange { get; set; }
    public decimal PriceChangePercent { get; set; }
    public decimal HighPrice { get; set; }
    public decimal LowPrice { get; set; }
    public decimal Volume { get; set; }
}