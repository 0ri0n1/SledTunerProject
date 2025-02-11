using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using UnityEngine;
using MelonLoader;

namespace SledTunerProject.SledParameterSystem
{
    /// <summary>
    /// Encapsulates reflection-based reading/writing of component fields.
    /// Builds a dictionary of fields and properties for each known component.
    /// </summary>
    public class ReflectionCache
    {
        // Reflection cache: componentName -> (fieldName -> MemberWrapper)
        private Dictionary<string, Dictionary<string, MemberWrapper>> _reflectionDict
            = new Dictionary<string, Dictionary<string, MemberWrapper>>();

        // We need access to the main snowmobile body to find components
        private GameObject _snowmobileBody;

        public ReflectionCache(GameObject snowmobileBody)
        {
            _snowmobileBody = snowmobileBody;
        }

        /// <summary>
        /// Build reflection cache for each component+field in 'componentsToInspect'.
        /// </summary>
        public void BuildCache(Dictionary<string, string[]> componentsToInspect)
        {
            _reflectionDict.Clear();

            foreach (var kvp in componentsToInspect)
            {
                string compName = kvp.Key;
                string[] fields = kvp.Value;

                var comp = GetComponentByName(compName);
                var memberDict = new Dictionary<string, MemberWrapper>();

                if (comp != null)
                {
                    Type type = comp.GetType();
                    foreach (string field in fields)
                    {
                        // Skip color channels for Light
                        if (compName == "Light" && (field == "r" || field == "g" || field == "b" || field == "a"))
                        {
                            // do nothing
                        }
                        else
                        {
                            MemberWrapper wrapper = new MemberWrapper();

                            FieldInfo fi = type.GetField(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                            if (fi != null)
                            {
                                wrapper.Field = fi;
                            }
                            else
                            {
                                PropertyInfo pi = type.GetProperty(field, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                if (pi != null) wrapper.Property = pi;
                            }

                            // e.g. handle Ragdoll alt names
                            if (!wrapper.IsValid && compName == "RagDollCollisionController")
                            {
                                if (field == "ragdollThreshold" || field == "ragdollThresholdDownFactor")
                                {
                                    string altName = (field == "ragdollThreshold") ? "ragdollTreshold" : "ragdollTresholdDownFactor";
                                    FieldInfo altFi = type.GetField(altName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                    if (altFi != null)
                                    {
                                        MelonLogger.Warning($"[ReflectionCache] Found alt field '{altName}' for {compName}, using for '{field}'.");
                                        wrapper.Field = altFi;
                                    }
                                    else
                                    {
                                        PropertyInfo altPi = type.GetProperty(altName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                                        if (altPi != null)
                                        {
                                            MelonLogger.Warning($"[ReflectionCache] Found alt property '{altName}' for {compName}, using for '{field}'.");
                                            wrapper.Property = altPi;
                                        }
                                    }
                                }
                            }

                            memberDict[field] = wrapper;
                        }
                    }
                }
                else
                {
                    // if comp not found, store empty placeholders
                    foreach (string field in fields)
                    {
                        memberDict[field] = new MemberWrapper();
                    }
                }

                _reflectionDict[compName] = memberDict;
            }

            MelonLogger.Msg("[ReflectionCache] BuildCache complete.");
        }

        /// <summary>
        /// Reads a field's value from the cached reflection data.
        /// </summary>
        public object TryReadMember(string compName, string fieldName)
        {
            // special handling for Light color channels
            if (compName == "Light" && (fieldName == "r" || fieldName == "g" || fieldName == "b" || fieldName == "a"))
            {
                var lightComp = (Light)GetComponentByName("Light");
                if (lightComp == null) return "(No Light)";
                Color c = lightComp.color;
                switch (fieldName)
                {
                    case "r": return c.r;
                    case "g": return c.g;
                    case "b": return c.b;
                    case "a": return c.a;
                    default: return "(Unknown color channel)";
                }
            }

            // fallback to reflection dictionary
            if (!_reflectionDict.ContainsKey(compName)) return $"(No reflection entry for {compName})";
            var fields = _reflectionDict[compName];
            if (!fields.ContainsKey(fieldName) || !fields[fieldName].IsValid) return $"(Not found: {fieldName})";

            var wrapper = fields[fieldName];
            if (!wrapper.CanRead) return "(Not readable)";

            try
            {
                var comp = GetComponentByName(compName);
                if (comp == null) return $"(Component {compName} not found)";

                object raw = (wrapper.Field != null)
                    ? wrapper.Field.GetValue(comp)
                    : wrapper.Property.GetValue(comp);

                return ConvertOrSkip(raw, wrapper.MemberType);
            }
            catch (Exception ex)
            {
                return $"Error reading '{fieldName}': {ex.Message}";
            }
        }

        /// <summary>
        /// Writes a field's value via reflection.
        /// </summary>
        public void ApplyField(string compName, string fieldName, object value)
        {
            // special handling for Light color channels
            if (compName == "Light" && (fieldName == "r" || fieldName == "g" || fieldName == "b" || fieldName == "a"))
            {
                var lightComp = (Light)GetComponentByName("Light");
                if (lightComp == null) return;
                Color c = lightComp.color;

                float floatVal = 0f;
                if (value is double dVal) floatVal = (float)dVal;
                else if (value is float fVal) floatVal = fVal;
                else if (value is int iVal) floatVal = iVal;
                else
                {
                    try { floatVal = Convert.ToSingle(value); } catch { }
                }

                switch (fieldName)
                {
                    case "r": c.r = Mathf.Clamp01(floatVal); break;
                    case "g": c.g = Mathf.Clamp01(floatVal); break;
                    case "b": c.b = Mathf.Clamp01(floatVal); break;
                    case "a": c.a = Mathf.Clamp01(floatVal); break;
                }
                lightComp.color = c;
                return;
            }

            if (!_reflectionDict.ContainsKey(compName)) return;
            if (!_reflectionDict[compName].ContainsKey(fieldName)) return;

            var wrapper = _reflectionDict[compName][fieldName];
            if (!wrapper.IsValid || !wrapper.CanWrite) return;

            var comp = GetComponentByName(compName);
            if (comp == null) return;

            try
            {
                object converted = ConvertValue(value, wrapper.MemberType);
                if (wrapper.Field != null)
                    wrapper.Field.SetValue(comp, converted);
                else
                    wrapper.Property.SetValue(comp, converted, null);
            }
            catch (Exception ex)
            {
                MelonLogger.Error($"[ReflectionCache] Error setting {compName}.{fieldName}: {ex.Message}");
            }
        }

        /// <summary>
        /// Returns the type of the field or property from the cache.
        /// </summary>
        public Type GetMemberType(string compName, string fieldName)
        {
            if (!_reflectionDict.ContainsKey(compName)) return null;
            if (!_reflectionDict[compName].ContainsKey(fieldName)) return null;
            var w = _reflectionDict[compName][fieldName];
            return w.MemberType;
        }

        // =======================
        // Helper Methods
        // =======================
        private Component GetComponentByName(string compName)
        {
            if (_snowmobileBody == null) return null;

            if (compName == "Rigidbody") return _snowmobileBody.GetComponent<Rigidbody>();
            if (compName == "Light")
            {
                Transform t = _snowmobileBody.transform.Find("Spot Light");
                if (t != null) return t.GetComponent<Light>();
                return null;
            }
            // ... handle special RagDollCollisionController, Shock, etc. like your old code...
            return _snowmobileBody.GetComponent(compName);
        }

        private object ConvertOrSkip(object raw, Type fieldType)
        {
            if (raw == null) return null;
            if (fieldType == typeof(Vector3)) return (Vector3)raw;
            if (fieldType != null && typeof(UnityEngine.Object).IsAssignableFrom(fieldType))
                return "(Skipped UnityEngine.Object)";
            if (fieldType != null && fieldType.IsEnum) return raw;
            if (fieldType != null && !fieldType.IsPrimitive
                && fieldType != typeof(string) && fieldType != typeof(decimal) && !fieldType.IsEnum)
            {
                return "(Skipped complex type)";
            }
            return raw;
        }

        private object ConvertValue(object raw, Type targetType)
        {
            if (raw == null || targetType == null) return null;
            if (targetType == typeof(Vector3)) return raw;

            try
            {
                if (targetType == typeof(float) && raw is double dVal) return (float)dVal;
                if (targetType == typeof(int) && raw is long lVal) return (int)lVal;
                if (targetType == typeof(bool) && raw is bool bVal) return bVal;
                if (targetType.IsInstanceOfType(raw)) return raw;
                return Convert.ChangeType(raw, targetType);
            }
            catch
            {
                return raw;
            }
        }

        // A small struct for wrapping field vs. property
        private struct MemberWrapper
        {
            public FieldInfo Field;
            public PropertyInfo Property;

            public bool IsValid => Field != null || Property != null;
            public bool CanRead => Field != null || (Property != null && Property.CanRead);
            public bool CanWrite => Field != null || (Property != null && Property.CanWrite);
            public Type MemberType => Field != null ? Field.FieldType : Property?.PropertyType;
        }
    }
}
