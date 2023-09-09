namespace CryptoBardWorkerService.Helpers;

public static class FilePathHelper
{
    public static string CombineWithBaseDirectory(string fileName)
    {
        var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
        var fullPath = Path.Combine(baseDirectory, fileName);
        return fullPath;
    }
}