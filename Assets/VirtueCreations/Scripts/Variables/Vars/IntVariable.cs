using UnityEngine;
using UnityEngine.Events;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Int Variable", menuName = "Variable/Create Int Var"), ManageableData]
    public sealed class IntVariable : ScriptableObject {
        [SerializeField]
        int value;

        [HideInInspector]
        public UnityIntEvent OnValueChanged = new UnityIntEvent ();

        public int Value {
            get => value;
            set {
                this.value = value;
                OnValueChanged?.Invoke (value);
            }
        }

        public void AddListener (UnityAction<int> listener) {
            OnValueChanged.AddListener (listener);
        }

        public void RemoveListener (UnityAction<int> listener) {
            OnValueChanged.RemoveListener (listener);
        }
    }
}