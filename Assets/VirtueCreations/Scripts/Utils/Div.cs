using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    public class Div : MonoBehaviour {
        public float num1;
        public float num2;
        [ReadOnly]
        public float result;

        void OnValidate () {
            result = num1 / num2;
        }
    }
}