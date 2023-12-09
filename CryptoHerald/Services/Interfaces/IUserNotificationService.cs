namespace CryptoHerald.Services.Interfaces;

public interface IUserNotificationService
{
    Task NotifyUsers(string message);
}