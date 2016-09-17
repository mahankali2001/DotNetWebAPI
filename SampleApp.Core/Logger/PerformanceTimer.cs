using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using log4net;
using log4net.Config;

namespace SampleApp.Core.Logger
{
    public sealed class PerformanceTimer
    {
        //private readonly string performanceTimerName = string.Empty;
        private string message = string.Empty;
        private static readonly Stopwatch stopwatch = new Stopwatch();
        private const string PERFORMANCE_LOG4NET_CONFIG_FILENAME = "performancelog4net.config";
        private ILog logger = log4net.LogManager.GetLogger(typeof(PerformanceTimer));

        private static string GetLoggerConfigurationFile()
        {
            string loggerConfigDirectory = null;
            loggerConfigDirectory = ConfigurationManager.AppSettings["SampleApp.Core.Logger.PerformanceTimer.Config.Dir"];
            if (string.IsNullOrEmpty(loggerConfigDirectory))
            {
                // Default to the executing assembly location.
                loggerConfigDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            }

            StringBuilder loggerConfigurationFile = new StringBuilder(loggerConfigDirectory);
            if (loggerConfigDirectory.EndsWith("\\") == false)
            {
                loggerConfigurationFile.Append("\\");
            }

            loggerConfigurationFile.Append(PERFORMANCE_LOG4NET_CONFIG_FILENAME);
            return loggerConfigurationFile.ToString();
        }

        private void ConfigureLog4net()
        {
            XmlConfigurator.ConfigureAndWatch(new FileInfo(GetLoggerConfigurationFile()));
        }

        //public PerformanceTimer(string performanceTimerName)
        public PerformanceTimer()
        {
            //this.performanceTimerName = performanceTimerName;
            ConfigureLog4net();
        }

        public void Start(string message)
        {
            if (stopwatch.IsRunning)
                stopwatch.Reset();

            this.message = message;
            stopwatch.Start();
            //string logMessage = string.Format("Start timer: [{0}]: {1}", performanceTimerName, message);
            string logMessage = string.Format("Start timer: {0}", message);
            if (logger.IsInfoEnabled)
                logger.Info(logMessage);
        }

        public void Stop()
        {
            stopwatch.Stop();
            // Calculate the time elasped from the time the timer was started.
            //string logMessage = string.Format("Stop timer: [{0}]: [{1} milliseconds]: {2}", performanceTimerName, stopwatch.Elapsed.Milliseconds, message);
            string logMessage = string.Format("Stop timer: [{0} milliseconds]: {1}", stopwatch.Elapsed.Milliseconds, message);
            if (logger.IsInfoEnabled)
                logger.Info(logMessage);
        }

        public void Reset()
        {
            stopwatch.Reset();
            //string logMessage = string.Format("Reset timer: [{0}]: {1}", performanceTimerName, message);
            string logMessage = string.Format("Reset timer: {0}", message);
            if (logger.IsInfoEnabled)
                logger.Info(logMessage);
        }

    }
}