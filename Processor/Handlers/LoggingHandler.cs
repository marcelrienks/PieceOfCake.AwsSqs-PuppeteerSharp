using System;
using static SharedModels.Models.Enums;

namespace Processor.Handlers
{
    static class LoggingHandler
    {
        public static void LogEvent(LoggingLevel level, string message, Exception exception = null)
        {
            var logMessage = string.Empty;

            if (exception != null)
                logMessage = $"{DateTime.Now.ToShortTimeString()} - {level} : {message}. EXCEPTION: {exception.Message}. STACKTRACE: {exception.StackTrace}";

            else
                logMessage = $"{DateTime.Now.ToShortTimeString()} - {level} : {message}";

            switch (level)
            {
                case LoggingLevel.None:
                case LoggingLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;
                case LoggingLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;
                case LoggingLevel.Error:
                case LoggingLevel.Exception:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;
                default:
                    break;
            }

            Console.WriteLine(logMessage);
        }
    }
}
