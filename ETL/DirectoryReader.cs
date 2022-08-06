namespace ETL;

public class DirectoryReader: IDirectoryReader
{
    private readonly AppSettings _settings;
    private readonly LogSaver _logSaver;

    public DirectoryReader(AppSettings settings,LogSaver logSaver)
    {
        _settings = settings;
        _logSaver = logSaver;
    }

    public async Task ReadAsync(CancellationTokenSource cancellationTokenSource = default!)
    { 
       var filesNumber = 1;
       var tasks = Directory.GetFiles(_settings.InputFolderPath)
            .Select(async x =>
                await new FileImporter(_logSaver)
                    .ReadAndWriteAsync(new FileSaver($"output{filesNumber++}.json", _settings.OutputFolderPath),
                        new FileReader(x))).ToArray();
      await Task.Run(()=> Task.WaitAny(tasks, cancellationTokenSource.Token));
    }
}

public interface IDirectoryReader
{
    Task ReadAsync(CancellationTokenSource cancellationTokenSource = default!);
}