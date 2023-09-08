using System.Net.NetworkInformation;

namespace CryptoBardWorkerService.Validators;

public interface IInternetConnectionValidator
{
    Task<bool> CheckInternetConnectionAsync();
}

public class InternetConnectionValidator : IInternetConnectionValidator
{
    private readonly ILogger<InternetConnectionValidator> _logger;

    public InternetConnectionValidator(ILogger<InternetConnectionValidator> logger)
    {
        _logger = logger;
    }

    public async Task<bool> CheckInternetConnectionAsync()
    {
        var hasInternetConnection = HasInternetConnection();

        if (hasInternetConnection)
        {
            _logger.LogInformation("Internet connection is available.");
        }
        else
        {
            _logger.LogWarning("No internet connection detected. Retrying in 1 minute...");
            await Task.Delay(TimeSpan.FromMinutes(1));
        }

        return hasInternetConnection;
    }

    private static bool HasInternetConnection()
    {
        try
        {
            using var ping = new Ping();
            var result = ping.Send("8.8.8.8", 1000); // Ping Google's DNS server
            return result.Status == IPStatus.Success;
        }
        catch
        {
            return false;
        }
    }
}