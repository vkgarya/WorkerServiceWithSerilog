using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System;
using System.Collections.Generic;
using System.Data;

namespace WorkerServiceWithSerilog.Logging
{
    public interface ICustomLogger<TCategoryName>
    {
        void LogVerbose(string message, string additionalInformation = "");
        void LogDebug(string message, string additionalInformation = "");
        void LogInformation(string message, string additionalInformation = "");
        void LogWarning(string message, string additionalInformation = "");
        void LogError(Exception exception, string additionalInformation = "");
        void LogFatal(Exception exception, string additionalInformation = "");
    }
    public class CustomLogger<T> : ICustomLogger<T>
    {
        private readonly ILogger<T> _logger;
        private readonly IConfiguration _configuration;
        public CustomLogger(ILogger<T> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }
        public void LogVerbose(string message, string additionalInformation = "")
        {
            _logger.LogTrace("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), _configuration.GetValue<string>("ServiceName"), message, additionalInformation });
        }

        public void LogDebug(string message, string additionalInformation = "")
        {
            _logger.LogDebug("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), _configuration.GetValue<string>("ServiceName"), message, additionalInformation });
        }

        public void LogInformation(string message, string additionalInformation = "")
        {
            _logger.LogInformation("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), _configuration.GetValue<string>("ServiceName"), message, additionalInformation });
        }

        public void LogWarning(string message, string additionalInformation = "")
        {
            _logger.LogWarning("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), _configuration.GetValue<string>("ServiceName"), message, additionalInformation });
        }

        public void LogError(Exception exception, string additionalInformation = "")
        {
            _logger.LogError("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), _configuration.GetValue<string>("ServiceName"), exception.Message + Environment.NewLine + exception.StackTrace, additionalInformation });
        }

        public void LogFatal(Exception exception, string additionalInformation = null)
        {
            _logger.LogCritical("{LogId}{Application}{LogMessage}{AdditionalInformation}", new object[] { Guid.NewGuid(), _configuration.GetValue<string>("ServiceName"), exception.Message + Environment.NewLine + exception.StackTrace, additionalInformation });
        }
    }
}
