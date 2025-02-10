using UnityEngine;

namespace SledTunerMod.Utility
{
    public static class ErrorHandler
    {
        public static void HandleException(System.Exception ex, string context)
        {
            Debug.LogError($""[SledTuner:EXCEPTION] {context}: {ex.Message}"");
            // TODO: Additional error handling
        }

        public static void ShowErrorDialog(string message)
        {
            Debug.LogError($""[SledTuner:ERROR] {message}"");
            // TODO: Implement GUI dialog
        }
    }
}
