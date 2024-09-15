using UnityEngine.Events;

namespace VIRTUE {
    public static class PauseEvent {
        static UnityEvent GamePauseEvent = new();

        public static void AddListener (UnityAction listener) {
            GamePauseEvent.AddListener (listener);
        }

        public static void RemoveListener (UnityAction listener) {
            GamePauseEvent.RemoveListener (listener);
        }

        public static void PauseGame () {
            GamePauseEvent?.Invoke ();
        }
    }
}