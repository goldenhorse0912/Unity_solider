using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class RagDoll : MonoBehaviour {
        Animator _anim;
        bool _isThrown;

        Collider[] _cols;
        Rigidbody[] _rb;

        [SerializeField]
        Collider _collider;
        [SerializeField]
        Rigidbody centerRb;

        void Awake () {
            _anim = GetComponentInChildren<Animator> ();
            _cols = GetComponentsInChildren<Collider> ();
            _rb = GetComponentsInChildren<Rigidbody> ();
            ToggleRagDoll (false);
        }

        void ToggleColliderState (bool state) {
            foreach (var t in _cols) {
                t.enabled = state;
            }
            _collider.enabled = !state;
        }

        void ToggleRigidbodyState (bool state) {
            foreach (var t in _rb) {
                t.isKinematic = state;
            }
        }

        internal void ToggleRagDoll (bool state) {
            ToggleColliderState (state);
            ToggleRigidbodyState (!state);
            _anim.enabled = !state;
        }

        internal void GetThrown () {
            if (_isThrown) return;
            _isThrown = true;
            centerRb.AddForce (-transform.forward * 20f + (Vector3.up * 30f), ForceMode.VelocityChange);
            DOVirtual.DelayedCall (3f, () => {
                foreach (var t in _cols) {
                    t.enabled = false;
                }
                Destroy (gameObject, 1f);
            });
        }
    }
}