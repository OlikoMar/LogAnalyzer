using System.Net;
using LogAnalyzer.Models;
using LogAnalyzer.Services.LogParser;
using Microsoft.AspNetCore.Mvc;

namespace LogAnalyzer.Controllers;

public class LogAnalysisController : Controller
{
    private readonly ILogParser _logParser;
    public LogAnalysisController(ILogParser logParser)
    {
        _logParser = logParser;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(IFormFile logFile)
    {
        if (logFile.Length == 0)
        {
            ModelState.AddModelError("logFile", "Please select a file to upload.");
            return View();
        }

        await using var stream = logFile.OpenReadStream();
        var ips = _logParser.GetIpsAsync(stream);
        
        var results = await AnalyzeLogDataStreamAsync(ips);
        return View(results);
    }

    private async Task<List<LogAnalysisResult>> AnalyzeLogDataStreamAsync(IAsyncEnumerable<string> reader)
    {
        var hitCountByIP = new Dictionary<string, int>();

        await foreach (var cip in reader)
        {
            if (hitCountByIP.ContainsKey(cip))
            {
                hitCountByIP[cip]++;
            }
            else
            {
                hitCountByIP[cip] = 1;
            }
        }

        var tasks = hitCountByIP.Keys.Select(async cip => new LogAnalysisResult
        {
            ClientIP = cip,
            Hits = hitCountByIP[cip],
            Hostname = await GetHostNameAsync(cip)
        });

        var sortedResults = (await Task.WhenAll(tasks))
            .OrderByDescending(result => result.Hits)
            .ToList();

        return sortedResults;
    }

    private async Task<string> GetHostNameAsync(string ipAddress)
    {
        try
        {
            var hostEntry = await Dns.GetHostEntryAsync(IPAddress.Parse(ipAddress));
            return hostEntry.HostName;
        }
        catch (Exception)
        {
            return "Unknown";
        }
    }
}