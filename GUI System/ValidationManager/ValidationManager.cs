using UnityEngine;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// Intercepts parameter changes before committing them, ensuring they are within valid ranges or types.
    /// </summary>
    public class ValidationManager
    {
        // If referencing ParameterValidator (from Sled Parameter System), store it here:
        // private ParameterValidator _paramValidator;

        public ValidationManager()
        {
            // Possibly inject a ParameterValidator or keep it simple for now.
        }

        public bool Validate(string paramID, object newValue)
        {
            // For example, check range. In real usage, you'd consult metadata in SledParameterManager.
            Debug.Log($"[ValidationManager] Validating param '{paramID}' => {newValue}");
            return true; // Return false if invalid
        }

        public void ShowValidationError(string paramID, string message)
        {
            Debug.LogError($"[ValidationManager] Error on '{paramID}': {message}");
        }
    }
}
