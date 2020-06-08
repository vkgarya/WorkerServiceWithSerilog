using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using WorkerServiceWithSerilog.Logging;

namespace WorkerServiceWithSerilog
{
    public class Worker : BackgroundService
    {
        private IWorkerServiceWithSerilogProcess _workerServiceWithSerilogProcess;
        private readonly ICustomLogger<Worker> _logger;

        public Worker(IWorkerServiceWithSerilogProcess workerServiceWithSerilogProcess, ICustomLogger<Worker> logger)
        {
            _workerServiceWithSerilogProcess = workerServiceWithSerilogProcess;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
               // _logger.LogInformation($"Worker running at: {DateTimeOffset.Now}");
                var result = _workerServiceWithSerilogProcess.ExecuteMyWorkerServiceProcess();
                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
