using UnityEngine;
using UnityEngine.InputSystem;

namespace VIRTUE {
    public class AgentUtil : MonoBehaviour {
        [SerializeField]
        InputActionReference touchPress;

        [SerializeField]
        InputActionReference touchPosition;

        void Awake () {
            touchPress.action.Enable ();
            touchPosition.action.Enable ();
        }

        void Update () {
            if (touchPosition.action.WasPerformedThisFrame ()) {
                GetComponent<Agent> ().OnTouchPress (touchPosition.action.ReadValue<Vector2> ());
            }
        }
    }
}