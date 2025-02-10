using System;
using UnityEngine;

namespace SledTunerProject.UtilitySystem
{
    /// <summary>
    /// Catches unhandled exceptions (if possible) and handles critical failures.
    /// Provides user-facing error messages or logs error reports.
    /// </summary>
    public static class ErrorHandler
    {
        /// <summary>
        /// Initializes global exception handling. 
        /// For Unity, you can also hook Application.logMessageReceived to intercept logs.
        /// </summary>
        public static void Initialize()
        {
#if !UNITY_EDITOR
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.logMessageReceived += OnLogMessageReceived;
#endif
        }

        /// <summary>
        /// Called when an unhandled exception occurs in a .NET context (non-Unity thread).
        /// </summary>
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception ex = e.ExceptionObject as Exception;
            string context = "Unhandled Exception";
            if (ex != null)
            {
                HandleException(ex, context);
            }
        }

        /// <summary>
        /// Logs messages from Unity’s internal systems. 
        /// Could filter or handle error messages specifically.
        /// </summary>
        private static void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            if (type == LogType.Exception || type == LogType.Error)
            {
                // Optional: Show an in-game dialog, or record the error.
                // Currently, just log to console.
                Debug.LogError($"[ErrorHandler] Unity Log Error: {condition}\n{stackTrace}");
            }
        }

        /// <summary>
        /// Handles exceptions in a consistent way (logging, UI message, etc.).
        /// </summary>
        public static void HandleException(Exception ex, string context)
        {
            string errorMsg = $"[ErrorHandler] [{context}] Exception: {ex.Message}\nStack: {ex.StackTrace}";
            Debug.LogError(errorMsg);

            // Optionally, show an error dialog or log to file:
            // LoggingManager.LogToFile(errorMsg, "ErrorReport.txt");
        }

        /// <summary>
        /// Displays an error dialog or user-facing message inside the game UI.
        /// (Implementation depends on your mod’s UI approach.)
        /// </summary>
        public static void ShowErrorDialog(string message)
        {
            // For example, if you have a GUIManager with a popup system:
            // GUIManager.ShowMessage(message, MessageType.Error);
            // or just do a Debug.LogError
            Debug.LogError($"[ErrorHandler] ShowErrorDialog: {message}");
        }
    }
}
