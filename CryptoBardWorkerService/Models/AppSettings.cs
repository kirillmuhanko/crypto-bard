using System.ComponentModel.DataAnnotations;

namespace CryptoBardWorkerService.Models;

public class CryptoAnalysisOptions
{
    public const string ConfigurationSectionName = "CryptoAnalysis";

    [Required(ErrorMessage = "The BinanceTicker24HrApiUrl is required.")]
    [Url(ErrorMessage = "The BinanceTicker24HrApiUrl must be a valid URL.")]
    public string BinanceTicker24HrApiUrl { get; set; } = null!;

    [Required(ErrorMessage = "The TelegramBotToken is required.")]
    [RegularExpression(@"^\d+:[a-zA-Z0-9_-]+$", ErrorMessage = "The TelegramBotToken format is invalid.")]
    public string TelegramBotToken { get; set; } = null!;

    [Range(1, 100, ErrorMessage = "PriceChangedPercent must be between 1 and 100.")]
    public decimal PriceChangedPercent { get; set; }

    [Range(1, 100, ErrorMessage = "MinPercentDifferenceForNotification must be between 0 and 100.")]
    public decimal MinPercentDifferenceForNotification { get; set; } = 4m;

    [Required(ErrorMessage = "The Cryptocurrencies list is required.")]
    public List<string> Cryptocurrencies { get; set; } = null!;
}