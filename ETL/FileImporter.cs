namespace ETL;

public class FileImporter
{
    private readonly LogSaver _logSaver;

    public FileImporter(LogSaver logSaver)
    {
        _logSaver = logSaver;
    }

    public async Task ReadAndWriteAsync(FileSaver fileSaver, FileReader fileReader)
    {
        var inputs = await fileReader.ReadFileAsync();
        var logInfo = fileReader.GetLogs();
        await fileSaver.SaveFileAsync(OutputData.Map(inputs));
        await _logSaver.LogAsync(logInfo);
    }
}