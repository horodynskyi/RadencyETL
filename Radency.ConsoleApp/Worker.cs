using ETL;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Radency.ConsoleApp;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IEtl _etl;
    public Worker(ILogger<Worker> logger, IEtl etl)
    {
        _logger = logger;
        _etl = etl;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var cts = new CancellationTokenSource();
            await _etl.ProcessDirectoryAsync(cts);
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(36000, stoppingToken);
        }
    }
}