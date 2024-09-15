using UnityEngine;

namespace VIRTUE {
    public class Pause : MonoBehaviour {
#if UNITY_EDITOR
        void OnApplicationQuit () {
            PauseEvent.PauseGame ();
        }

#else
        void OnApplicationPause (bool pauseStatus) {
            if (pauseStatus) {
                PauseEvent.PauseGame ();
            }
        }
#endif
    }
}