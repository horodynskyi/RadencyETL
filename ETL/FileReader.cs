using System.Collections.Concurrent;

namespace ETL;

public class FileReader
{
    private readonly string _filePath;
    private int _parsedLines;
    private int _foundErrors;
    public FileReader(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<List<InputData>> ReadFileAsync()
    {
        var list = new ConcurrentBag<InputData?>();
        var skipHeader = _filePath.EndsWith("csv");
        using (var reader = new StreamReader(_filePath))
        {
            while (!reader.EndOfStream)
            {
                var line = await reader.ReadLineAsync();
                if (skipHeader)
                {
                    skipHeader = false;
                    continue;
                }
                _parsedLines++;
                if (line == null) continue;
                if (InputData.TryParse(line, out var inputData))
                {
                    list.Add(inputData);
                }
                else
                {
                    _foundErrors++;
                }
            }
        }
        return list.ToList();
    }
    public LogInfo GetLogs() => new(_foundErrors, _parsedLines,_filePath);
}