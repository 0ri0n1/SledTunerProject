using SledTunerProject.SledParameterSystem;
using System;
using System.Collections.Generic;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// Types of controls for editing parameters (slider, toggle, etc.).
    /// </summary>
    public enum ControlType
    {
        Slider,
        Toggle,
        Text,
        Dropdown
    }

    /// <summary>
    /// Describes metadata for a parameter: display name, description, min/max, control type, etc.
    /// </summary>
    public class ParameterMetadata
    {
        public string DisplayName;
        public string Description;
        public float MinValue;
        public float MaxValue;
        public ControlType Control;

        public ParameterMetadata(
            string displayName,
            string description,
            float minValue,
            float maxValue,
            ControlType control = ControlType.Slider)
        {
            DisplayName = displayName;
            Description = description;
            MinValue = minValue;
            MaxValue = maxValue;
            Control = control;
        }
    }

    /// <summary>
    /// Holds the parameter metadata for each component/field in a static dictionary.
    /// SledParameterManager will load these definitions in its constructor or init.
    /// </summary>
    public static class ParameterDefinitions
    {
        /// <summary>
        /// Returns a dictionary: 
        ///   Key = component name (e.g. "Rigidbody")
        ///   Value = dictionary of fieldName -> ParameterMetadata
        /// </summary>
        public static Dictionary<string, Dictionary<string, ParameterMetadata>> CreateParameterMetadata()
        {
            var meta = new Dictionary<string, Dictionary<string, ParameterMetadata>>();

            // Example: fill in with your big dictionary of metadata
            // ========== START FILL ==========

            // Metadata for SnowmobileController
            meta["SnowmobileController"] = new Dictionary<string, ParameterMetadata>
            {
                ["leanSteerFactorSoft"] = new ParameterMetadata("Lean Steer Factor (Soft)", "Soft steering factor", 0f, 10f),
                ["leanSteerFactorTrail"] = new ParameterMetadata("Lean Steer Factor (Trail)", "Trail steering factor", 0f, 10f),
                ["throttleExponent"] = new ParameterMetadata("Throttle Exponent", "Exponent for throttle input", 0f, 5f),
                ["drowningDepth"] = new ParameterMetadata("Drowning Depth", "Depth threshold before drowning", 0f, 10f),
                ["drowningTime"] = new ParameterMetadata("Drowning Time", "Time before drowning", 0f, 60f),
                ["isEngineOn"] = new ParameterMetadata("Engine On", "Engine status (toggle)", 0f, 1f, ControlType.Toggle),
                ["isStuck"] = new ParameterMetadata("Is Stuck", "Indicates if the sled is stuck", 0f, 1f, ControlType.Toggle),
                ["canRespawn"] = new ParameterMetadata("Can Respawn", "Indicates if the sled can respawn", 0f, 1f, ControlType.Toggle),
                ["hasDrowned"] = new ParameterMetadata("Has Drowned", "Indicates if the sled has drowned", 0f, 1f, ControlType.Toggle),
                ["rpmSensitivity"] = new ParameterMetadata("RPM Sensitivity", "Sensitivity of the RPM", 0f, 1f),
                ["rpmSensitivityDown"] = new ParameterMetadata("RPM Sensitivity Down", "Downward RPM sensitivity", 0f, 1f),
                ["minThrottleOnClutchEngagement"] = new ParameterMetadata("Min Throttle (Clutch)", "Minimum throttle during clutch engagement", 0f, 1f),
                ["clutchRpmMin"] = new ParameterMetadata("Clutch RPM Min", "Minimum clutch RPM", 0f, 1000f),
                ["clutchRpmMax"] = new ParameterMetadata("Clutch RPM Max", "Maximum clutch RPM", 0f, 10000f),
                ["isHeadlightOn"] = new ParameterMetadata("Headlight On", "Headlight status (toggle)", 0f, 1f, ControlType.Toggle),
                ["wheelieThreshold"] = new ParameterMetadata("Wheelie Threshold", "Angle threshold for wheelies", 0f, 90f),
                ["driverTorgueFactorRoll"] = new ParameterMetadata("Driver Torque Factor (Roll)", "Torque factor for roll", 0f, 100f),
                ["driverTorgueFactorPitch"] = new ParameterMetadata("Driver Torque Factor (Pitch)", "Torque factor for pitch", 0f, 100f),
                ["snowmobileTorgueFactor"] = new ParameterMetadata("Snowmobile Torque Factor", "Torque factor for the snowmobile", 0f, 100f),
                ["isWheeling"] = new ParameterMetadata("Is Wheeling", "Indicates if the sled is performing a wheelie", 0f, 1f, ControlType.Toggle)
            };

            // Metadata for SnowmobileControllerBase
            meta["SnowmobileControllerBase"] = new Dictionary<string, ParameterMetadata>
            {
                ["skisMaxAngle"] = new ParameterMetadata("Skis Max Angle", "Maximum angle of the skis", 0f, 90f),
                ["driverZCenter"] = new ParameterMetadata("Driver Z Center", "Center position on the Z axis for the driver", -10f, 10f),
                ["enableVerticalWeightTransfer"] = new ParameterMetadata("Vertical Weight Transfer", "Enable vertical weight transfer (toggle)", 0f, 1f, ControlType.Toggle),
                ["trailLeanDistance"] = new ParameterMetadata("Trail Lean Distance", "Distance for trail lean", 0f, 5f),
                ["switchbackTransitionTime"] = new ParameterMetadata("Switchback Transition Time", "Time to transition during a switchback", 0f, 5f),
                ["toeAngle"] = new ParameterMetadata("Toe Angle", "Toe angle of the skis", -45f, 45f),
                ["hopOverPreJump"] = new ParameterMetadata("Hop Over Pre Jump", "Pre jump hop over flag (toggle)", 0f, 1f, ControlType.Toggle),
                ["switchBackLeanDistance"] = new ParameterMetadata("Switchback Lean Distance", "Lean distance in switchback", 0f, 5f)
            };

            // Metadata for MeshInterpretter
            meta["MeshInterpretter"] = new Dictionary<string, ParameterMetadata>
            {
                ["power"] = new ParameterMetadata("Power", "Engine power", 0f, 200000f),
                ["powerEfficiency"] = new ParameterMetadata("Power Efficiency", "Efficiency of engine power", 0f, 1f),
                ["breakForce"] = new ParameterMetadata("Brake Force", "Force applied by brakes", 0f, 10000f),
                ["frictionForce"] = new ParameterMetadata("Friction Force", "Force of friction", 0f, 1000f),
                ["trackMass"] = new ParameterMetadata("Track Mass", "Mass of the track", 0f, 500f),
                ["coefficientOfFriction"] = new ParameterMetadata("Coefficient of Friction", "Coefficient for friction", 0f, 1f),
                ["snowPushForceFactor"] = new ParameterMetadata("Snow Push Force Factor", "Factor for pushing snow", 0f, 100f),
                ["snowPushForceNormalizedFactor"] = new ParameterMetadata("Snow Push Force Normalized Factor", "Normalized factor for pushing snow", 0f, 500f),
                ["snowSupportForceFactor"] = new ParameterMetadata("Snow Support Force Factor", "Force factor for snow support", 0f, 2000f),
                ["maxSupportPressure"] = new ParameterMetadata("Max Support Pressure", "Maximum support pressure", 0f, 100f),
                ["lugHeight"] = new ParameterMetadata("Lug Height", "Height of the lug", 0f, 2f),
                ["snowOutTrackWidth"] = new ParameterMetadata("Snow Out Track Width", "Width of the track for snow output", 0f, 5f),
                ["pitchFactor"] = new ParameterMetadata("Pitch Factor", "Factor affecting pitch", 0f, 20f),
                ["drivetrainMinSpeed"] = new ParameterMetadata("Drivetrain Min Speed", "Minimum drivetrain speed", 0f, 10f),
                ["drivetrainMaxSpeed1"] = new ParameterMetadata("Drivetrain Max Speed 1", "Primary maximum drivetrain speed", 0f, 50f),
                ["drivetrainMaxSpeed2"] = new ParameterMetadata("Drivetrain Max Speed 2", "Secondary maximum drivetrain speed", 0f, 50f)
            };

            // Metadata for SnowParameters
            meta["SnowParameters"] = new Dictionary<string, ParameterMetadata>
            {
                ["snowNormalConstantFactor"] = new ParameterMetadata("Snow Normal Constant Factor", "Constant factor for snow normal", 0f, 10f),
                ["snowNormalDepthFactor"] = new ParameterMetadata("Snow Normal Depth Factor", "Depth factor for snow normal", 0f, 10f),
                ["snowFrictionFactor"] = new ParameterMetadata("Snow Friction Factor", "Friction factor for snow", 0f, 10f),
                ["snowNormalSpeedFactor"] = new ParameterMetadata("Snow Normal Speed Factor", "Speed factor for snow normal", 0f, 10f)
            };

            // Metadata for SuspensionController
            meta["SuspensionController"] = new Dictionary<string, ParameterMetadata>
            {
                ["suspensionSubSteps"] = new ParameterMetadata("Suspension Sub Steps", "Number of sub-steps in suspension", 1f, 10f),
                ["antiRollBarFactor"] = new ParameterMetadata("Anti Roll Bar Factor", "Factor for the anti-roll bar", 0f, 5f),
                ["skiAutoTurn"] = new ParameterMetadata("Ski Auto Turn", "Automatically turn skis (toggle)", 0f, 1f, ControlType.Toggle),
                ["trackRigidityFront"] = new ParameterMetadata("Track Rigidity Front", "Rigidity of the front track", 0f, 10f),
                ["trackRigidityRear"] = new ParameterMetadata("Track Rigidity Rear", "Rigidity of the rear track", 0f, 10f),
                ["reduceSuspensionForceByTilt"] = new ParameterMetadata("Reduce Suspension Force By Tilt", "Reduce suspension force when tilted (toggle)", 0f, 1f, ControlType.Toggle)
            };

            // Metadata for Stabilizer
            meta["Stabilizer"] = new Dictionary<string, ParameterMetadata>
            {
                ["trackSpeedGyroMultiplier"] = new ParameterMetadata("Track Speed Gyro Multiplier", "Multiplier for track speed gyro", 0f, 10f),
                ["idleGyro"] = new ParameterMetadata("Idle Gyro", "Idle gyro value", 0f, 10f),
                // For Vector3 fields, the metadata applies to each axis.
                ["trackSpeedDamping"] = new ParameterMetadata("Track Speed Damping", "Damping factor for track speed (per axis)", 0f, 10f),
                ["damping"] = new ParameterMetadata("Damping", "Damping vector for stabilizer (per axis)", 0f, 10f)
            };

            // Metadata for RagDollCollisionController
            meta["RagDollCollisionController"] = new Dictionary<string, ParameterMetadata>
            {
                ["ragdollThreshold"] = new ParameterMetadata("Ragdoll Threshold", "Threshold for ragdoll activation", 0f, 1000f),
                ["ragdollThresholdDownFactor"] = new ParameterMetadata("Ragdoll Threshold Down Factor", "Down factor for ragdoll threshold", 0f, 10f)
            };

            // Metadata for Rigidbody
            meta["Rigidbody"] = new Dictionary<string, ParameterMetadata>
            {
                ["mass"] = new ParameterMetadata("Mass", "Mass of the rigidbody", 0f, 500f),
                ["drag"] = new ParameterMetadata("Drag", "Drag coefficient", 0f, 10f),
                ["angularDrag"] = new ParameterMetadata("Angular Drag", "Angular drag coefficient", 0f, 10f),
                ["useGravity"] = new ParameterMetadata("Use Gravity", "Determines if gravity is applied (toggle)", 0f, 1f, ControlType.Toggle),
                ["maxAngularVelocity"] = new ParameterMetadata("Max Angular Velocity", "Maximum angular velocity", 0f, 100f)
            };

            // Metadata for Light
            meta["Light"] = new Dictionary<string, ParameterMetadata>
            {
                ["r"] = new ParameterMetadata("Red", "Red channel of light", 0f, 1f),
                ["g"] = new ParameterMetadata("Green", "Green channel of light", 0f, 1f),
                ["b"] = new ParameterMetadata("Blue", "Blue channel of light", 0f, 1f),
                ["a"] = new ParameterMetadata("Alpha", "Alpha channel of light", 0f, 1f)
            };

            // Metadata for Shock
            meta["Shock"] = new Dictionary<string, ParameterMetadata>
            {
                ["compression"] = new ParameterMetadata("Compression", "Compression of the shock", 0f, 1f),
                ["mass"] = new ParameterMetadata("Mass", "Mass of the shock", 0f, 100f),
                ["maxCompression"] = new ParameterMetadata("Max Compression", "Maximum compression of the shock", 0f, 1f),
                ["velocity"] = new ParameterMetadata("Velocity", "Velocity of the shock", 0f, 100f)
            };

            // (Continue adding dictionaries for MeshInterpretter, SnowParameters, SuspensionController, etc.)
            // ========== END FILL ==========

            return meta;
        }
    }
}