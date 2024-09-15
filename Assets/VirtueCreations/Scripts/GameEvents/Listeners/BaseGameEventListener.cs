using UnityEngine;
using UnityEngine.Events;

namespace VIRTUE {
[DefaultExecutionOrder(-1)]
    public abstract class BaseGameEventListener<T, E, UER> : MonoBehaviour, IGameEventListener<T> where E : BaseGameEvent<T> where UER : UnityEvent<T> {
        [SerializeField]
        E gameEvent;

        [SerializeField]
        UER unityEventResponse;

        void OnEnable () {
            if (gameEvent == null) return;
            gameEvent.RegisterListener (this);
        }

        void OnDisable () {
            if (gameEvent == null) return;
            gameEvent.UnregisterListener (this);
        }

        public void OnEventRaised (T item) {
            unityEventResponse?.Invoke (item);
        }
    }
}