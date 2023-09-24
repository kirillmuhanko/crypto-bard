using Microsoft.Toolkit.Uwp.Notifications;

namespace CryptoBardWorkerService.Notifiers;

public interface IWindowsNotifier
{
    void Notify(string message);
}

public class WindowsNotifier : IWindowsNotifier
{
    private readonly ILogger<WindowsNotifier> _logger;

    public WindowsNotifier(ILogger<WindowsNotifier> logger)
    {
        _logger = logger;
    }

    public void Notify(string message)
    {
        try
        {
            new ToastContentBuilder()
                .AddText(nameof(WindowsNotifier))
                .AddText(message)
                .Show();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in Notify");
        }
    }
}