namespace CryptoBardWorkerService.Repositories;

public interface IChatIdRepository
{
    bool IsChatIdNew(long chatId);
    void SaveChatId(long chatId);
    IEnumerable<long> GetAllChatIds();
}

public class ChatIdRepository : IChatIdRepository
{
    private const string ChatIdFilePath = "ChatIds.txt";
    private readonly List<long> _chatIds;

    public ChatIdRepository()
    {
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

    private static List<long> LoadChatIdsFromFile()
    {
        if (File.Exists(ChatIdFilePath))
        {
            var lines = File.ReadAllLines(ChatIdFilePath);
            return lines.Select(long.Parse).ToList();
        }

        return new List<long>();
    }

    private static void AppendChatIdToFile(long chatId)
    {
        File.AppendAllText(ChatIdFilePath, $"{chatId}{Environment.NewLine}");
    }
}