namespace BlockchainObservatory.WorkerService.Services.Interfaces;

public interface IUserNotificationService
{
    Task NotifyUsers(string message);
}