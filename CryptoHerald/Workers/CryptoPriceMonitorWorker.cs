using CryptoHerald.Detectors.Interfaces;
using CryptoHerald.Repositories.Interfaces;
using CryptoHerald.Services.Interfaces;

namespace CryptoHerald.Workers;

public class CryptoPriceMonitorWorker : BackgroundService
{
    private readonly ICryptoService _cryptoService;
    private readonly IDateChangeDetector _dateChangeDetector;
    private readonly IInternetConnectionDetector _internetConnectionDetector;
    private readonly ILogger<CryptoPriceMonitorWorker> _logger;
    private readonly INotificationService _notificationService;
    private readonly IPriceChangeRepository _priceChangeRepository;

    public CryptoPriceMonitorWorker(
        ICryptoService cryptoService,
        IDateChangeDetector dateChangeDetector,
        IInternetConnectionDetector internetConnectionDetector,
        ILogger<CryptoPriceMonitorWorker> logger,
        INotificationService notificationService,
        IPriceChangeRepository priceChangeRepository)
    {
        _cryptoService = cryptoService;
        _dateChangeDetector = dateChangeDetector;
        _internetConnectionDetector = internetConnectionDetector;
        _logger = logger;
        _notificationService = notificationService;
        _priceChangeRepository = priceChangeRepository;
        _notificationService = notificationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Crypto Bard has awakened! Initiating the cryptocurrency price monitoring...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await _internetConnectionDetector.CheckInternetConnectionAsync())
                    continue;

                await ProcessAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
            }

            _logger.LogInformation("Crypto Bard takes a rest, awaiting the next melody...");
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }

    private async Task ProcessAsync()
    {
        if (_dateChangeDetector.HasDateChanged())
        {
            _logger.LogInformation("A new day has begun! Resetting the dictionary...");
            _priceChangeRepository.ClearAllPriceChangePercentages();
        }

        var models = await _cryptoService.GetCryptocurrencyDataAsync();

        foreach (var model in models)
        {
            if (_priceChangeRepository.IsPriceSignificantlyChanged(model.Symbol, model.PriceChangePercent))
            {
                var message = $"{model.Symbol} has risen by {model.PriceChangePercent:F2}% in the last 24 hours!";
                await _notificationService.Notify(message);
                _priceChangeRepository.UpdateLatestPriceChangePercentage(model.Symbol, model.PriceChangePercent);

                _logger.LogInformation(
                    $"{model.Symbol} has witnessed a significant increase of {model.PriceChangePercent:F2}% in the past day!");
            }
            else
            {
                _logger.LogInformation(
                    $"{model.Symbol} price change: {model.PriceChangePercent:F2}% - No message sent.");
            }
        }
    }
}