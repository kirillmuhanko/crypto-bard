namespace CryptoBardWorkerService.Services.Interfaces;

public interface INotificationService
{
    Task Notify(string message);
}