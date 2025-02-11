using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using MelonLoader;
using System.Reflection;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// The main orchestrator for parameter data and game object references.
    /// Delegates reflection tasks to ReflectionCache, and loads parameter metadata from ParameterDefinitions.
    /// </summary>
    public class SledParameterManager
    {
        public readonly Dictionary<string, string[]> ComponentsToInspect;
        public string JsonFolderPath { get; }
        public bool IsInitialized { get; private set; } = false;

        // references
        private GameObject _snowmobileBody;
        private Rigidbody _rigidbody;
        private Light _light;

        // We store original/current param data
        private Dictionary<string, Dictionary<string, object>> _originalValues;
        private Dictionary<string, Dictionary<string, object>> _currentValues;

        // The reflection cache for reading/writing fields
        private ReflectionCache _reflectionCache;

        // The parameter metadata dictionary: compName -> (fieldName -> ParameterMetadata)
        private Dictionary<string, Dictionary<string, ParameterMetadata>> _parameterMetadata;

        public SledParameterManager()
        {
            // Define the components and field names to inspect.
            ComponentsToInspect = new Dictionary<string, string[]>
            {
                ["SnowmobileController"] = new string[]
                {
                    "leanSteerFactorSoft", "leanSteerFactorTrail", "throttleExponent", "drowningDepth", "drowningTime",
                    "isEngineOn", "isStuck", "canRespawn", "hasDrowned", "rpmSensitivity", "rpmSensitivityDown",
                    "minThrottleOnClutchEngagement", "clutchRpmMin", "clutchRpmMax", "isHeadlightOn",
                    "wheelieThreshold", "driverTorgueFactorRoll", "driverTorgueFactorPitch",
                    "snowmobileTorgueFactor", "isWheeling"
                },
                ["SnowmobileControllerBase"] = new string[]
                {
                    "skisMaxAngle", "driverZCenter", "enableVerticalWeightTransfer",
                    "trailLeanDistance", "switchbackTransitionTime",
                    "toeAngle", "hopOverPreJump", "switchBackLeanDistance"
                },
                ["MeshInterpretter"] = new string[]
                {
                    "power", "powerEfficiency", "breakForce", "frictionForce", "trackMass", "coefficientOfFriction",
                    "snowPushForceFactor", "snowPushForceNormalizedFactor", "snowSupportForceFactor",
                    "maxSupportPressure", "lugHeight", "snowOutTrackWidth", "pitchFactor",
                    "drivetrainMinSpeed", "drivetrainMaxSpeed1", "drivetrainMaxSpeed2"
                },
                ["SnowParameters"] = new string[]
                {
                    "snowNormalConstantFactor", "snowNormalDepthFactor",
                    "snowFrictionFactor", "snowNormalSpeedFactor"
                },
                ["SuspensionController"] = new string[]
                {
                    "suspensionSubSteps", "antiRollBarFactor", "skiAutoTurn",
                    "trackRigidityFront", "trackRigidityRear", "reduceSuspensionForceByTilt"
                },
                ["Stabilizer"] = new string[]
                {
                    "trackSpeedGyroMultiplier", "idleGyro", "trackSpeedDamping", "damping"
                },
                ["RagDollCollisionController"] = new string[]
                {
                    "ragdollThreshold", "ragdollThresholdDownFactor"
                },
                ["Rigidbody"] = new string[]
                {
                    "mass", "drag", "angularDrag", "useGravity", "maxAngularVelocity"
                },
                ["Light"] = new string[] { "r", "g", "b", "a" },
                ["Shock"] = new string[]
                {
                    "compression", "mass", "maxCompression", "velocity"
                }
            };

            _originalValues = new Dictionary<string, Dictionary<string, object>>();
            _currentValues = new Dictionary<string, Dictionary<string, object>>();

            // create a folder for JSON if needed
            string basePath = Path.Combine(Directory.GetCurrentDirectory(), "Mods", "SledTuner");
            if (!Directory.Exists(basePath)) Directory.CreateDirectory(basePath);
            JsonFolderPath = basePath;

            // Load up parameter metadata from ParameterDefinitions
            _parameterMetadata = ParameterDefinitions.CreateParameterMetadata();

            MelonLogger.Msg("[SledParameterManager] Constructed, waiting to initialize components.");
        }

        /// <summary>
        /// Called after the sled spawns. We find the snowmobile, build reflection, etc.
        /// </summary>
        public void InitializeComponents()
        {
            IsInitialized = false;
            MelonLogger.Msg("[SledParameterManager] Initializing sled components...");

            var snowmobile = GameObject.Find("Snowmobile(Clone)");
            if (snowmobile == null)
            {
                MelonLogger.Warning("[SledParameterManager] 'Snowmobile(Clone)' not found, no init.");
                return;
            }
            var bodyTransform = snowmobile.transform.Find("Body");
            if (bodyTransform == null)
            {
                MelonLogger.Warning("[SledParameterManager] 'Body' not found under 'Snowmobile(Clone)'.");
                return;
            }

            _snowmobileBody = bodyTransform.gameObject;
            _rigidbody = _snowmobileBody.GetComponent<Rigidbody>();

            var spotLight = _snowmobileBody.transform.Find("Spot Light");
            if (spotLight != null)
                _light = spotLight.GetComponent<Light>();

            // Now create ReflectionCache with the body
            _reflectionCache = new ReflectionCache(_snowmobileBody);
            // Build the cache
            _reflectionCache.BuildCache(ComponentsToInspect);

            // Inspect components to fill original/current dictionaries
            _originalValues = InspectSledComponents();
            _currentValues = InspectSledComponents();

            if (_originalValues != null)
            {
                IsInitialized = true;
                MelonLogger.Msg("[SledParameterManager] Sled components initialized successfully.");
            }
            else
            {
                MelonLogger.Warning("[SledParameterManager] Inspection failed, no original values.");
            }
        }

        /// <summary>
        /// Applies the current values to the actual sled components via reflection.
        /// </summary>
        public void ApplyParameters()
        {
            foreach (var compEntry in _currentValues)
            {
                string compName = compEntry.Key;
                foreach (var fieldEntry in compEntry.Value)
                {
                    string fieldName = fieldEntry.Key;
                    object value = fieldEntry.Value;
                    _reflectionCache.ApplyField(compName, fieldName, value);
                }
            }
            MelonLogger.Msg("[SledParameterManager] Applied parameters to sled.");
        }

        /// <summary>
        /// Reverts parameters to original (at time of init).
        /// </summary>
        public void RevertParameters()
        {
            if (_originalValues == null) return;

            foreach (var compKvp in _originalValues)
            {
                string compName = compKvp.Key;
                foreach (var fieldKvp in compKvp.Value)
                {
                    string fieldName = fieldKvp.Key;
                    object value = fieldKvp.Value;
                    _reflectionCache.ApplyField(compName, fieldName, value);
                }
            }
            MelonLogger.Msg("[SledParameterManager] Reverted params to original values.");
        }

        public Dictionary<string, Dictionary<string, object>> GetCurrentParameters()
        {
            return _currentValues;
        }

        /// <summary>
        /// Allows external code to set the current param dictionary, then apply it.
        /// </summary>
        public void SetParameters(Dictionary<string, Dictionary<string, object>> data)
        {
            _currentValues = data;
            ApplyParameters();
        }

        public void ResetParameters()
        {
            RevertParameters();
        }

        /// <summary>
        /// Retrieves the current value for (compName, fieldName).
        /// </summary>
        public object GetFieldValue(string componentName, string fieldName)
        {
            if (_currentValues.TryGetValue(componentName, out var dict) &&
                dict.TryGetValue(fieldName, out var val))
            {
                return val;
            }
            return null;
        }

        /// <summary>
        /// Looks up the reflection type for a given field. Used by the GUI to see if it's float, bool, etc.
        /// </summary>
        public Type GetFieldType(string componentName, string fieldName)
        {
            return _reflectionCache?.GetMemberType(componentName, fieldName);
        }

        /// <summary>
        /// For the GUI, fetch min from the metadata dictionary if available.
        /// </summary>
        public float GetSliderMin(string componentName, string fieldName)
        {
            if (_parameterMetadata.TryGetValue(componentName, out var metaDict)
                && metaDict.TryGetValue(fieldName, out var meta))
            {
                return meta.MinValue;
            }
            return -100f;
        }

        public float GetSliderMax(string componentName, string fieldName)
        {
            if (_parameterMetadata.TryGetValue(componentName, out var metaDict)
                && metaDict.TryGetValue(fieldName, out var meta))
            {
                return meta.MaxValue;
            }
            return 100f;
        }

        /// <summary>
        /// Updates the in-memory _currentValues with a new val for (component,field).
        /// The reflection apply happens when you call ApplyParameters.
        /// </summary>
        public void SetFieldValue(string componentName, string fieldName, object value)
        {
            if (!_currentValues.ContainsKey(componentName))
                _currentValues[componentName] = new Dictionary<string, object>();
            _currentValues[componentName][fieldName] = value;
        }

        /// <summary>
        /// Example: returns the name of the sled from a property, if found.
        /// </summary>
        public string GetSledName()
        {
            if (_snowmobileBody == null) return null;
            var controller = _snowmobileBody.GetComponent("SnowmobileController");
            if (controller == null)
            {
                MelonLogger.Warning("[SledParameterManager] SnowmobileController not found on Body.");
                return null;
            }

            var prop = controller.GetType().GetProperty("GKMNAIKNNMJ", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (prop == null || !prop.CanRead) return null;
            try
            {
                object val = prop.GetValue(controller, null);
                if (val == null) return null;
                string name = val.ToString();
                const string suffix = " (VehicleScriptableObject)";
                if (name.EndsWith(suffix))
                    return name.Substring(0, name.Length - suffix.Length);
                return name;
            }
            catch (Exception ex)
            {
                MelonLogger.Warning("[SledParameterManager] Error reading sled name: " + ex.Message);
                return null;
            }
        }

        // =========================
        // PRIVATE HELPER: Inspect Components
        // =========================
        private Dictionary<string, Dictionary<string, object>> InspectSledComponents()
        {
            var result = new Dictionary<string, Dictionary<string, object>>();

            foreach (var kvp in ComponentsToInspect)
            {
                string compName = kvp.Key;
                Dictionary<string, object> compValues = new Dictionary<string, object>();

                foreach (var field in kvp.Value)
                {
                    // Use reflectionCache to read
                    object val = _reflectionCache.TryReadMember(compName, field);
                    compValues[field] = val;
                }

                result[compName] = compValues;
            }

            MelonLogger.Msg("[SledParameterManager] InspectSledComponents complete.");
            return result;
        }
    }
}
