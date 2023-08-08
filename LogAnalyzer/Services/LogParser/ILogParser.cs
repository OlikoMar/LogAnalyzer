namespace LogAnalyzer.Services.LogParser;

public interface ILogParser
{
    IAsyncEnumerable<string> GetIpsAsync(Stream stream);
}