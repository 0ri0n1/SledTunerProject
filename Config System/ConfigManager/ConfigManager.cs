using System;
using System.IO;
using UnityEngine;

namespace SledTunerProject.ConfigSystem
{
    /// <summary>
    /// Handles file operations (read/write JSON configs),
    /// backups, and optional auto-save intervals.
    /// </summary>
    public class ConfigManager
    {
        // The base directory where configs are stored.
        // e.g., "SledTuner/Configs" in Unity's persistent data path or a mod folder.
        private readonly string _configDirectory;

        // For auto-save logic (in minutes).
        private float _autoSaveInterval = 0f;
        private float _timeSinceLastSave = 0f;
        private bool _autoSaveEnabled = false;

        public ConfigManager()
        {
            // Example: place configs in "SledTuner/Configs" under persistentDataPath.
            // Adjust to your environment or user-defined location.
            _configDirectory = Path.Combine(Application.persistentDataPath, "SledTuner", "Configs");

            if (!Directory.Exists(_configDirectory))
            {
                Directory.CreateDirectory(_configDirectory);
                Debug.Log($"[ConfigManager] Created config directory at: {_configDirectory}");
            }
        }

        /// <summary>
        /// Reads the contents of a config file and returns raw JSON.
        /// </summary>
        public string ReadConfigFile(string fileName)
        {
            string filePath = Path.Combine(_configDirectory, fileName);
            if (!File.Exists(filePath))
            {
                Debug.LogWarning($"[ConfigManager] Config file not found: {filePath}");
                return string.Empty;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                Debug.Log($"[ConfigManager] Successfully read config from: {filePath}");
                return json;
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigManager] Error reading file '{filePath}': {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// Writes raw JSON data to a config file using an atomic approach.
        /// </summary>
        public void WriteConfigFile(string fileName, string jsonData)
        {
            string filePath = Path.Combine(_configDirectory, fileName);

            try
            {
                // 1. Write to a temp file first.
                string tempFile = filePath + ".tmp";
                File.WriteAllText(tempFile, jsonData);

                // 2. Atomically replace the old file with the new one.
                // If the target file doesn't exist, this just renames the temp file.
                // If it does exist, it replaces it safely.
                File.Replace(tempFile, filePath, null);
                Debug.Log($"[ConfigManager] Successfully wrote config to: {filePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigManager] Error writing to file '{filePath}': {ex.Message}");
            }
        }

        /// <summary>
        /// Creates a backup copy of a specified config file, appending a timestamp.
        /// </summary>
        public void BackupConfigFile(string fileName)
        {
            string sourcePath = Path.Combine(_configDirectory, fileName);
            if (!File.Exists(sourcePath))
            {
                Debug.LogWarning($"[ConfigManager] Cannot backup. File not found: {sourcePath}");
                return;
            }

            string backupDirectory = Path.Combine(_configDirectory, "Backups");
            if (!Directory.Exists(backupDirectory))
            {
                Directory.CreateDirectory(backupDirectory);
            }

            string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
            string backupFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{timestamp}.bak";
            string backupPath = Path.Combine(backupDirectory, backupFileName);

            try
            {
                File.Copy(sourcePath, backupPath);
                Debug.Log($"[ConfigManager] Backup created: {backupPath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigManager] Error backing up '{fileName}': {ex.Message}");
            }
        }

        /// <summary>
        /// Restores a backup file by copying it back to the main config directory
        /// and optionally overwriting the original config.
        /// </summary>
        public void RestoreBackup(string backupPath)
        {
            if (!File.Exists(backupPath))
            {
                Debug.LogWarning($"[ConfigManager] Backup file not found: {backupPath}");
                return;
            }

            try
            {
                string configName = Path.GetFileNameWithoutExtension(backupPath);
                // We might strip the timestamp if we appended it. Adjust as needed.
                // For simplicity, let's just call the restored file "RestoredConfig.json"
                string restoredFilePath = Path.Combine(_configDirectory, "RestoredConfig.json");
                File.Copy(backupPath, restoredFilePath, true); // Overwrite if needed

                Debug.Log($"[ConfigManager] Restored backup to: {restoredFilePath}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"[ConfigManager] Error restoring backup '{backupPath}': {ex.Message}");
            }
        }

        /// <summary>
        /// Sets an auto-save interval in minutes. Pass 0 or less to disable.
        /// Example usage: SetAutoSaveInterval(5f) to auto-save every 5 minutes.
        /// </summary>
        public void SetAutoSaveInterval(float minutes)
        {
            if (minutes > 0f)
            {
                _autoSaveInterval = minutes;
                _autoSaveEnabled = true;
                Debug.Log($"[ConfigManager] Auto-save enabled every {minutes} minute(s).");
            }
            else
            {
                _autoSaveInterval = 0f;
                _autoSaveEnabled = false;
                Debug.Log("[ConfigManager] Auto-save disabled.");
            }
        }

        /// <summary>
        /// This method should be called periodically (e.g., in Update()) if you want auto-saving.
        /// </summary>
        public void UpdateAutoSave(float deltaTime, Action onAutoSave)
        {
            if (!_autoSaveEnabled || _autoSaveInterval <= 0f)
                return;

            _timeSinceLastSave += deltaTime;
            // If we've hit the interval, save and reset the timer
            if (_timeSinceLastSave >= _autoSaveInterval * 60f) // minutes -> seconds
            {
                _timeSinceLastSave = 0f;
                Debug.Log("[ConfigManager] Auto-save triggered.");
                onAutoSave?.Invoke(); // Let the caller define what happens on auto-save
            }
        }
    }
}
