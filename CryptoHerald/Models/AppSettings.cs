using System.ComponentModel.DataAnnotations;

namespace CryptoHerald.Models;

public class CryptoAnalysisOptions
{
    public const string ConfigurationSectionName = "CryptoAnalysis";

    [Required(ErrorMessage = "TelegramBotToken is required.")]
    public string TelegramBotToken { get; set; } = null!;

    [Required(ErrorMessage = "BinanceTicker24HrApiUrl is required.")]
    [Url(ErrorMessage = "BinanceTicker24HrApiUrl must be a valid URL.")]
    public string BinanceTicker24HrApiUrl { get; set; } = null!;

    [Required(ErrorMessage = "BinanceCryptoSymbols list is required.")]
    [MinLength(1, ErrorMessage = "At least one cryptocurrency symbol must be specified.")]
    public List<string> BinanceCryptoSymbols { get; set; } = null!;

    [Range(1, 100, ErrorMessage = "PriceChangedThresholdPercent must be between 1 and 100.")]
    public decimal PriceChangedThresholdPercent { get; set; }

    [Range(1, 100, ErrorMessage = "MinimumPriceChangeForNotificationPercent must be between 1 and 100.")]
    public decimal MinimumPriceChangeForNotificationPercent { get; set; }
}