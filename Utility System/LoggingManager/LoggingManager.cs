using UnityEngine;

namespace SledTunerProject.UtilitySystem
{
    /// <summary>
    /// Manages log messages with configurable levels.
    /// Routes output to Unity’s console or optionally a log file.
    /// </summary>
    public static class LoggingManager
    {
        public enum LogLevel
        {
            Info,
            Warning,
            Error
        }

        // Optional: If you want to filter logs above/below certain levels:
        private static LogLevel _minLogLevel = LogLevel.Info;

        /// <summary>
        /// Sets the minimum log level for messages to be logged.
        /// </summary>
        public static void SetMinLogLevel(LogLevel level)
        {
            _minLogLevel = level;
        }

        /// <summary>
        /// Logs an info-level message if the current log level permits.
        /// </summary>
        public static void LogInfo(string message)
        {
            if (_minLogLevel <= LogLevel.Info)
            {
                Debug.Log($"[SledTuner] INFO: {message}");
            }
        }

        /// <summary>
        /// Logs a warning-level message if the current log level permits.
        /// </summary>
        public static void LogWarning(string message)
        {
            if (_minLogLevel <= LogLevel.Warning)
            {
                Debug.LogWarning($"[SledTuner] WARNING: {message}");
            }
        }

        /// <summary>
        /// Logs an error-level message if the current log level permits.
        /// </summary>
        public static void LogError(string message)
        {
            if (_minLogLevel <= LogLevel.Error)
            {
                Debug.LogError($"[SledTuner] ERROR: {message}");
            }
        }

        /// <summary>
        /// Optionally, you could add a method to log to a file if desired.
        /// </summary>
        public static void LogToFile(string message, string filePath = "SledTunerLogs.txt")
        {
            // Example: Append log to a file in persistent data path
            // Caution with performance for frequent calls. Typically buffer or batch logs.
            string path = System.IO.Path.Combine(Application.persistentDataPath, filePath);
            System.IO.File.AppendAllText(path, $"{System.DateTime.Now}: {message}\n");
        }
    }
}
