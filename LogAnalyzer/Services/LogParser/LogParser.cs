using LogAnalyzer.Services.LogReader;

namespace LogAnalyzer.Services.LogParser;

public class LogParser : ILogParser
{
    private readonly ILogReader _logReader;

    public LogParser(ILogReader logReader)
    {
        _logReader = logReader;
    }

    public async IAsyncEnumerable<string> GetIpsAsync(Stream stream)
    {
        var lines = _logReader.ReadLinesAsync(stream);
        var index = 0;

        await foreach (var line in lines)
        {
            if (line.StartsWith("#Fields:"))
            {
                var fieldss = line.Split(' ');
                index = Array.IndexOf(fieldss, "c-ip") - 1;

                if (index < 0)
                    throw new ArgumentOutOfRangeException();
            }

            if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line))
                continue;

            var fields = line.Split(' ');

            yield return fields[index];
        }
    }
}