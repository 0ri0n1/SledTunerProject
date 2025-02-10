using UnityEngine;
using System.Collections.Generic;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// Manages tabbed UI panels for different parameter categories.
    /// </summary>
    public class TabPanelManager
    {
        // Possibly an enum or data structure representing your categories:
        // e.g. "Engine", "Suspension", "Physics", etc.

        private List<string> _categories;
        private int _currentTabIndex = 0;

        public TabPanelManager()
        {
            _categories = new List<string> { "Engine", "Suspension", "Physics" };
        }

        /// <summary>
        /// Dynamically creates tab buttons or toggles for the categories.
        /// </summary>
        public void GenerateTabs(List<string> categories)
        {
            _categories = categories;
            // Implementation depends on your UI system.
            Debug.Log("[TabPanelManager] Tabs generated.");
        }

        /// <summary>
        /// Called when a tab is selected, e.g., user clicks on "Engine" tab.
        /// </summary>
        /// <param name="category">The selected category name.</param>
        public void OnTabSelected(string category)
        {
            // Switch active panel or content.
            _currentTabIndex = _categories.IndexOf(category);
            Debug.Log($"[TabPanelManager] Tab selected: {category}");
        }

        public void DrawTabs()
        {
            // IMGUI example (very minimal):
            GUILayout.BeginHorizontal();
            for (int i = 0; i < _categories.Count; i++)
            {
                if (GUILayout.Button(_categories[i]))
                {
                    OnTabSelected(_categories[i]);
                }
            }
            GUILayout.EndHorizontal();

            // Then draw the currently selected category’s content below:
            GUILayout.Label($"Current Tab: {_categories[_currentTabIndex]}");
        }
    }
}
