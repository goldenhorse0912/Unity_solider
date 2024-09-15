using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu, ManageableData]
    public class Bullet_SO : ScriptableObject {
        public float speed;
        public float dmg;
        public GameObject hitParticlePrefab;
    }
}