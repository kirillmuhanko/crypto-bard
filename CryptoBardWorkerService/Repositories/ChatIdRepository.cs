namespace CryptoBardWorkerService.Repositories;

public interface IChatIdRepository
{
    bool IsChatIdNew(long chatId);
    void SaveChatId(long chatId);
    IEnumerable<long> GetAllChatIds();
}

public class ChatIdRepository : IChatIdRepository
{
    private const string ChatIdFileName = "ChatIds.txt";
    private readonly string _chatIdFilePath;
    private readonly List<long> _chatIds;

    public ChatIdRepository()
    {
        _chatIdFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ChatIdFileName);
        _chatIds = LoadChatIdsFromFile();
    }

    public bool IsChatIdNew(long chatId)
    {
        return !_chatIds.Contains(chatId);
    }

    public void SaveChatId(long chatId)
    {
        if (IsChatIdNew(chatId))
        {
            _chatIds.Add(chatId);
            AppendChatIdToFile(chatId);
        }
    }

    public IEnumerable<long> GetAllChatIds()
    {
        return _chatIds.ToList();
    }

    private List<long> LoadChatIdsFromFile()
    {
        if (File.Exists(_chatIdFilePath))
        {
            var lines = File.ReadAllLines(_chatIdFilePath);
            return lines.Select(long.Parse).ToList();
        }

        return new List<long>();
    }

    private void AppendChatIdToFile(long chatId)
    {
        File.AppendAllText(_chatIdFilePath, $"{chatId}{Environment.NewLine}");
    }
}