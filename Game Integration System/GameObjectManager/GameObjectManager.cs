using UnityEngine;

namespace SledTunerProject.GameIntegrationSystem
{
    /// <summary>
    /// Bridges parameter changes to actual Unity GameObjects (sled, player, etc.).
    /// Provides methods invoked by ParameterUpdater or other systems.
    /// </summary>
    public class GameObjectManager : MonoBehaviour
    {
        // References to key GameObjects or components in Sledders.
        private GameObject _snowmobile;
        private GameObject _player;

        // Example references to specific controllers or scripts (replace with actual types from Sledders).
        private EngineController _engineController;
        private SuspensionController _suspensionController;
        private RagdollController _ragdollController;

        /// <summary>
        /// Initialize core references to the sled, player, and any relevant components.
        /// Typically called from Main.cs or SledParameterManager once the objects are loaded.
        /// </summary>
        /// <param name="snowmobile">The main snowmobile object.</param>
        /// <param name="player">The player object (if applicable).</param>
        public void InitializeObjects(GameObject snowmobile, GameObject player)
        {
            _snowmobile = snowmobile;
            _player = player;

            if (_snowmobile != null)
            {
                // Attempt to find relevant controllers on the snowmobile.
                _engineController = _snowmobile.GetComponentInChildren<EngineController>();
                _suspensionController = _snowmobile.GetComponentInChildren<SuspensionController>();
            }

            if (_player != null)
            {
                // Example: if the ragdoll component is attached to the player.
                _ragdollController = _player.GetComponentInChildren<RagdollController>();
            }

            Debug.Log("[GameObjectManager] Objects initialized.");
        }

        /// <summary>
        /// Updates the sled's engine power, if an EngineController is found.
        /// </summary>
        /// <param name="newPower">The new engine power value.</param>
        public void UpdateEnginePower(float newPower)
        {
            if (_engineController == null)
            {
                Debug.LogWarning("[GameObjectManager] EngineController not found. Cannot update engine power.");
                return;
            }

            _engineController.SetPower(newPower);
            Debug.Log($"[GameObjectManager] Engine power updated to {newPower}.");
        }

        /// <summary>
        /// Updates suspension stiffness, travel, or other attributes.
        /// </summary>
        /// <param name="stiffness">New stiffness value.</param>
        /// <param name="travel">New travel distance.</param>
        public void UpdateSuspension(float stiffness, float travel)
        {
            if (_suspensionController == null)
            {
                Debug.LogWarning("[GameObjectManager] SuspensionController not found. Cannot update suspension.");
                return;
            }

            _suspensionController.SetStiffness(stiffness);
            _suspensionController.SetTravel(travel);
            Debug.Log($"[GameObjectManager] Suspension updated. Stiffness={stiffness}, Travel={travel}.");
        }

        /// <summary>
        /// Enables or disables ragdoll physics on the player.
        /// </summary>
        /// <param name="enable">True to enable ragdoll, false to disable.</param>
        public void ToggleRagdoll(bool enable)
        {
            if (_ragdollController == null)
            {
                Debug.LogWarning("[GameObjectManager] RagdollController not found. Cannot toggle ragdoll.");
                return;
            }

            _ragdollController.SetRagdollState(enable);
            Debug.Log($"[GameObjectManager] Ragdoll toggled to {(enable ? "ON" : "OFF")}.");
        }

        /// <summary>
        /// Example teleport method to move the snowmobile or player to a given location.
        /// </summary>
        /// <param name="destination">Where to teleport.</param>
        public void TeleportToLocation(Vector3 destination)
        {
            if (_snowmobile != null)
            {
                _snowmobile.transform.position = destination;
                Debug.Log($"[GameObjectManager] Teleported snowmobile to {destination}.");
            }
        }

        /// <summary>
        /// Called on scene changes or resets if you need to re-fetch references.
        /// </summary>
        public void ReinitializeIfNeeded()
        {
            // For example, if the snowmobile was destroyed and re-spawned:
            // Find or re-assign references again. This is a placeholder example.
            if (_snowmobile == null)
            {
                _snowmobile = GameObject.Find("Snowmobile(Clone)");
                if (_snowmobile)
                {
                    _engineController = _snowmobile.GetComponentInChildren<EngineController>();
                    _suspensionController = _snowmobile.GetComponentInChildren<SuspensionController>();
                }
            }
            // Similarly for _player, etc.

            Debug.Log("[GameObjectManager] Reinitialized references if needed.");
        }
    }
}
