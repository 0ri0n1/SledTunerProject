using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// Responsible for loading and saving preset files (distinct from the GUI side).
    /// </summary>
    public class ParameterPresetManager
    {
        private string _presetFolderPath;

        public ParameterPresetManager()
        {
            // e.g., determine your mod folder or presets folder
            _presetFolderPath = Path.Combine(Application.dataPath, "Mods", "SledTuner", "Presets");
            if (!Directory.Exists(_presetFolderPath))
            {
                Directory.CreateDirectory(_presetFolderPath);
            }
        }

        public Dictionary<string, float> LoadPreset(string presetName)
        {
            string filePath = Path.Combine(_presetFolderPath, presetName + ".json");
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[ParameterPresetManager] Preset not found: {filePath}");
                return new Dictionary<string, float>();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<Dictionary<string, float>>(json);
                if (data == null)
                {
                    Debug.LogWarning("[ParameterPresetManager] Preset data is null.");
                    return new Dictionary<string, float>();
                }
                return data;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ParameterPresetManager] Failed to load preset '{presetName}': {ex.Message}");
                return new Dictionary<string, float>();
            }
        }

        public void SavePreset(string presetName, Dictionary<string, float> currentData)
        {
            string filePath = Path.Combine(_presetFolderPath, presetName + ".json");
            try
            {
                string json = JsonConvert.SerializeObject(currentData, Formatting.Indented);
                File.WriteAllText(filePath, json);
                Debug.Log($"[ParameterPresetManager] Preset saved: {filePath}");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ParameterPresetManager] Failed to save preset '{presetName}': {ex.Message}");
            }
        }

        public Dictionary<string, float> GetDefaultPreset()
        {
            // If you ship with a default preset, load it from a known file or return some default values.
            Debug.Log("[ParameterPresetManager] Returning default preset (hardcoded).");
            return new Dictionary<string, float>
            {
                { "engine_power", 100000f },
                { "suspension_stiffness", 0.7f }
                // etc.
            };
        }
    }
}
