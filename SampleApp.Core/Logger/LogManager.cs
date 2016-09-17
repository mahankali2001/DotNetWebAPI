using System;
using SampleApp.Core.Logger;

namespace SampleApp.Core.Logger
{
    public static class LogManager
    {
        private static ILogger logger = new Log4netImpl();

        public static ILogger GetLogger(string loggerName)
        {
            ((Log4netImpl)logger).GetLogger(loggerName);
            return logger;
        }

        public static ILogger GetLogger(Type classType)
        {
            ((Log4netImpl)logger).GetLogger(classType);
            return logger;
        }

    }
}