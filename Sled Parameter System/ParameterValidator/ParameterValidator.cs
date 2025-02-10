using UnityEngine;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// Provides validation (range checks, type checks) for parameter values.
    /// Could also provide error messages or clamping logic.
    /// </summary>
    public class ParameterValidator
    {
        // In a more advanced system, you'd have metadata or references to ParameterStore to check min/max.
        public bool IsValid(string paramID, float candidateValue)
        {
            // For now, just always true.
            // Expand logic using param store if needed.
            return true;
        }

        public string GetValidationError(string paramID, float candidateValue)
        {
            // Return an error message if invalid, otherwise null or empty.
            return "";
        }

        public float Clamp(string paramID, float candidateValue)
        {
            // Example clamp:
            // Suppose we fetch min/max from ParameterStore or metadata if needed.
            // This example is naive (just clamps to -100000..100000).
            float min = -100000f;
            float max = 100000f;
            return Mathf.Clamp(candidateValue, min, max);
        }
    }
}
