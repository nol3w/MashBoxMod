using MelonLoader;
using System.Reflection;

// CREDIT:
// This is a slightly modified version of Lineryders Logger class 
// https://github.com/thejeffreyallen/MapDLLInjector/blob/master/src/Logger.cs

namespace MashBoxMod
{
    /// <summary>
    /// Provides a utility class for logging normal and error messages with color differentiation.
    /// </summary>
    /// <remarks>
    /// This class utilizes the MelonLogger framework to log messages. There are two loggers instantiated:
    /// one for normal messages, displayed in green, one for error messages, displayed in red, and one for warnings displayed in yellow.
    /// Each logger instance is associated with the name of the currently executing assembly.
    /// The class provides static methods to log standard messages and error messages.
    /// </remarks>
    public static class Log
    {
        
        private static MelonLogger.Instance loggerInstance = new MelonLogger.Instance(Assembly.GetExecutingAssembly().GetName().Name, System.ConsoleColor.Green);
        private static MelonLogger.Instance warningLoggerInstance = new MelonLogger.Instance(Assembly.GetExecutingAssembly().GetName().Name, System.ConsoleColor.Yellow);
        private static MelonLogger.Instance errorLoggerInstance = new MelonLogger.Instance(Assembly.GetExecutingAssembly().GetName().Name, System.ConsoleColor.Red);

        public static void Msg(object msg)
        {
            loggerInstance.Msg(msg);
        }

        public static void Warning(object msg)
        {
            warningLoggerInstance.Warning(msg);
        }

        public static void Error(object msg)
        {
            errorLoggerInstance.Msg(msg);
        }

        
    }
}
