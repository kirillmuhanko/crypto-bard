namespace CryptoBardWorkerService.Models;

public class CryptoTicker24HrResponseModel
{
    public string Symbol { get; set; } = null!;

    public decimal PriceChangePercent { get; set; }
}