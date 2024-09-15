using Sirenix.Utilities;
using UnityEngine;

namespace VIRTUE {
    public class ImmuneObject : MonoBehaviour {
        [SerializeField]
        float immuneDuration = 5f;
        HitFlash[] _hitFlashes;

        void Awake () {
            _hitFlashes = GetComponentsInChildren<HitFlash> (true);
        }

        public void StartFlash () {
            _hitFlashes.ForEach (x => x.StartFlashing (immuneDuration));
        }
    }
}