using UnityEngine;
using System.Collections.Generic;
using System.IO;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// Displays a list of existing JSON configs, allows create/rename/delete actions.
    /// </summary>
    public class FileSelectionPanel
    {
        private List<string> _files = new List<string>();
        private int _selectedIndex = -1;
        private string _configDirectory = "Configs"; // or a path from ConfigManager

        public FileSelectionPanel()
        {
            // Could set _configDirectory from some global path or ConfigManager reference.
        }

        public void ScanConfigDirectory()
        {
            _files.Clear();
            if (!Directory.Exists(_configDirectory))
                Directory.CreateDirectory(_configDirectory);

            string[] jsonFiles = Directory.GetFiles(_configDirectory, "*.json");
            _files.AddRange(jsonFiles);
            Debug.Log($"[FileSelectionPanel] Found {_files.Count} config file(s).");
        }

        public void OnFileSelected(int index)
        {
            if (index >= 0 && index < _files.Count)
            {
                _selectedIndex = index;
                Debug.Log($"[FileSelectionPanel] Selected file: {_files[index]}");
                // e.g., pass the file to GUIManager or ConfigManager
            }
        }

        public void CreateNewConfig(string newName)
        {
            // e.g., create an empty JSON file
            string path = Path.Combine(_configDirectory, newName + ".json");
            File.WriteAllText(path, "{}");
            Debug.Log($"[FileSelectionPanel] Created new config: {path}");
            ScanConfigDirectory();
        }

        public void DeleteConfig(int index)
        {
            if (index >= 0 && index < _files.Count)
            {
                string fileToDelete = _files[index];
                File.Delete(fileToDelete);
                Debug.Log($"[FileSelectionPanel] Deleted file: {fileToDelete}");
                ScanConfigDirectory();
            }
        }

        public void RenameConfig(int index, string newName)
        {
            if (index >= 0 && index < _files.Count)
            {
                string oldPath = _files[index];
                string newPath = Path.Combine(_configDirectory, newName + ".json");
                File.Move(oldPath, newPath);
                Debug.Log($"[FileSelectionPanel] Renamed config: {oldPath} -> {newPath}");
                ScanConfigDirectory();
            }
        }

        public void DrawFileList()
        {
            GUILayout.Label("Available Configs:");
            for (int i = 0; i < _files.Count; i++)
            {
                if (GUILayout.Button(Path.GetFileName(_files[i])))
                {
                    OnFileSelected(i);
                }
            }
        }
    }
}
