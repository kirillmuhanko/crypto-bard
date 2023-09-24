using CryptoBardWorkerService.Detectors;
using CryptoBardWorkerService.Notifiers;
using CryptoBardWorkerService.Repositories;
using CryptoBardWorkerService.Services;
using CryptoBardWorkerService.Validators;

namespace CryptoBardWorkerService;

public class Worker : BackgroundService
{
    private readonly IBotNotifier _botNotifier;
    private readonly ICryptoService _cryptoService;
    private readonly IDateChangeDetector _dateChangeDetector;
    private readonly IInternetConnectionValidator _internetConnectionValidator;
    private readonly ILogger<Worker> _logger;
    private readonly IPriceChangeRepository _priceChangeRepository;
    private readonly IWindowsNotifier _windowsNotifier;

    public Worker(
        IBotNotifier botNotifier,
        ICryptoService cryptoService,
        IDateChangeDetector dateChangeDetector,
        IInternetConnectionValidator internetConnectionValidator,
        ILogger<Worker> logger,
        IPriceChangeRepository priceChangeRepository, 
        IWindowsNotifier windowsNotifier)
    {
        _botNotifier = botNotifier;
        _cryptoService = cryptoService;
        _dateChangeDetector = dateChangeDetector;
        _internetConnectionValidator = internetConnectionValidator;
        _logger = logger;
        _priceChangeRepository = priceChangeRepository;
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

        var models = await _cryptoService.GetCryptocurrencyDataAsync();

        foreach (var model in models)
        {
            var lastPriceChangePercentage = _priceChangeRepository.GetLastPriceChangePercentage(model.Symbol);

            if (model.PriceChangePercent >= 20 && model.PriceChangePercent > lastPriceChangePercentage)
            {
                var message = $"{model.Symbol} {model.PriceChangePercent:F2}%";
                await _botNotifier.NotifyAsync(message, cancellationToken);
                _windowsNotifier.ShowNotification(message);
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