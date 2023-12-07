using CryptoBardWorkerService.Detectors.Interfaces;

namespace CryptoBardWorkerService.Detectors;

public class DateChangeDetector : IDateChangeDetector
{
    private DateTime _lastCheckedDate = DateTime.UtcNow.Date;

    public bool HasDateChanged()
    {
        var currentDate = DateTime.UtcNow.Date;

        if (currentDate == _lastCheckedDate)
            return false;

        _lastCheckedDate = currentDate;

        return true;
    }
}