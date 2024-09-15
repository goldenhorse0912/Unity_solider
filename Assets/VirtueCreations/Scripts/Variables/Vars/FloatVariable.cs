using System;
using UnityEngine;
using UnityEngine.Events;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Float Variable", menuName = "Variable/Create Float Var"), ManageableData]
    public sealed class FloatVariable : ScriptableObject {
        [SerializeField]
        float value;

        [HideInInspector]
        public UnityFloatEvent OnValueChanged = new UnityFloatEvent ();

        public float Value {
            get => value;
            set {
                if (Math.Abs (this.value - value) != 0) {
                    this.value = value;
                    OnValueChanged?.Invoke (value);
                }
            }
        }

        public void AddListener (UnityAction<float> listener) {
            OnValueChanged.AddListener (listener);
        }

        public void RemoveListener (UnityAction<float> listener) {
            OnValueChanged.RemoveListener (listener);
        }
    }
}