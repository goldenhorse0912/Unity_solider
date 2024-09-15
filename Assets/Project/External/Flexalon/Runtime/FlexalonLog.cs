using UnityEngine;

namespace Flexalon
{
    internal class FlexalonLog
    {
        [System.Diagnostics.Conditional("FLEXALON_LOG")]
        public static void Log(string message)
        {
            Debug.Log(message);
        }

        [System.Diagnostics.Conditional("FLEXALON_LOG")]
        public static void Log(string message, FlexalonNode node)
        {
            Debug.Log(message + " (" + node?.GameObject?.name + ")");
        }

        [System.Diagnostics.Conditional("FLEXALON_LOG")]
        public static void Log<T>(string message, FlexalonNode node, T value)
        {
            Debug.Log(message + " (" + node?.GameObject?.name + "): " + value);
        }
    }
}