using UnityEngine;
using System.Collections.Generic;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// Allows searching for parameters by name, category, or value range.
    /// </summary>
    public class SearchManager
    {
        private string _searchQuery = "";

        public List<string> SearchParameters(string query)
        {
            // In practice, query the ParameterStore or SledParameterManager
            Debug.Log($"[SearchManager] Searching with query: {query}");
            // Return mock results for now
            return new List<string> { "engine_power", "suspension_stiffness" };
        }

        public void DisplaySearchResults(List<string> parameters)
        {
            if (parameters == null) return;
            GUILayout.Label("Search Results:");
            foreach (var param in parameters)
            {
                GUILayout.Label($" - {param}");
            }
        }

        public void DrawSearchUI()
        {
            GUILayout.Label("Search Parameters:");
            _searchQuery = GUILayout.TextField(_searchQuery, GUILayout.Width(200));
            if (GUILayout.Button("Search"))
            {
                var results = SearchParameters(_searchQuery);
                DisplaySearchResults(results);
            }
        }
    }
}
