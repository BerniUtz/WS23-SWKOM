 
namespace SWKOM_paperless.Logger;

    public static class LoggerFactory
    {
        public static ILoggerWrapper GetLogger(string name)
        {
            return Log4NetWrapper.CreateLogger("./log4net.config", name);
        }
    }
