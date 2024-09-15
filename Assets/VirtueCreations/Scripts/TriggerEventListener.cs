using UnityEngine;
using UnityEngine.Events;

namespace VIRTUE {
    public class TriggerEventListener : MonoBehaviour {
        [SerializeField]
        string tagToCompare = "Player";

        [SerializeField]
        UnityEvent onTriggerEntered;

        void OnTriggerEnter (Collider other) {
            if (other.CompareTag (tagToCompare)) {
                onTriggerEntered.Invoke ();
            }
        }
    }
}