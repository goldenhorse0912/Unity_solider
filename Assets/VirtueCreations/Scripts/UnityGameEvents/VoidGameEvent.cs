using UnityEngine;
using UnityEngine.Events;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Void Game Event", menuName = "Unity Game Events/Create New Void Game Event")]
    public class VoidGameEvent : ScriptableObject {
        public UnityAction OnEvent;

        public void Raise () {
            OnEvent?.Invoke ();
        }
    }
}