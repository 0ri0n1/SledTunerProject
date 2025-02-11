using UnityEngine;
using MelonLoader;

namespace SledTunerProject.ConfigSystem
{
    /// <summary>
    /// Contains methods for toggling in-game features:
    /// ragdoll, tree renderer, teleport, etc.
    /// </summary>
    public class InGameToggles
    {
        /// <summary>
        /// Toggles the ragdoll state by enabling/disabling RagDollManager and collision controllers.
        /// </summary>
        public void ToggleRagdoll()
        {
            MelonLogger.Msg("[InGameToggles] ToggleRagdoll called.");
            GameObject driverGO = GameObject.Find("Snowmobile(Clone)/Body/IK Player (Drivers)");
            if (driverGO == null)
            {
                MelonLogger.Warning("[InGameToggles] 'IK Player (Drivers)' not found.");
                return;
            }

            // Toggle RagDollManager
            Component ragdollManager = driverGO.GetComponent("RagDollManager");
            if (ragdollManager != null)
            {
                ToggleComponentEnabled(ragdollManager);
                MelonLogger.Msg("[InGameToggles] RagDollManager toggled.");
            }
            else
            {
                MelonLogger.Msg("[InGameToggles] RagDollManager not found.");
            }

            // Toggle RagDollCollisionController
            Component ragdollCollision = driverGO.GetComponent("RagDollCollisionController");
            if (ragdollCollision != null)
            {
                ToggleComponentEnabled(ragdollCollision);
                MelonLogger.Msg("[InGameToggles] RagDollCollision toggled.");
            }
            else
            {
                MelonLogger.Msg("[InGameToggles] RagDollCollisionController not found.");
            }
        }

        /// <summary>
        /// Toggles the tree renderer gameobject under LevelEssentials/TreeRenderer.
        /// </summary>
        public void ToggleTreeRenderer()
        {
            MelonLogger.Msg("[InGameToggles] ToggleTreeRenderer called.");
            GameObject levelEssentials = GameObject.Find("LevelEssentials");
            if (levelEssentials == null)
            {
                MelonLogger.Warning("[InGameToggles] 'LevelEssentials' not found.");
                return;
            }
            Transform treeRendererTransform = levelEssentials.transform.Find("TreeRenderer");
            if (treeRendererTransform == null)
            {
                MelonLogger.Warning("[InGameToggles] 'TreeRenderer' not found.");
                return;
            }
            GameObject treeRenderer = treeRendererTransform.gameObject;
            bool newState = !treeRenderer.activeSelf;
            treeRenderer.SetActive(newState);
            MelonLogger.Msg($"[InGameToggles] TreeRenderer toggled to {(newState ? "ON" : "OFF")}.");
        }

        /// <summary>
        /// Teleports the sled if needed. (Unimplemented logic.)
        /// </summary>
        public void TeleportSled()
        {
            MelonLogger.Msg("[InGameToggles] TeleportSled called. (TODO: Implement logic.)");
        }

        /// <summary>
        /// Helper method to toggle a component's enabled property if it exists.
        /// </summary>
        private void ToggleComponentEnabled(Component comp)
        {
            var prop = comp.GetType().GetProperty("enabled", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
            if (prop != null && prop.CanRead && prop.CanWrite)
            {
                bool currentState = (bool)prop.GetValue(comp, null);
                prop.SetValue(comp, !currentState, null);
            }
        }
    }
}
