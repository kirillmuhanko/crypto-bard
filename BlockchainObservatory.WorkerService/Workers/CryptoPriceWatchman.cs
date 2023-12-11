using BlockchainObservatory.WorkerService.Detectors.Interfaces;
using BlockchainObservatory.WorkerService.Repositories.Interfaces;
using BlockchainObservatory.WorkerService.Services.Interfaces;

namespace BlockchainObservatory.WorkerService.Workers;

public class CryptoPriceWatchman : BackgroundService
{
    private readonly ICryptoPriceFetcherService _cryptoPriceFetcherService;
    private readonly IDateChangeDetector _dateChangeDetector;
    private readonly IInternetConnectionDetector _internetConnectionDetector;
    private readonly ILogger<CryptoPriceWatchman> _logger;
    private readonly IPriceChangeRepository _priceChangeRepository;
    private readonly IUserNotificationService _userNotificationService;

    public CryptoPriceWatchman(
        ICryptoPriceFetcherService cryptoPriceFetcherService,
        IDateChangeDetector dateChangeDetector,
        IInternetConnectionDetector internetConnectionDetector,
        ILogger<CryptoPriceWatchman> logger,
        IPriceChangeRepository priceChangeRepository,
        IUserNotificationService userNotificationService)
    {
        _cryptoPriceFetcherService = cryptoPriceFetcherService;
        _dateChangeDetector = dateChangeDetector;
        _internetConnectionDetector = internetConnectionDetector;
        _logger = logger;
        _priceChangeRepository = priceChangeRepository;
        _userNotificationService = userNotificationService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            "CryptoPriceWatchman, the vigilant sentinel of the blockchain-observatory, commences its watch.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await _internetConnectionDetector.CheckInternetConnectionAsync())
                {
                    _logger.LogInformation(
                        "CryptoPriceWatchman reports: Connection to the outside world is lost. Resuming monitoring upon reconnection.");

                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                    continue;
                }

                _logger.LogInformation(
                    "CryptoPriceWatchman, with unwavering focus, engages in the task of scrutinizing the crypto realm.");
                await MonitorCryptoMarketAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    $"CryptoPriceWatchman stumbles upon an unexpected obstacle: {ex.Message}. Adapting to the situation.");
            }

            _logger.LogInformation(
                "CryptoPriceWatchman, ever vigilant, concludes its current monitoring cycle. Resuming surveillance shortly.");
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }

        _logger.LogInformation("CryptoPriceWatchman, having completed its duty, bids farewell.");
    }


    private async Task MonitorCryptoMarketAsync()
    {
        if (_dateChangeDetector.HasDateChanged())
        {
            _logger.LogInformation(
                "CryptoPriceWatchman detects a temporal shift! Resetting the scrolls of price change records.");
            _priceChangeRepository.ClearAllPriceChangePercentages();
        }

        var marketData = await _cryptoPriceFetcherService.FetchLatestCryptocurrencyDataAsync();

        foreach (var data in marketData)
        {
            var priceChangeMessage = $"{data.Symbol} has shifted by {data.PriceChangePercent:F2}%";

            if (_priceChangeRepository.IsPriceSignificantlyChanged(data.Symbol, data.PriceChangePercent))
            {
                _logger.LogInformation(
                    $"CryptoPriceWatchman raises the alarm! {priceChangeMessage} - A seismic crypto shift!");
                await _userNotificationService.NotifyUsers($"🚨 Attention! {priceChangeMessage} 🚨");
                _priceChangeRepository.UpdateLatestPriceChangePercentage(data.Symbol, data.PriceChangePercent);
            }
            else
            {
                _logger.LogInformation(
                    $"CryptoPriceWatchman observes serenely: {priceChangeMessage} - All quiet on the crypto front.");
            }
        }
    }
}