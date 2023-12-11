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
        _logger.LogInformation("CryptoPriceWatchman, with piercing eyes, detects a change in the temporal fabric.");
        if (_dateChangeDetector.HasDateChanged())
        {
            _logger.LogInformation("CryptoPriceWatchman, with meticulous precision, resets the price change records.");
            _priceChangeRepository.ClearAllPriceChangePercentages();
        }

        _logger.LogInformation(
            "CryptoPriceWatchman, with unwavering dedication, plunges into the vast sea of cryptocurrency data.");
        var models = await _cryptoPriceFetcherService.FetchLatestCryptocurrencyDataAsync();

        _logger.LogInformation("CryptoPriceWatchman, with eagle-eyed scrutiny, scans the retrieved data.");
        foreach (var model in models)
        {
            _logger.LogInformation(
                $"CryptoPriceWatchman, with heightened senses, identifies a noteworthy price movement: {model.Symbol} has undergone a remarkable ({model.PriceChangePercent:F2}%) alteration in the past 24 hours.");
            if (_priceChangeRepository.IsPriceSignificantlyChanged(model.Symbol, model.PriceChangePercent))
            {
                _logger.LogInformation(
                    $"CryptoPriceWatchman, with unwavering loyalty, alerts the concerned individuals: {model.Symbol} has surged by {model.PriceChangePercent:F2}%! Stay vigilant, crypto enthusiasts!");
                await _userNotificationService.NotifyUsers(
                    $"{model.Symbol} has risen by {model.PriceChangePercent:F2}% in the last 24 hours!");
                _priceChangeRepository.UpdateLatestPriceChangePercentage(model.Symbol, model.PriceChangePercent);
            }
            else
            {
                _logger.LogInformation(
                    $"CryptoPriceWatchman, with a reassuring tone, informs the community: {model.Symbol}'s price change of {model.PriceChangePercent:F2}% doesn't warrant an immediate attention, yet.");
            }
        }
    }
}