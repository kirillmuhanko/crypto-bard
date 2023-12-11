using BlockchainObservatory.WorkerService.Detectors.Interfaces;

namespace BlockchainObservatory.WorkerService.Detectors;

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