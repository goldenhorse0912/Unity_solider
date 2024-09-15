using UnityEngine;

namespace VIRTUE {
    public class BulletHitParticle : MonoBehaviour {
        void Awake () {
            transform.parent = null;
            Destroy (gameObject,2f);
        }
    }
}