using UnityEngine;

namespace VIRTUE {
    public class Confetti : MonoBehaviour {
        ParticleSystem[] _confettis;

        void Awake () {
            _confettis = new ParticleSystem[2];
            _confettis[0] = transform.GetChild (0).GetComponent<ParticleSystem> ();
            _confettis[1] = transform.GetChild (1).GetComponent<ParticleSystem> ();
        }

        public void Play (Vector3 position) {
            transform.position = position;
            _confettis[0].Play ();
            _confettis[1].Play ();
        }
    }
}