namespace CryptoHerald.Models;

public class CryptoAnalysisOptions
{
    public const string ConfigurationSectionName = "CryptoAnalysis";

    public string BinanceTicker24HrApiUrl { get; set; } = null!;

    public string TelegramBotToken { get; set; } = null!;

    public decimal PriceChangedThresholdPercent { get; set; }

    public decimal MinimumPriceChangeForNotificationPercent { get; set; }

    public List<string> Cryptocurrencies { get; set; } = null!;
}