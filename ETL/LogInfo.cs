namespace ETL;

public struct LogInfo
{
    public LogInfo(int foundErrors, int parsedLines, string filePath)
    {
        Errors = foundErrors;
        Lines = parsedLines;
        FilePath = filePath;
    }

    public int Errors { get; }

    public int Lines { get; }

    public string FilePath { get; }
}