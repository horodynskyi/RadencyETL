namespace ETL;

public class LogSaver
{
    private readonly MetaLog _log = new();
    private readonly string _folderPath;
    public LogSaver(string folderPath)
    {
        _folderPath = folderPath;
    }
    public async Task LogAsync(LogInfo logInfo)
    {
        _log.Log(logInfo.FilePath,logInfo.Errors,logInfo.Lines);
        using (var writter = new StreamWriter($@"{_folderPath}\{DateTime.Now:dd-MM-yyyy}\meta.log",false))
        {
            await writter.WriteLineAsync(_log.ToString());
        }
    }
}