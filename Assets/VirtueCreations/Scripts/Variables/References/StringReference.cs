using UnityEngine;

namespace VIRTUE {
    [System.Serializable]
    public sealed class StringReference {
        [SerializeField]
        bool useConstant = true;
        [SerializeField]
        string constantValue;
        [SerializeField]
        StringVariable variable;

        public string Value {
            get => useConstant ? constantValue : variable.value;
            set {
                if (useConstant)
                    constantValue = value;
                else
                    variable.value = value;
            }
        }
    }
}