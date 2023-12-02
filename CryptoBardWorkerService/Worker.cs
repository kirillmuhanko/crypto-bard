using CryptoBardWorkerService.Detectors;
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
    private readonly INotificationService _notificationService;
    private readonly IOptions<GlobalOptions> _options;
    private readonly IPriceChangeRepository _priceChangeRepository;

    public Worker(
        ICryptocurrencyDataService cryptocurrencyDataService,
        IDateChangeDetector dateChangeDetector,
        IInternetConnectionValidator internetConnectionValidator,
        ILogger<Worker> logger,
        INotificationService notificationService,
        IOptions<GlobalOptions> options,
        IPriceChangeRepository priceChangeRepository)
    {
        _cryptocurrencyDataService = cryptocurrencyDataService;
        _dateChangeDetector = dateChangeDetector;
        _internetConnectionValidator = internetConnectionValidator;
        _logger = logger;
        _options = options;
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
                if (!await _internetConnectionValidator.CheckInternetConnectionAsync())
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
            _priceChangeRepository.ClearAll();
        }

        var models = await _cryptocurrencyDataService.GetCryptocurrencyDataAsync();

        foreach (var model in models)
        {
            var lastPriceChangePercentage = _priceChangeRepository.GetLastPriceChangePercentage(model.Symbol);
            var percentDifference = model.PriceChangePercent - lastPriceChangePercentage;

            if (model.PriceChangePercent >= _options.Value.PriceChangedPercent && percentDifference > 4m)
            {
                var message = $"{model.Symbol} has risen by {model.PriceChangePercent:F2}% in the last 24 hours!";
                await _notificationService.Notify(message);
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