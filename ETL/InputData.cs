using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace ETL;

public class InputData
{
    public string? FirstName { get; set; }
    public string? SecondName { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    public string? Flat { get; set; }
    public decimal Payment { get; set; }
    public DateTime Date { get; set; }
    public long AccountNumber { get; set; }
    public string? ServiceName { get; set; }

    public static bool TryParse(string line, out InputData? inputData)
    {
        var regex = new Regex(@"((\b[^,]+\b)((?<=\.\w).,)?)");
        var matches = regex.Matches(line);
        if (matches.Count == 9)
        {
            inputData = new InputData();
            inputData.FirstName = matches[0].Value;
            inputData.SecondName = matches[1].Value;
            inputData.City = matches[2].Value;
            inputData.Street = matches[3].Value;
            inputData.Flat = matches[4].Value;
            if (Decimal.TryParse(matches[5].Value, NumberStyles.Any,CultureInfo.InvariantCulture, out var payment))
                inputData.Payment = payment;
            else
            {
                inputData = null;
                return false;
            }
            if (DateTime.TryParseExact(matches[6].Value,"yyyy-dd-mm",CultureInfo.InvariantCulture,DateTimeStyles.None,out var date))
                inputData.Date = date;
            else
            {
                inputData = null;
                return false;
            }
            if (Int64.TryParse(matches[7].Value,out var accountNumber))
                inputData.AccountNumber = accountNumber;
            else
            {
                inputData = null;
                return false;
            }
            inputData.ServiceName = matches[8].Value;
        }
        else
        {
            inputData = null;
            return false;
        }
        return true;
    }
}

public class FileReader
{
    private readonly FileSaver _fileSaver;
    private int _parsedLines;
    private int _foundErrors;
    private string _filePath = String.Empty;
    public FileReader()
    {
        _fileSaver = new FileSaver();
    }

    public async Task<List<InputData?>> ReadFileAsync(string filePath)
    {
        var list = new ConcurrentBag<InputData?>();
        var skipHeader = filePath.EndsWith("csv");
        using (var reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                if(skipHeader)
                    continue;
                var line = await reader.ReadLineAsync();
                _parsedLines++;
                if (line != null && InputData.TryParse(line, out var inputData))
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

    public MetaLog GetMetaLog()
    {
        var log = new MetaLog();
        log.FoundErrors += _foundErrors;
        log.ParsedLines += _parsedLines;
        log.FilePaths = _filePath;
        return log;
    }
}

public class MetaLog
{
    
    public int ParsedLines { get; set; }
    public int FoundErrors { get; set; }
    public string FilePaths { get; set; } = String.Empty;
}
public class DirectoryReader
{
    public async Task<List<InputData?>> ReadAsync(string directoryPath)
    {
        var fileImporter = new FileImporter();
        var filesPath = Directory.GetFiles(directoryPath);
        var fileReader = new FileReader();
        var input = (await Task.WhenAll(filesPath.Select(x => fileReader.ReadFileAsync(x))))
            .SelectMany(s => s).ToList();
        return input;
    }

    private async Task<FileImporter> ReadFileWithLog(string filePath)
    {
        var fileReader = new FileReader();
        var fileImporter = new FileImporter();
        fileImporter.InputDatas = await fileReader.ReadFileAsync(filePath);
        fileImporter.MetaLog = fileReader.GetMetaLog();
        return fileImporter;
    }
}

public class FileImporter
{
    public List<InputData?> InputDatas { get; set; } = new();
    public MetaLog MetaLog { get; set; } = new();
}
public class FileSaver
{
    public async Task SaveFileAsync(string filePath,string line)
    {
        using (var writer = new StreamWriter(filePath))
        {
            await writer.WriteLineAsync(line);
        }
    }
}

