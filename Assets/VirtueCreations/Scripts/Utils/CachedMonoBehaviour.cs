using UnityEngine;

namespace VIRTUE {
    public abstract class CachedMonoBehaviour : MonoBehaviour {
        internal Transform CachedTransform { get; private set; }
        internal GameObject CachedGameObject { get; private set; }

        void Awake () {
            CachedTransform = transform;
            CachedGameObject = gameObject;
            OnAwake ();
        }

        protected virtual void OnAwake () { }
    }
}