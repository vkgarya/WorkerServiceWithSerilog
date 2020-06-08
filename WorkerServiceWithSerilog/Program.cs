using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using WorkerServiceWithSerilog.Installers;

namespace WorkerServiceWithSerilog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            bool runAsConsole = false;

#if DEBUG
            runAsConsole = true;
#endif
            if (runAsConsole)
            {
                CreateHostBuilderAsConsoleApplication(args).Build().Run();
            }
            else
            {
                CreateHostBuilderAsWindowsService(args).Build().Run();
            }
        }

        public static IHostBuilder CreateHostBuilderAsConsoleApplication(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .ConfigureServices((hostContext, services) =>
               {
                   WorkerServiceWithSerilogInstaller.RegisterWorkerServiceWithSerilog(hostContext, services);
                   LoggerServiceInstaller.RegisterLoggerServices(hostContext, services);
                   services.AddHostedService<Worker>();
               })
               .UseSerilog();

        public static IHostBuilder CreateHostBuilderAsWindowsService(string[] args) =>
           Host.CreateDefaultBuilder(args)
               .UseWindowsService() // Install Nuget - Microsoft.Extensions.Hosting.WindowsServices
               .ConfigureServices((hostContext, services) =>
               {
                   WorkerServiceWithSerilogInstaller.RegisterWorkerServiceWithSerilog(hostContext, services);
                   LoggerServiceInstaller.RegisterLoggerServices(hostContext, services);
                   services.AddHostedService<Worker>();
               })
               .UseSerilog();
    }
}
