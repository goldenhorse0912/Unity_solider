using UnityEngine;

namespace VIRTUE {
    public class DontDestroy : MonoBehaviour {
        void Awake () {
            DontDestroyOnLoad (gameObject);
        }
    }
}