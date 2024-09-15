using UnityEngine;

namespace VIRTUE {
    public class Billboard : MonoBehaviour {
        void LateUpdate () {
            transform.LookAt (transform.position + Helper.Camera.transform.forward);
        }
    }
}