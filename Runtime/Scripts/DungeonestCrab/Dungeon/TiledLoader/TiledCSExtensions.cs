using System;
using System.Globalization;
using System.Linq;
using TiledCS;
using UnityEngine;

namespace DungeonestCrab.Dungeon.TiledLoader {
    public static class TiledCSExtensions {
        public static TiledProperty GetPropertyWithType(this TiledObject obj, string name, TiledPropertyType type) {
            return GetPropWithType(obj, obj.properties, name, type);
        }
        public static bool GetBoolProperty(this TiledObject obj, string name, bool defValue) {
            var prop = GetPropWithType(obj, obj.properties, name, TiledPropertyType.Bool);
            if (prop == null) return defValue;

            return bool.Parse(prop.value);
        }

        public static bool GetBoolProperty(this TiledMap obj, string name, bool defValue) {
            var prop = GetPropWithType(obj, obj.Properties, name, TiledPropertyType.Bool);
            if (prop == null) return defValue;

            return bool.Parse(prop.value);
        }

        public static string GetStringProperty(this TiledObject obj, string name, string defValue) {
            var prop = GetPropWithType(obj, obj.properties, name, TiledPropertyType.String);
            if (prop == null) return defValue;

            return prop.value;
        }

        public static string GetStringProperty(this TiledMap obj, string name, string defValue) {
            var prop = GetPropWithType(obj, obj.Properties, name, TiledPropertyType.String);
            if (prop == null) return defValue;

            return prop.value;
        }

        public static float GetFloatProperty(this TiledObject obj, string name, float defValue) {
            var prop = GetPropWithType(obj, obj.properties, name, TiledPropertyType.Float);
            if (prop == null) return defValue;

            try {
                return float.Parse(prop.value, CultureInfo.InvariantCulture);
            } catch (Exception e) {
                Debug.LogWarning($"Exception parsing float value '{prop.value}' for name '{name}'. Returning default value {defValue}. Exception: {e}");
                return defValue;
            }
        }

        public static float GetFloatProperty(this TiledMap obj, string name, float defValue) {
            var prop = GetPropWithType(obj, obj.Properties, name, TiledPropertyType.Float);
            if (prop == null) return defValue;

            try {
                return float.Parse(prop.value, CultureInfo.InvariantCulture);
            } catch (Exception e) {
                Debug.LogWarning($"Exception parsing float value '{prop.value}' for name '{name}'. Returning default value {defValue}. Exception: {e}");
                return defValue;
            }
        }

        public static int GetIntProperty(this TiledObject obj, string name, int defValue) {
            var prop = GetPropWithType(obj, obj.properties, name, TiledPropertyType.Int);
            if (prop == null) return defValue;

            try {
                return int.Parse(prop.value);
            } catch (Exception e) {
                Debug.LogWarning($"Exception parsing int value '{prop.value}' for name '{name}'. Returning default value {defValue}. Exception: {e}");
                return defValue;
            }
        }

        public static int GetIntProperty(this TiledMap obj, string name, int defValue) {
            var prop = GetPropWithType(obj, obj.Properties, name, TiledPropertyType.Int);
            if (prop == null) return defValue;

            try {
                return int.Parse(prop.value);
            } catch (Exception e) {
                Debug.LogWarning($"Exception parsing int value '{prop.value}' for name '{name}'. Returning default value {defValue}. Exception: {e}");
                return defValue;
            }
        }

        public static Color GetColorProperty(this TiledObject obj, string name, Color defValue) {
            var prop = GetPropWithType(obj, obj.properties, name, TiledPropertyType.Color);
            if (prop == null) return defValue;

            return ParseColorString(prop.value);
        }

        public static Color GetColorProperty(this TiledMap obj, string name, Color defValue) {
            var prop = GetPropWithType(obj, obj.Properties, name, TiledPropertyType.Color);
            if (prop == null) return defValue;

            return ParseColorString(prop.value);
        }

        /// <summary>
        /// Expected format (tiled format): #aarrggbb
        /// </summary>
        static Color ParseColorString(string str) {
            float alpha = int.Parse(str.Substring(1, 2), NumberStyles.HexNumber);
            float r = int.Parse(str.Substring(3, 2), NumberStyles.HexNumber);
            float g = int.Parse(str.Substring(5, 2), NumberStyles.HexNumber);
            float b = int.Parse(str.Substring(7, 2), NumberStyles.HexNumber);
            return new Color(r / 255, g / 255, b / 255, alpha / 255);
        }

        static TiledProperty GetPropWithType(object src, TiledProperty[] props, string name, TiledPropertyType expectedType) {
            var prop = props.Where(x => x.name == name).FirstOrDefault();
            if (prop != null) {
                if (prop.type != expectedType) {
                    Debug.LogWarning($"Property {name} on {src} was type {prop.type}, expected {expectedType}!");
                    prop = null;
                }
            }
            return prop;
        }
    }
}