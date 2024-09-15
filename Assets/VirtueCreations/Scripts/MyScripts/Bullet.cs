using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VIRTUE {
    public class Bullet : PooledObject {
        bool _isTriggered;
        Vector3 _targetPos;
        Rigidbody _rb;
        LayerMask _myLayer;

        [SerializeField]
        TrailRenderer trail;

        public Bullet_SO bulledData;

        [ReadOnly]
        public HealthSystemComponent target;

        protected override void OnAwake () {
            base.OnAwake ();
            _rb = GetComponent<Rigidbody> ();
        }

        /*void OnDrawGizmos () {
            var dir = transform.DirectionTo (_targetPos);
            var targetPos = _targetPos + dir * 1f;
            Gizmos.DrawWireSphere (targetPos, 1f);
        }*/

        public void ShootAt (HealthSystemComponent targetHealth, LayerMask layerToIgnore) {
            if (!targetHealth) {
                Release ();
                return;
            }
            _myLayer = layerToIgnore;
            target = targetHealth;
            _targetPos = targetHealth.transform.position + Vector3.up;
            _rb.velocity = transform.DirectionTo (_targetPos) * bulledData.speed;
            DOVirtual.DelayedCall (3f, Release).SetId (GetHashCode ());
        }

        public void ShootInRadius (HealthSystemComponent targetHealth, LayerMask layerToIgnore) {
            if (!targetHealth) {
                Release ();
                return;
            }
            _myLayer = layerToIgnore;
            target = targetHealth;
            var radius = new Vector3 (Random.Range (-1f, 1f), 1, Random.Range (-1f, 1f));
            _targetPos = targetHealth.transform.position + radius;
            _rb.velocity = transform.DirectionTo (_targetPos) * bulledData.speed;
            DOVirtual.DelayedCall (3f, Release).SetId (GetHashCode ());
        }

        void OnTriggerEnter (Collider other) {
            if (_isTriggered || ((1 << other.gameObject.layer) & _myLayer) != 0 || !other.TryGetComponent (out HealthSystemComponent hsc)) {
                return;
            }
            if (!target.TryGetComponent (out Building _)) {
                if (!hsc.TryGetComponent (out Building _)) {
                    Hit (hsc);
                }
            } else {
                Hit (hsc);
            }
        }

        void Hit (HealthSystemComponent hsc) {
            if (hsc.GetHealthSystem ().IsDead ()) {
                return;
            }
            _isTriggered = true;
            hsc.GetHealthSystem ().Damage (bulledData.dmg);
            var particle = Instantiate (bulledData.hitParticlePrefab);
            particle.transform.position = hsc.transform.position + Vector3.up + hsc.transform.forward * 0.5f;
            Release ();
        }

        public override void OnGetFromPool () {
            _isTriggered = false;
            gameObject.Show ();
        }

        public override void OnReleaseToPool () {
            trail.Clear ();
            DOTween.Kill (GetHashCode ());
            _targetPos = transform.position;
            _rb.velocity = Vector3.zero;
            gameObject.Hide ();
        }
    }
}