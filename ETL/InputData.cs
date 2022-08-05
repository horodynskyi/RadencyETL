using System.Collections.Concurrent;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

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
    private readonly string _filePath;
    private int _parsedLines;
    private int _foundErrors;
    public FileReader(string filePath)
    {
        _filePath = filePath;
    }

    public async Task<List<InputData?>> ReadFileAsync()
    {
        var list = new ConcurrentBag<InputData?>();
        var skipHeader = _filePath.EndsWith("csv");
        using (var reader = new StreamReader(_filePath))
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
    
    public (int, int,string) GetLogs() => (_foundErrors, _parsedLines,_filePath);
}

public class MetaLog
{
    private int ParsedLines { get; set; }
    private int ParsedFiles { get; set; }
    private int FoundErrors { get; set; }
    private List<string> FilePaths { get; set; } = new();
    public void Log(string filePath, int errors, int parsedLines)
    {
        FilePaths.Add(filePath);
        FoundErrors += errors;
        ParsedLines += parsedLines;
        ParsedFiles++;
    }

    public override string ToString()
    {
        return $"parsed_files: {ParsedFiles}\nparsed_lines: {ParsedLines}\nfound_errors: {FoundErrors}\ninvalid_files:[{String.Join(",",FilePaths)}]";
    }
}

public class FileSaver
{
    private readonly string _fileName;
    private readonly string _folderName = $@"..\..\..\folder_b\{DateTime.Now.ToString("dd-MM-yyyy")}";
    public FileSaver(string fileName)
    {
        _fileName = fileName;
  
    }

    public async Task SaveFileAsync(IEnumerable<OutputData> data)
    {
        if (!Directory.Exists(_folderName))
            Directory.CreateDirectory(_folderName);
        using (var writer = new StreamWriter($@"{_folderName}\{_fileName}") ?? throw new Exception())
        {
            await writer.WriteLineAsync(JsonConvert.SerializeObject(data));
        }
    }
}


public class DirectoryReader
{
    private  int _filesNumber = 1;
    public async Task ReadAsync(string directoryPath)
    {
        await Task.WhenAll(Directory.GetFiles(directoryPath)
            .Select(async x => await new FileImporter(new FileSaver($"output{_filesNumber++}.json"), new FileReader(x))
                .ReadAndWriteAsync()));
    }
}

public class FileImporter
{
    private readonly FileSaver _fileSaver;
    private readonly FileReader _fileReader;
    private readonly LogSaver _logSaver = new();

    public FileImporter(FileSaver fileSaver, FileReader fileReader)
    {
        _fileSaver = fileSaver;
        _fileReader = fileReader;
    }

    public async Task ReadAndWriteAsync()
    {
        var inputs = await _fileReader.ReadFileAsync();
        (int lines, int errors, string filePath) = _fileReader.GetLogs();
        await _fileSaver.SaveFileAsync(OutputData.Map(inputs));
        await _logSaver.LogAsync($@"..\..\..\folder_b\{DateTime.Now.ToString("dd-MM-yyyy")}", lines,errors,filePath);
    }
}

public class LogSaver
{
    private readonly MetaLog _log = new();
    public async Task LogAsync(string folderPath,int lines, int errors, string filePath)
    {
        _log.Log(filePath,errors,lines);
        using (var writter = new StreamWriter($@"{folderPath}\meta.log",false))
        {
            await writter.WriteLineAsync(_log.ToString());
        }
    }
}

