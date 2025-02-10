using UnityEngine;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// Pushes updated parameter values to actual Unity game objects.
    /// Could be reflection-based or direct references to components.
    /// </summary>
    public class ParameterUpdater
    {
        // Optional: store references, e.g., GameObjectManager, or do reflection logic.

        public ParameterUpdater()
        {
            // e.g., fetch or set up references
        }

        /// <summary>
        /// Called whenever a single parameter changes, if live updates are enabled.
        /// </summary>
        public void UpdateGameObject(string paramID, float newValue)
        {
            // Reflection or direct approach:
            // e.g., find "SnowmobileController" and set power, or call a method.
            // This is a placeholder.
            Debug.Log($"[ParameterUpdater] Updating {paramID} to value {newValue} in the game.");
        }

        /// <summary>
        /// If you need to apply all parameters at once, you can create a separate method here.
        /// </summary>
        public void ApplyAllParametersToGame()
        {
            // Typically called if you want to batch update everything at once.
            Debug.Log("[ParameterUpdater] Applying all parameters in batch mode.");
        }

        public void ToggleLiveUpdates(bool enable)
        {
            // If performance is a concern, you might buffer changes here.
            Debug.Log($"[ParameterUpdater] Live updates toggled: {enable}");
        }
    }
}
