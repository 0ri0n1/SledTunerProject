using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using MelonLoader;
using Newtonsoft.Json;
using SledTunerProject.SledParameterSystem;
using UnityEngine;

namespace SledTunerProject.ConfigSystem
{
    /// <summary>
    /// The main public class for config tasks:
    /// - Save/Load config
    /// - Reset parameters
    /// - Has an event ConfigurationLoaded
    /// - Delegates in-game toggles to InGameToggles
    /// </summary>
    public class ConfigManager
    {
        private readonly SledParameterManager _sledParameterManager;
        private ConfigInitialization _init;
        private InGameToggles _toggles;

        // event for the GUI
        public event Action ConfigurationLoaded;

        public ConfigManager(SledParameterManager sledParameterManager)
        {
            _sledParameterManager = sledParameterManager ?? throw new ArgumentNullException(nameof(sledParameterManager));

            // Create sub-components
            _init = new ConfigInitialization(this, _sledParameterManager);
            _toggles = new InGameToggles();
        }

        /// <summary>
        /// Returns the sub-component that handles initial waits, etc.
        /// </summary>
        public ConfigInitialization Initialization => _init;

        /// <summary>
        /// Provide easy access to toggles
        /// </summary>
        public InGameToggles Toggles => _toggles;

        // ============= FILE OPS =============
        public async void SaveConfiguration()
        {
            _init.InitializeIfNeeded();
            if (!_init.ReadyToUse())
            {
                MelonLogger.Warning("[ConfigManager] Not ready to save.");
                return;
            }

            string path = _init.ConfigFilePath;
            var data = _sledParameterManager.GetCurrentParameters();
            try
            {
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                await Task.Run(() => File.WriteAllText(path, json));
                MelonLogger.Msg($"[ConfigManager] Config saved to {path}");
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[ConfigManager] Error saving config: {ex.Message}");
            }
        }

        public async void LoadConfiguration()
        {
            _init.InitializeIfNeeded();
            if (!_init.ReadyToUse())
            {
                MelonLogger.Warning("[ConfigManager] Not ready to load.");
                return;
            }

            string path = _init.ConfigFilePath;
            if (!File.Exists(path))
            {
                MelonLogger.Warning($"[ConfigManager] No config file: {path}");
                return;
            }

            try
            {
                string json = await Task.Run(() => File.ReadAllText(path));
                var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, object>>>(json);
                if (data != null)
                {
                    _sledParameterManager.SetParameters(data);
                    MelonLogger.Msg($"[ConfigManager] Loaded config from {path}");
                    ConfigurationLoaded?.Invoke();
                }
                else
                {
                    MelonLogger.Warning("[ConfigManager] Loaded config is null or invalid.");
                }
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[ConfigManager] Error loading config: {ex.Message}");
            }
        }

        public void ResetParameters()
        {
            _sledParameterManager.ResetParameters();
            MelonLogger.Msg("[ConfigManager] Sled parameters reset to original.");
            ConfigurationLoaded?.Invoke();
        }

        // ============= IN-GAME TOGGLES =============
        public void ToggleRagdoll() => _toggles.ToggleRagdoll();
        public void ToggleTreeRenderer() => _toggles.ToggleTreeRenderer();
        public void TeleportSled() => _toggles.TeleportSled();
    }
}
