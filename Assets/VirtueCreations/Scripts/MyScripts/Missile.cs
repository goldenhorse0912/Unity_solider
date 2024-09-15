using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    public class Missile : PooledObject {
        bool _isTriggered;
        Vector3 _targetPos;
        Rigidbody _rb;
        LayerMask _myLayer;

        public Bullet_SO bulledData;
        public float dmgRadius = 5f;

        protected override void OnAwake () {
            base.OnAwake ();
            _rb = GetComponent<Rigidbody> ();
        }

        /*void OnDrawGizmos () {
            var dir = transform.DirectionTo (_targetPos);
            var targetPos = _targetPos + dir * 1f;
            Gizmos.DrawWireSphere (targetPos, dmgRadius);
        }*/

        public void ShootAt (HealthSystemComponent targetHealth, LayerMask layerToIgnore) {
            _myLayer = layerToIgnore;
            _targetPos = targetHealth.transform.position + Vector3.up;
            CachedTransform.LookAt (_targetPos);
            _rb.velocity = transform.DirectionTo (_targetPos) * bulledData.speed;
            DOVirtual.DelayedCall (3f, Release).SetId (GetHashCode ());
        }

        void OnTriggerEnter (Collider other) {
            if (_isTriggered || ((1 << other.gameObject.layer) & _myLayer) != 0 || !other.TryGetComponent (out HealthSystemComponent hsc)) {
                return;
            }
            var targets = Physics.OverlapSphere (CachedTransform.position, dmgRadius, ~_myLayer);
            foreach (var col in targets) {
                if (col.TryGetComponent (out HealthSystemComponent enemyHsc)) {
                    enemyHsc.GetHealthSystem ().Damage (bulledData.dmg);
                }
            }
            _isTriggered = true;
            var particle = Instantiate (bulledData.hitParticlePrefab);
            particle.transform.position = hsc.transform.position + Vector3.up + hsc.transform.forward * 0.5f;
            Manager.Instance.CameraController.ShakeCamera ();
            Release ();
        }

        public override void OnGetFromPool () {
            _isTriggered = false;
            gameObject.Show ();
        }

        public override void OnReleaseToPool () {
            DOTween.Kill (GetHashCode ());
            _targetPos = transform.position;
            _rb.velocity = Vector3.zero;
            gameObject.Hide ();
        }
    }
}