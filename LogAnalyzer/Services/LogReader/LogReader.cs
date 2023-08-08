namespace LogAnalyzer.Services.LogReader;

public class LogReader : ILogReader
{
    public async IAsyncEnumerable<string> ReadLinesAsync(Stream stream)
    {
        
        using var streamReader = new StreamReader(stream);
        while (!streamReader.EndOfStream)
        {
             yield return await streamReader.ReadLineAsync();
        }
    }
}