using UnityEngine;
using UnityEngine.Events;

namespace VIRTUE {
    public sealed class AnimationEvents : MonoBehaviour {
        [SerializeField]
        UnityEvent[] unityEvents;

        /// <summary>
        /// Invoked from Animation Event
        /// </summary>
        /// <param name="eventIndex">The index of the event to be invoked</param>
        public void InvokeEventByIndex (int eventIndex) {
            unityEvents[eventIndex]?.Invoke ();
        }
    }
}