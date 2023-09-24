using CryptoBardWorkerService.Detectors;
using CryptoBardWorkerService.Notifiers;
using CryptoBardWorkerService.Options;
using CryptoBardWorkerService.Repositories;
using CryptoBardWorkerService.Services;
using CryptoBardWorkerService.Validators;
using Microsoft.Extensions.Options;

namespace CryptoBardWorkerService;

public class Worker : BackgroundService
{
    private readonly ICryptocurrencyDataService _cryptocurrencyDataService;
    private readonly IDateChangeDetector _dateChangeDetector;
    private readonly IInternetConnectionValidator _internetConnectionValidator;
    private readonly ILogger<Worker> _logger;
    private readonly IOptions<GlobalOptions> _options;
    private readonly IPriceChangeRepository _priceChangeRepository;
    private readonly ITelegramNotifier _telegramNotifier;
    private readonly IWindowsNotifier _windowsNotifier;

    public Worker(
        ICryptocurrencyDataService cryptocurrencyDataService,
        IDateChangeDetector dateChangeDetector,
        IInternetConnectionValidator internetConnectionValidator,
        ILogger<Worker> logger,
        IOptions<GlobalOptions> options,
        IPriceChangeRepository priceChangeRepository,
        ITelegramNotifier telegramNotifier,
        IWindowsNotifier windowsNotifier)
    {
        _cryptocurrencyDataService = cryptocurrencyDataService;
        _dateChangeDetector = dateChangeDetector;
        _internetConnectionValidator = internetConnectionValidator;
        _logger = logger;
        _options = options;
        _priceChangeRepository = priceChangeRepository;
        _telegramNotifier = telegramNotifier;
        _windowsNotifier = windowsNotifier;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Crypto Bard has awakened! Initiating the cryptocurrency price monitoring...");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (!await _internetConnectionValidator.CheckInternetConnectionAsync())
                    continue;

                await ProcessAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred.");
            }

            _logger.LogInformation("Crypto Bard takes a rest, awaiting the next melody...");
            await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
        }
    }

    private async Task ProcessAsync(CancellationToken cancellationToken)
    {
        if (_dateChangeDetector.HasDateChanged())
        {
            _logger.LogInformation("A new day has begun! Resetting the dictionary...");
            _priceChangeRepository.ClearAll();
        }

        var models = await _cryptocurrencyDataService.GetCryptocurrencyDataAsync();

        foreach (var model in models)
        {
            var lastPriceChangePercentage = _priceChangeRepository.GetLastPriceChangePercentage(model.Symbol);

            if (model.PriceChangePercent >= _options.Value.PriceChangedPercent &&
                model.PriceChangePercent > lastPriceChangePercentage)
            {
                var message = $"{model.Symbol} has risen by {model.PriceChangePercent:F2}% in the last 24 hours!";
                await _telegramNotifier.SendToAllChatIdsAsync(message, cancellationToken);
                _windowsNotifier.Notify(message);
                _priceChangeRepository.UpdateLastPriceChangePercentage(model.Symbol, model.PriceChangePercent);

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