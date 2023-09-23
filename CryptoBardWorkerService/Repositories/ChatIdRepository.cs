using CryptoBardWorkerService.Helpers;

namespace CryptoBardWorkerService.Repositories;

public interface IChatIdRepository
{
    bool IsChatIdNew(long chatId);
    void SaveChatId(long chatId);
    IEnumerable<long> GetAllChatIds();
}

public class ChatIdRepository : IChatIdRepository
{
    private readonly string _chatIdFilePath;
    private readonly IList<long> _chatIds;

    public ChatIdRepository()
    {
        _chatIdFilePath = FilePathHelper.CombineWithBaseDirectory("ChatIds.txt");
        _chatIds = LoadChatIdsFromFile();
    }

    public bool IsChatIdNew(long chatId)
    {
        return !_chatIds.Contains(chatId);
    }

    public void SaveChatId(long chatId)
    {
        if (!IsChatIdNew(chatId))
            return;

        _chatIds.Add(chatId);
        AppendChatIdToFile(chatId);
    }

    public IEnumerable<long> GetAllChatIds()
    {
        return _chatIds;
    }

    private IList<long> LoadChatIdsFromFile()
    {
        if (!File.Exists(_chatIdFilePath))
            return new List<long>();

        var lines = File.ReadAllLines(_chatIdFilePath);
        return lines.Select(long.Parse).ToList();
    }

    private void AppendChatIdToFile(long chatId)
    {
        File.AppendAllText(_chatIdFilePath, $"{chatId}{Environment.NewLine}");
    }
}