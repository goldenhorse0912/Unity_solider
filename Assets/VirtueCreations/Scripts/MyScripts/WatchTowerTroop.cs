using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace VIRTUE {
    public class WatchTowerTroop : CachedAnimator {
        internal HealthSystemComponent TowerHealth;

        [ValueDropdown ("@VIRTUE.PooledObjectIds.PoolIds")]
        [SerializeField]
        string bulletId;
        [SerializeField]
        Transform gunPoint, aimTargetPoint;
        [SerializeField, ReadOnly]
        internal HealthSystemComponent targetToShoot;

        AudioSource _bulletSound;
        Rig _rig;

        protected override void OnAwake () {
            base.OnAwake ();
            _rig = GetComponentInChildren<Rig> ();
            TryGetComponent (out _bulletSound);
        }

        void Start () {
            StartAiming (false);
        }

        void StartAiming (bool state) {
            if (state) {
                DOTween.To (x => _rig.weight = x, _rig.weight, 1f, .01f);
            } else {
                DOTween.To (x => _rig.weight = x, _rig.weight, 0f, .01f);
            }
        }

#region Animation Callback

        //Animation Callback
        public void AttackEnemy () {
            if (!targetToShoot || TowerHealth.GetHealthSystem ().IsDead ()) {
                StopAttack ();
                return;
            }
            CachedTransform.DOLookAt (targetToShoot.transform.position.With (y: CachedTransform.position.y), .1f).OnComplete (() => {
                aimTargetPoint.position = targetToShoot.transform.position + Vector3.up;
                var bullet = (Bullet)Manager.Instance.PoolManager.GetFromPoolByName (bulletId);
                bullet.transform.position = gunPoint.position;
                bullet.ShootAt (targetToShoot, gameObject.layer);
                _bulletSound.Play();
            });
        }

        //Animation Callback
        public void CheckEnemyHealth () {
            if (targetToShoot && !(targetToShoot.GetHealthSystem ().GetHealth () <= 0)) return;
            targetToShoot = null;
            StopAttack ();
        }

#endregion

        internal void StartAttack (HealthSystemComponent enemyHealth) {
            targetToShoot = enemyHealth;
            StartAiming (true);
            PlayAnim (AnimatorParams.Shoot, true);
        }

        void StopAttack () {
            StartAiming (false);
            PlayAnim (AnimatorParams.Shoot, false);
            targetToShoot = null;
        }
    }
}