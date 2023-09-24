using Microsoft.Toolkit.Uwp.Notifications;

namespace CryptoBardWorkerService.Notifiers;

public interface IWindowsNotifier
{
    void ShowNotification(string text);
}

public class WindowsNotifier : IWindowsNotifier
{
    private readonly ILogger<WindowsNotifier> _logger;

    public WindowsNotifier(ILogger<WindowsNotifier> logger)
    {
        _logger = logger;
    }

    public void ShowNotification(string text)
    {
        try
        {
            new ToastContentBuilder()
                .AddText(nameof(WindowsNotifier))
                .AddText(text)
                .Show();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in ShowNotification");
        }
    }
}