namespace CryptoBardWorkerService.Detectors.Interfaces;

public interface IInternetConnectionDetector
{
    Task<bool> CheckInternetConnectionAsync();
}