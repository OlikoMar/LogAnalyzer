namespace LogAnalyzer.Services.LogReader;

public interface ILogReader
{
    IAsyncEnumerable<string> ReadLinesAsync(Stream stream);
}