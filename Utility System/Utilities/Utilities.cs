using System;
using Newtonsoft.Json.Linq; // Example if you want to parse JSON with Newtonsoft
using UnityEngine;

namespace SledTunerProject.UtilitySystem
{
    /// <summary>
    /// Provides generic utility methods for formatting, clamping, or parsing JSON.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Formats a float with the specified number of decimals.
        /// Example usage: FormatFloat(123.4567f, 2) -> "123.46"
        /// </summary>
        public static string FormatFloat(float value, int decimals)
        {
            return value.ToString($"F{decimals}");
        }

        /// <summary>
        /// Clamps a float value between min and max.
        /// </summary>
        public static float ClampValue(float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Safely parses a JSON string and returns a Newtonsoft.Json.Linq.JToken (or null if invalid).
        /// </summary>
        /// <param name="content">The JSON string to parse.</param>
        public static JToken SafeParseJSON(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            try
            {
                return JToken.Parse(content);
            }
            catch (Exception ex)
            {
                Debug.LogError($"[Utilities] SafeParseJSON: Invalid JSON. {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Example: Converts a raw string to a safe file name (no invalid chars).
        /// </summary>
        public static string MakeSafeFileName(string rawName)
        {
            if (string.IsNullOrEmpty(rawName))
                return "UnknownFile";

            // Remove parentheses
            string safe = rawName.Replace("(", "").Replace(")", "");
            // Replace spaces, colons, slashes, etc. with underscores
            safe = safe.Replace(" ", "_")
                       .Replace(":", "_")
                       .Replace("/", "_")
                       .Replace("\\", "_");

            return safe;
        }
    }
}
