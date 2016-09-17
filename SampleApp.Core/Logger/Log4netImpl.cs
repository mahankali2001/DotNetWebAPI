using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using SampleApp.Core.Logger;
using log4net;
using System.Diagnostics;

namespace SampleApp.Core.Logger
{
    class Log4netImpl : ILogger
    {
        #region Members

        private ILog logger = null;

        #endregion

        #region Constructors

        public Log4netImpl()
        {
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new FileInfo(GetLoggerConfigurationFile()));
        }

        #endregion

        #region Private methods

        private static string GetLoggerConfigurationFile()
        {
            string loggerConfigDirectory = null;
            loggerConfigDirectory = ConfigurationManager.AppSettings["SampleApp.Core.Logger.Config.Dir"];
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

            loggerConfigurationFile.Append("log4net.config");
            return loggerConfigurationFile.ToString();
        }

        #endregion

        #region Public methods

        public void GetLogger(Type classType)
        {
            logger = log4net.LogManager.GetLogger(classType);
        }

        public void GetLogger(string loggerName)
        {
            logger = log4net.LogManager.GetLogger(loggerName);
        }

        #endregion

        #region ILogger Members

        public void Debug(string message)
        {
            if (logger.IsDebugEnabled)
                logger.Debug(message);
        }

        public void Info(string message)
        {
            if (logger.IsInfoEnabled)
                logger.Info(message);
        }

        public void Warn(string message)
        {
            if (logger.IsWarnEnabled)
                logger.Warn(message);
        }

        public void Error(string message)
        {
            if (logger.IsErrorEnabled)
                logger.Error(message);
        }

        public void Fatal(string message)
        {
            if (logger.IsFatalEnabled)
                logger.Fatal(message);
        }

        public void Debug(string message, Exception exception)
        {
            if (logger.IsDebugEnabled)
                logger.Debug(message, exception);
        }

        public void Info(string message, Exception exception)
        {
            if (logger.IsInfoEnabled)
                logger.Info(message, exception);
        }

        public void Warn(string message, Exception exception)
        {
            if (logger.IsWarnEnabled)
                logger.Warn(message, exception);
        }

        public void Error(string message, Exception exception)
        {
            if (logger.IsErrorEnabled)
                logger.Error(message, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            if (logger.IsFatalEnabled)
                logger.Fatal(message, exception);
        }

        public void Log(Exception ex)
        {
            Exception[] e = GetInnerExceptions(ex);
            foreach (var exception in e)
            {
                Error(exception.Message + " " + exception.StackTrace);
            }
        }

        public void Log(string m, LogType lt)
        {
            string msg = m.ToUpper();
            if (msg.Contains("NOT") || msg.Contains("NO") || msg.Contains("ERROR") || msg.Contains("EXCEPTION") || msg.Contains("PROBLEM")  )
                Console.BackgroundColor = ConsoleColor.Red;
            if (msg.StartsWith("ACTIVATING"))
            {
                Console.BackgroundColor = ConsoleColor.Green;
                Console.ForegroundColor = ConsoleColor.Black;
            }

            if (msg.Contains("START") || msg.Contains("END"))
            {
                Console.BackgroundColor = ConsoleColor.White;
                Console.ForegroundColor =ConsoleColor.Black;
            }

            switch (lt)
            {
                case (LogType.Debug):
                    logger.Debug(m);
                    break;
                case (LogType.Error):
                    logger.Error(m);
                    break;
            }
            Console.ResetColor();
        }

        public static Exception[] GetInnerExceptions(Exception ex)
        {
            List<Exception> exceptions = new List<Exception>();
            exceptions.Add(ex);
            Exception currentEx = ex;
            while (currentEx.InnerException != null)
            {
                currentEx = currentEx.InnerException;
                exceptions.Add(currentEx);
            }
            // Reverse the order to the innermost is first
            exceptions.Reverse();
            return exceptions.ToArray();
        }

        public void LogCallStack(bool logAsError)
        {
            StringBuilder callStackInfo = new StringBuilder();

            callStackInfo.AppendLine().Append("**** Call Stack Info : Start ***** ").AppendLine();
            StackTrace st = new StackTrace(true);
            string fileName;
            for (int i = 0; i < st.FrameCount; i++)
            {

                // Note that high up the call stack, there is only // one stack frame.
                StackFrame sf = st.GetFrame(i);

                // Skip current method
                if (sf.GetMethod().ToString().Contains("LogCallStack("))
                    continue;

                fileName = sf.GetFileName();

                //// Ignore framework calls like - ProcessRequest etc.
                //if (string.IsNullOrEmpty(fileName))
                //    continue;

                // To avoid complet path on local systems...
                if (fileName.IndexOf(@"SampleApp") > 0)
                    fileName =
                        fileName.Substring(fileName.IndexOf(@"SampleApp") +
                                           (@"SampleApp".Length));

                callStackInfo.AppendFormat("File:{0},  Method: {1} Line Number: {2}",
                                           fileName,
                                           sf.GetMethod(),
                                           sf.GetFileLineNumber()
                    ).AppendLine();
            }

            callStackInfo.Append("**** Call Stack Info : END ***** ").AppendLine();

            if(logAsError)
                this.Error(callStackInfo.ToString());
            else
                this.Info(callStackInfo.ToString());

        }

        #endregion

    }
}
