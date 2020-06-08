using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using WorkerServiceWithSerilog.Logging;

namespace WorkerServiceWithSerilog.Installers
{
    public static class LoggerServiceInstaller
    {
        public static void RegisterLoggerServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            // Dependency Injection - Register Logger Service
            services.AddSingleton(typeof(ICustomLogger<>), typeof(CustomLogger<>)); // Working for one cycle
            //services.AddTransient(typeof(ICustomLogger<>), typeof(CustomLogger<>));  // Works only for one call like LogError
            //services.AddScoped(typeof(ICustomLogger<>), typeof(CustomLogger<>));  // Error

            // Removed default columns and add new columns
            var columnOption = new ColumnOptions();
            columnOption.Store.Remove(StandardColumn.Id);
            columnOption.Store.Remove(StandardColumn.Message);
            columnOption.Store.Remove(StandardColumn.MessageTemplate);
            columnOption.Store.Remove(StandardColumn.Properties);
            columnOption.Store.Remove(StandardColumn.Exception);
            columnOption.Store.Remove(StandardColumn.LogEvent);
            columnOption.AdditionalColumns = new List<SqlColumn>
            {
                new SqlColumn{ColumnName = "LogId", AllowNull = false, DataType= SqlDbType.UniqueIdentifier, NonClusteredIndex = false},
                new SqlColumn{ColumnName = "Application", AllowNull = true, DataType= SqlDbType.NVarChar, DataLength=200},
                new SqlColumn{ColumnName = "LogMessage", AllowNull = true, DataType= SqlDbType.NVarChar },
                new SqlColumn{ColumnName = "AdditionalInformation", AllowNull = true, DataType= SqlDbType.NVarChar }
            };

            // Get Log Level
            var levelSwitch = new LoggingLevelSwitch();
            var logEventLevel = hostContext.Configuration.GetValue<string>("LogEventLevel");
            GetLogLevel(levelSwitch, logEventLevel);

            string env = hostContext.Configuration.GetValue<string>("Environment");
            string serviceName = hostContext.Configuration.GetValue<string>("ServiceName");
            string noReplyEmail = hostContext.Configuration.GetValue<string>("SerilogFromNoReplyEmail");
            string toEmail = hostContext.Configuration.GetValue<string>("SerilogToEmail");
            string fromEmail = string.Empty;
            if (env.Contains("Prod"))
                fromEmail = string.Format("{0}.{1}", serviceName, noReplyEmail);
            else
                fromEmail = string.Format("{0}.{1}.{2}", env, serviceName, noReplyEmail);

            Log.Logger = new LoggerConfiguration()
               .MinimumLevel.ControlledBy(levelSwitch)
               .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
               .Enrich.FromLogContext()

               .WriteTo.Console()

               .WriteTo.File(
                hostContext.Configuration.GetValue<string>("FileLogPath"),
                outputTemplate: "{Timestamp:yyyy-MM-ddTHH:mm:ss.fffZ} [{Level:u3}] {LogMessage:lj}{NewLine}{AdditionalInformation}{NewLine}",
                rollingInterval: RollingInterval.Day
                )

               .WriteTo.MSSqlServer(
                connectionString: hostContext.Configuration.GetValue<string>("SerilogDatabaseConnectionString"),
                tableName: hostContext.Configuration.GetValue<string>("SerilogDBTableName"),
                restrictedToMinimumLevel: levelSwitch.MinimumLevel, //LogEventLevel.Fatal,
                columnOptions: columnOption,
                schemaName: "dbo"
                )

               .WriteTo.Email(
                        outputTemplate: "{Timestamp:yyyy-MM-ddTHH:mm:ss.fffZ} [{Level:u3}] {LogMessage:lj}{NewLine}{AdditionalInformation}{NewLine}Log ID {LogId}",
                        fromEmail: fromEmail,
                        toEmail: toEmail,
                        mailSubject: string.Format("{0} - ACD Provisioning Processor Service error", env),
                        mailServer: hostContext.Configuration.GetValue<string>("SerilogEmailServer"),
                        restrictedToMinimumLevel: LogEventLevel.Error)
               .CreateLogger();
        }

        private static void GetLogLevel(LoggingLevelSwitch levelSwitch, string logEventLevel)
        {
            switch (logEventLevel)
            {
                case "Verbose":
                    levelSwitch.MinimumLevel = LogEventLevel.Verbose;
                    break;
                case "Debug":
                    levelSwitch.MinimumLevel = LogEventLevel.Debug;
                    break;
                case "Information":
                    levelSwitch.MinimumLevel = LogEventLevel.Information;
                    break;
                case "Warning":
                    levelSwitch.MinimumLevel = LogEventLevel.Warning;
                    break;
                case "Error":
                    levelSwitch.MinimumLevel = LogEventLevel.Error;
                    break;
                case "Fatal":
                    levelSwitch.MinimumLevel = LogEventLevel.Fatal;
                    break;
                default:
                    levelSwitch.MinimumLevel = LogEventLevel.Information;
                    break;
            }
        }
    }
}
