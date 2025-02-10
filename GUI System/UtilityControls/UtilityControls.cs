using UnityEngine;

namespace SledTunerProject.GUISystem
{
    /// <summary>
    /// Offers utility features for in-game actions like toggling ragdoll, teleportation, etc.
    /// Ties into the Game Integration System.
    /// </summary>
    public class UtilityControls
    {
        // You might store references to game integration classes:
        // private GameObjectManager _gameObjectManager;

        public UtilityControls()
        {
            // Possibly pass in or find references to managers
        }

        public void ToggleRagdoll()
        {
            Debug.Log("[UtilityControls] Toggling ragdoll...");
            // e.g., call _gameObjectManager.ToggleRagdoll(true/false)
        }

        public void TeleportToLocation(Vector3 destination)
        {
            Debug.Log($"[UtilityControls] Teleporting to {destination}...");
            // e.g., call _gameObjectManager.TeleportSled(destination)
        }

        public void DrawUtilityButtons()
        {
            // Example IMGUI usage:
            if (GUILayout.Button("Toggle Ragdoll"))
            {
                ToggleRagdoll();
            }
            if (GUILayout.Button("Teleport"))
            {
                TeleportToLocation(Vector3.zero); // Example
            }
        }
    }
}
