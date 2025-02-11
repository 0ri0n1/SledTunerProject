using System;
using System.Collections;
using System.IO;
using UnityEngine;
using MelonLoader;
using SledTunerProject.SledParameterSystem;

namespace SledTunerProject.ConfigSystem
{
    /// <summary>
    /// Handles sled detection, config file path construction,
    /// and the "initialize if needed" logic.
    /// </summary>
    public class ConfigInitialization
    {
        private readonly ConfigManager _parent;
        private readonly SledParameterManager _sledParameterManager;
        private bool _isInitialized = false;
        private string _configFilePath;

        public string ConfigFilePath => _configFilePath;

        public ConfigInitialization(ConfigManager parent, SledParameterManager spm)
        {
            _parent = parent;
            _sledParameterManager = spm ?? throw new ArgumentNullException(nameof(spm));
        }

        /// <summary>
        /// The coroutine that waits for snowmobile spawn, then calls InitializeIfNeeded.
        /// </summary>
        public IEnumerator WaitForSnowmobileAndInitialize()
        {
            while (!_isInitialized)
            {
                GameObject snowmobile = GameObject.Find("Snowmobile(Clone)");
                if (snowmobile != null)
                {
                    MelonLogger.Msg("[ConfigInitialization] Snowmobile found. Initializing config.");
                    InitializeIfNeeded();
                    yield break;
                }
                MelonLogger.Msg("[ConfigInitialization] Snowmobile not found. Retrying in 1 second...");
                yield return new WaitForSeconds(1f);
            }
        }

        /// <summary>
        /// Ensures the config is initialized (sets _configFilePath, logs the sled name, etc.).
        /// </summary>
        public void InitializeIfNeeded()
        {
            if (_isInitialized)
            {
                MelonLogger.Msg("[ConfigInitialization] Already initialized.");
                return;
            }
            MelonLogger.Msg("[ConfigInitialization] Attempting initialization...");
            UpdateConfigFilePath();
            string sledName = _sledParameterManager.GetSledName();
            if (string.IsNullOrEmpty(sledName))
            {
                MelonLogger.Warning("[ConfigInitialization] Failed: Sled name not found.");
                return;
            }
            MelonLogger.Msg($"[ConfigInitialization] Initialized for sled: {sledName}");
            _isInitialized = true;
        }

        /// <summary>
        /// Called inside InitializeIfNeeded to set the config file path from the sled name.
        /// </summary>
        private void UpdateConfigFilePath()
        {
            string sledName = _sledParameterManager.GetSledName() ?? "UnknownSled";
            string safeSledName = Utilities.MakeSafeFileName(sledName);
            _configFilePath = Path.Combine(_sledParameterManager.JsonFolderPath, $"{safeSledName}.json");
        }

        /// <summary>
        /// Checks if initialization is done and if the file path is valid.
        /// Used by ConfigManager before saving/loading.
        /// </summary>
        public bool ReadyToUse()
        {
            if (!_isInitialized)
            {
                MelonLogger.Warning("[ConfigInitialization] Not initialized yet.");
                return false;
            }
            if (string.IsNullOrEmpty(_configFilePath))
            {
                MelonLogger.Warning("[ConfigInitialization] Config file path is empty.");
                return false;
            }
            return true;
        }
    }
}
