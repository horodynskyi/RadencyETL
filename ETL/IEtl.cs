namespace ETL;

public interface IEtl
{
    Task ProcessDirectoryAsync(CancellationTokenSource cancellationTokenSource);
}

public class Etl : IEtl
{
    private readonly IDirectoryReader _directoryReader;
    public Etl(IDirectoryReader directoryReader)
    {
  
        _directoryReader = directoryReader;
    }

    public async Task ProcessDirectoryAsync(CancellationTokenSource cancellationTokenSource = default!)
    {
        await _directoryReader.ReadAsync(cancellationTokenSource);
    }
}