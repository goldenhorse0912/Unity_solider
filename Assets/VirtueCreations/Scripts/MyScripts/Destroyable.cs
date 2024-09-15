using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    public class Destroyable : MonoBehaviour {
        [SerializeField]
        Rigidbody[] rb;
        [SerializeField]
        Collider[] colliders;
        [SerializeField]
        ParticleSystem blastParticle;
        [SerializeField]
        float blastForce = 10f;

#if UNITY_EDITOR
        [Button]
        void SetUp () {
            rb = GetComponentsInChildren<Rigidbody> ();
            colliders = GetComponentsInChildren<Collider> ();
            foreach (var rig in rb) {
                rig.isKinematic = this;
                rig.useGravity = true;
            }
        }
#endif

        internal void GetDestroyed (GameObject parent, float cleanUpDuration = 3f, bool bigBlast = true) {
            foreach (var rig in rb) {
                rig.isKinematic = false;
            }
            if (bigBlast) {
                Blast ();
            } else {
                SmallBlast ();
            }
            DOVirtual.DelayedCall (cleanUpDuration, () => {
                foreach (var col in colliders) {
                    col.enabled = false;
                }
                Destroy (parent, 2f);
            });
        }

        void Blast () {
            foreach (var rig in rb) {
                rig.AddExplosionForce (blastForce, transform.position, 40f, 3f, ForceMode.Impulse);
            }
            if (blastParticle) {
                // blastParticle.Play ();
                blastParticle.gameObject.Show ();
            }
            // Manager.AudioManager.Play (SoundConstants.Clip_Explosion_nuke);
        }

        void SmallBlast () {
            foreach (var rig in rb) {
                rig.AddExplosionForce (blastForce, transform.position, 5f, 1f, ForceMode.Impulse);
            }
            if (blastParticle) {
                // blastParticle.Play ();
                blastParticle.gameObject.Show ();
            }
        }
    }
}