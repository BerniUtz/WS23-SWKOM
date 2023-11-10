using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SWKOM_paperless.Logger;

    public class Log4NetWrapper : ILoggerWrapper
    {
        private log4net.ILog logger;

        public static Log4NetWrapper CreateLogger(string configPath, string name)
        {
            if (!File.Exists(configPath))
            {
                throw new ArgumentException("Does not exist.", nameof(configPath));
            }

            log4net.Config.XmlConfigurator.Configure(new FileInfo(configPath));
            var logger = log4net.LogManager.GetLogger(name);
            return new Log4NetWrapper(logger);
        }

        private Log4NetWrapper(log4net.ILog logger)
        {
            this.logger = logger;
        }

        public void Debug(string message)
        {
            this.logger.Debug(message);
        }
        public void Warn(string message)
        {
            this.logger.Warn(message);
        }

        public void Error(string message)
        {
            this.logger.Error(message);
        }

        public void Fatal(string message)
        {
            this.logger.Fatal(message);
        }
    }
