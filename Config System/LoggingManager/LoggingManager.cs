using UnityEngine;

namespace SledTunerMod.Utility
{
    public static class LoggingManager
    {
        public static void LogInfo(string message)
        {
            Debug.Log($""[SledTuner:INFO] {message}"");
        }

        public static void LogWarning(string message)
        {
            Debug.LogWarning($""[SledTuner:WARN] {message}"");
        }

        public static void LogError(string message)
        {
            Debug.LogError($""[SledTuner:ERROR] {message}"");
        }
    }
}
