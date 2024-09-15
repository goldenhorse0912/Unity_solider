using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace VIRTUE {
    public static class DrowsyLogger {
        public static string Color (this string myStr, string color) => $"<color={color}>{myStr}</color>";

        static void DoLog (Action<string, Object> logFunction, string prefix, Object myObj, params object[] msg) {
            // #if UNITY_EDITOR
            var name = (myObj ? myObj.name : "NullObject").Color ("lightblue");
            logFunction ($"{prefix}[{name}]: {string.Join ("; ", msg)}\n ", myObj);
            // #endif
        }

        public static void Log (this Object myObj, params object[] msg) => DoLog (Debug.Log, "", myObj, msg);

        public static void LogError (this Object myObj, params object[] msg) => DoLog (Debug.LogError, "<!>".Color ("red"), myObj, msg);

        public static void LogWarning (this Object myObj, params object[] msg) => DoLog (Debug.LogWarning, "⚠️".Color ("yellow"), myObj, msg);

        public static void LogSuccess (this Object myObj, params object[] msg) => DoLog (Debug.Log, "☻".Color ("green"), myObj, msg);
    }
}