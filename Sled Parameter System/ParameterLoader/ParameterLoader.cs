using UnityEngine;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;  // For example, if using Newtonsoft

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// Loads raw JSON data from a file or string, converts into parameter dictionaries,
    /// handles defaults or malformed data.
    /// </summary>
    public class ParameterLoader
    {
        /// <summary>
        /// Reads a JSON file and returns a dictionary of parameterID -> float value
        /// or a more complex structure if needed.
        /// </summary>
        public Dictionary<string, float> LoadFromJSON(string filePath)
        {
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[ParameterLoader] File not found: {filePath}");
                return new Dictionary<string, float>();
            }

            try
            {
                string json = File.ReadAllText(filePath);
                var data = JsonConvert.DeserializeObject<Dictionary<string, float>>(json);
                if (data == null)
                {
                    Debug.LogWarning("[ParameterLoader] Deserialized data is null.");
                    return new Dictionary<string, float>();
                }
                return data;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ParameterLoader] Error reading JSON: {ex.Message}");
                return new Dictionary<string, float>();
            }
        }

        /// <summary>
        /// Applies the dictionary data to the ParameterStore.
        /// Existing parameters can be overwritten, or new ones added.
        /// </summary>
        public void ApplyToParameterStore(ParameterStore store, Dictionary<string, float> data)
        {
            if (data == null)
                return;

            foreach (var kvp in data)
            {
                store.SetParameterValue(kvp.Key, kvp.Value);
            }
            Debug.Log("[ParameterLoader] Applied JSON data to ParameterStore.");
        }
    }
}
