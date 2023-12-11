using System.ComponentModel.DataAnnotations;

namespace BlockchainObservatory.WorkerService.Models;

public class CryptoPriceWatchmanOptions
{
    public const string ConfigurationSectionName = "CryptoPriceWatchman";

    [Required] public string TelegramBotToken { get; set; } = default!;

    [Range(1, 100)] public decimal PriceChangedThresholdPercent { get; set; }

    [Range(1, 100)] public decimal MinimumPriceChangeForNotificationPercent { get; set; }

    [Required] [Url] public string BinanceTicker24HrApiUrl { get; set; } = default!;

    [Required] [MinLength(1)] public List<string> BinanceCryptoSymbols { get; set; } = new();
}