namespace CryptoBardWorkerService.Options;

public class BinanceOptions
{
    public string ApiUrl { get; set; } = null!;
    public List<string> Cryptocurrencies { get; set; } = null!;
}