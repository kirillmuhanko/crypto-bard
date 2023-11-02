using CryptoBardWorkerService.Helpers;
using Serilog;
using Serilog.Events;

namespace CryptoBardWorkerService.IoC;

public static class SerilogExtensions
{
    private const string LogFilePath = "logs/log-.txt";
    private const RollingInterval LogRollingInterval = RollingInterval.Day;
    private const int LogRetainedFileCountLimit = 7;
    private const long LogFileSizeLimitBytes = 1024 * 1024 * 10; // 10 MB

    public static ILoggingBuilder AddCustomSerilog(this ILoggingBuilder loggingBuilder)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Information()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.File(
                FilePathHelper.CombineWithBaseDirectory(LogFilePath),
                rollingInterval: LogRollingInterval,
                retainedFileCountLimit: LogRetainedFileCountLimit,
                fileSizeLimitBytes: LogFileSizeLimitBytes)
            .CreateLogger();

        return loggingBuilder.AddSerilog();
    }
}