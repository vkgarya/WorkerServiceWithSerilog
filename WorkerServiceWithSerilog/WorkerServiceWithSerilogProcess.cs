using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using WorkerServiceWithSerilog.Logging;

namespace WorkerServiceWithSerilog
{
    public interface IWorkerServiceWithSerilogProcess
    {
        bool ExecuteMyWorkerServiceProcess();
    }
    public class WorkerServiceWithSerilogProcess : IWorkerServiceWithSerilogProcess
    {
        private readonly object _asynLock;

        // Normal Logger
        //private readonly ILogger<WorkerServiceWithSerilogProcess> _logger;

        // Custom Logger
        private readonly ICustomLogger<WorkerServiceWithSerilogProcess> _customLogger;
        public WorkerServiceWithSerilogProcess(IConfiguration configuration

            //, ILogger<WorkerServiceWithSerilogProcess> logger // Normal Logger
            , ICustomLogger<WorkerServiceWithSerilogProcess> customLogger // Custom Logger
            )
        {
            _asynLock = new object();
            // _logger = logger; // Normal Logger
            _customLogger = customLogger; // Custom Logger
        }
        public bool ExecuteMyWorkerServiceProcess()
        {
            bool isSuccessful = false;
            lock (_asynLock)
            {
                try
                {

                    #region Normal Logger
                    //_logger.LogTrace("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), "Application Name", "Normal Logger - LogTrace", "Normal Logger - LogTrace" });
                    //_logger.LogDebug("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), "Application Name", "Normal Logger - LogDebug", "Normal Logger - LogDebug" });
                    //_logger.LogInformation("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), "Application Name", "Normal Logger - LogInformation", "Normal Logger - LogInformation" });
                    //_logger.LogWarning("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), "Application Name", "Normal Logger - LogWarning", "Normal Logger - LogWarning" });
                    //_logger.LogError("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), "Application Name", "Normal Logger - LogError", "Normal Logger - LogError" });
                    //_logger.LogCritical("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), "Application Name", "Normal Logger - LogCritical", "Normal Logger - LogCritical" });
                    #endregion

                    #region Custom Logger
                    _customLogger.LogVerbose("Custom Logger- LogVerbose", "Custom Logger - LogVerbose");
                    _customLogger.LogDebug("Custom Logger - LogDebug", "Custom Logger- LogDebug");
                    _customLogger.LogInformation("Custom Logger - LogInformation", "Custom Logger - LogInformation");
                    _customLogger.LogWarning("Custom Logger - LogWarning", "Custom Logger- LogWarning");

                    Exception exError = new Exception("Custom Logger - LogError");
                    _customLogger.LogError(exError, "Custom Logger- LogError");

                    Exception exFetal = new Exception("Custom Logger - LogError");
                    _customLogger.LogFatal(exFetal, "Custom Logger - LogFatal");
                    #endregion
                    int i = 1; int j = 0; int k = i / j;
                    isSuccessful = true;
                }
                catch (Exception ex)
                {
                    #region Normal Logger
                    //_logger.LogError("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), "Application Name", ex.Message, ex.StackTrace });
                    #endregion

                    #region Custom Logger
                    _customLogger.LogError(ex, "Some string");
                    #endregion

                    isSuccessful = false;
                }
            }
            return isSuccessful;
        }
    }
}
