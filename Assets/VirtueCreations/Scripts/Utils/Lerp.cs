using UnityEngine;

namespace VIRTUE {
    [ExecuteInEditMode]
    public class Lerp : MonoBehaviour {
        [SerializeField]
        Transform target;

        [SerializeField, Range (0f, 1f)]
        float lerp = 0.5f;

        Vector3 _startPos;

        void Start () {
            _startPos = transform.position;
        }

        void Update () {
            if (target == null) {
                return;
            }
            transform.position = Vector3.Lerp (_startPos, target.position, lerp);
        }
    }
}