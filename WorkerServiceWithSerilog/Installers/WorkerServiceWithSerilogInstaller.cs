using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceWithSerilog.Installers
{
    public class WorkerServiceWithSerilogInstaller
    {
        public static void RegisterWorkerServiceWithSerilog(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddTransient<IWorkerServiceWithSerilogProcess, WorkerServiceWithSerilogProcess>();
        }
    }
}
