using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Reflection;
using log4net;
using log4net.Util;
using NiteOut.Data.Imdb.Business.Infrastructure;
using NiteOut.Data.Imdb.Business.Common.Enums;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace NiteOut.Data.Imdb.Business
{
    /// <summary>
    /// Responsible for every logging functionality.
    /// </summary>
    public class LogManager : Singleton<LogManager>
    {
        /// <summary>
        /// Interface of the log4net logging.
        /// </summary>
        private readonly ILog logInterface;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogManager"/> class.
        /// </summary>
        public LogManager()
        {
            this.logInterface = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        }

        /// <summary>
        /// Creates an error entry in the log file.
        /// </summary>
        /// <param name="message">Additional message in the log file.</param>
        /// <param name="exc">Occured exception.</param>
        public void Error(string message, Exception exc)
            => this.WriteLog(Severity.Error, exc, message);

        /// <summary>
        /// Creates a warning entry in the log file.
        /// </summary>
        /// <param name="exc">Occured exception.</param>
        public void Warning(Exception exc)
            => this.WriteLog(Severity.Warning, exc);

        /// <summary>
        /// Creates an info entry in the log file.
        /// </summary>
        /// <param name="message">Message of the entry.</param>
        public void Info(string message)
            => this.WriteLog(Severity.Info, null, message);

        /// <summary>
        /// Creates a debug entry in the log file.
        /// </summary>
        /// <param name="exc">Occured exception.</param>
        public void Debug(Exception exc)
            => this.WriteLog(Severity.Debug, exc);

        /// <summary>
        /// Creates a log entry.
        /// </summary>
        /// <param name="severity">Severity of the entry.</param>
        /// <param name="exc">Occured exception.</param>
        /// <param name="message">Optional message.</param>
        private void WriteLog(Severity severity, Exception exc, string message = null)
        {
            try
            {
                LogicalThreadContextProperties properties = LogicalThreadContext.Properties;

                switch (severity)
                {
#if DEBUG
                    case Severity.Debug:
                        this.logInterface.Debug(message, exc);
                        break;
#endif
                    case Severity.Info:
                        this.logInterface.Info(message);
                        break;
                    case Severity.Error:
                        this.logInterface.Error(message, exc);
                        break;
                    case Severity.Warning:
                    default:
                        this.logInterface.Warn(message, exc);
                        break;
                }
            }
            catch (Exception ex)
            {
                try
                {
                    this.logInterface.Error($"Original message: {message}, Failed log exception message: {ex}", exc);
                }
                catch (Exception innerException)
                {
                    try
                    {
                        using (EventLog eventLog = new EventLog("Application"))
                        {
                            eventLog.Source = "HeatMailManager";
                            eventLog.WriteEntry(
                                $"Logging failed. Originial message: {message}. "
                                + $"Original exception: {exc}. Logging exceptions: {ex}; {innerException}",
                                EventLogEntryType.Error);
                        }
                    }
                    catch
                    {
                        // Nothing left to do.
                    }
                }
            }
        }
    }
}
