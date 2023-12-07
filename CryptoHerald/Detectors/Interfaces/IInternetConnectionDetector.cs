namespace CryptoHerald.Detectors.Interfaces;

public interface IInternetConnectionDetector
{
    Task<bool> CheckInternetConnectionAsync();
}