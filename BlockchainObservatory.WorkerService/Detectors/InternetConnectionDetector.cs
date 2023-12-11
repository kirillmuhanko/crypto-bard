using System.Net.NetworkInformation;
using BlockchainObservatory.WorkerService.Detectors.Interfaces;

namespace BlockchainObservatory.WorkerService.Detectors;

public class InternetConnectionDetector : IInternetConnectionDetector
{
    private const string GoogleDns = "8.8.8.8";

    public async Task<bool> CheckInternetConnectionAsync()
    {
        using var ping = new Ping();
        var pingReply = await ping.SendPingAsync(GoogleDns);
        var hasInternetConnection = pingReply.Status == IPStatus.Success;
        return hasInternetConnection;
    }
}