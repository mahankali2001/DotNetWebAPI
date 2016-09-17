using System;

namespace SampleApp.Core.Logger
{
    public enum LogType
    {
        Debug = 0,
        Error = 1,
    }

    public interface ILogger
    {
        void Debug(string message);
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Fatal(string message);

        void Debug(string message, Exception exception);
        void Info(string message, Exception exception);
        void Warn(string message, Exception exception);
        void Error(string message, Exception exception);
        void Fatal(string message, Exception exception);

        void Log(Exception ex);
        void Log(string msg, LogType lt);
        void LogCallStack(bool logAsError);

    }
}