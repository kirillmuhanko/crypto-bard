namespace BlockchainObservatory.WorkerService.Detectors.Interfaces;

public interface IInternetConnectionDetector
{
    Task<bool> CheckInternetConnectionAsync();
}