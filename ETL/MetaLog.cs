namespace ETL;

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