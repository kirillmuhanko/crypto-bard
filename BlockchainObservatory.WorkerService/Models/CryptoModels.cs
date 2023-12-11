namespace BlockchainObservatory.WorkerService.Models;

public class CryptoTicker24HrResponseModel
{
    public string Symbol { get; set; } = default!;

    public decimal PriceChangePercent { get; set; }
}