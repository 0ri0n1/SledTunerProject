using UnityEngine;
using System.Collections.Generic;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// Handles loading/saving of complete parameter presets from a GUI perspective.
    /// Works closely with a ParameterPresetManager (in the Sled Parameter System).
    /// </summary>
    public class GUIPresetManager
    {
        // Example: store known presets
        private List<string> _availablePresets;

        public GUIPresetManager()
        {
            _availablePresets = new List<string> { "Default", "PowerBoost", "Experimental" };
        }

        public List<string> GetAvailablePresets()
        {
            return _availablePresets;
        }

        public void LoadPreset(string presetName)
        {
            Debug.Log($"[GUIPresetManager] Loading preset: {presetName}");
            // e.g., call ParameterPresetManager.LoadPreset(presetName);
        }

        public void SavePreset(string presetName)
        {
            Debug.Log($"[GUIPresetManager] Saving preset: {presetName}");
            // e.g., gather current param values, call ParameterPresetManager.SavePreset(presetName);
        }

        public void DrawPresetUI()
        {
            GUILayout.Label("Available Presets:");
            foreach (var preset in _availablePresets)
            {
                if (GUILayout.Button(preset))
                {
                    LoadPreset(preset);
                }
            }
            if (GUILayout.Button("Save Current As..."))
            {
                SavePreset("NewPreset"); // Example usage
            }
        }
    }
}
