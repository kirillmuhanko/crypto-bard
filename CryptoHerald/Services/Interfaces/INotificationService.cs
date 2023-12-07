namespace CryptoHerald.Services.Interfaces;

public interface INotificationService
{
    Task Notify(string message);
}