using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class SpecialHelicopter : Vehicle {
        Coroutine _attackingBuildingCoroutine;
        [SerializeField]
        DOTweenAnimation[] propellerAnimation;
        [SerializeField]
        Transform[] gunPoints;
        [SerializeField]
        ParticleSystem[] muzzleFlash;
        [SerializeField]
        Transform gfx;
        [SerializeField]
        float gapWithinBullets = .5f;

        protected override void OnStart () {
            base.OnStart ();
            DOVirtual.DelayedCall (.2f, () => {
                gfx.DOLocalMoveY (0f, 3f).OnComplete (() => {
                    gfx.DOLocalMoveY (.2f, .8f).SetLoops (-1, LoopType.Yoyo);
                });
                gfx.DOLocalRotate (Vector3.right * 15f, 3f);
                SearchEnemy ();
            });
        }

        protected override void OnDie () {
            base.OnDie ();
            foreach (var doTweenAnimation in propellerAnimation) {
                doTweenAnimation.DOKill ();
            }
        }

        protected override void ShootEnemy () {
            if (Hsc.GetHealthSystem ().IsDead ()) {
                return;
            }
            CachedTransform.DOLookAt (targetToShoot.transform.position.With (y: CachedTransform.position.y), .2f).OnComplete (() => {
                _attackingBuildingCoroutine = StartCoroutine (Shoot ());
            });

            IEnumerator Shoot () {
                for (var i = 0; i < gunPoints.Length; i++) {
                    var bullet = (Missile)Manager.Instance.PoolManager.GetFromPoolByName (troopData.bulletId);
                    bullet.transform.position = gunPoints[i].position;
                    bullet.ShootAt (targetToShoot, troopData.layerToIgnore);
                    muzzleFlash[i].Play ();
                    BulletSound.Play ();
                    yield return Helper.WaitFor (.2f);
                }
                yield return Helper.WaitFor (gapWithinBullets);
                CheckEnemyHealth ();
            }
        }

        protected override void StopShootingEnemy () {
            if (_attackingBuildingCoroutine != null) {
                StopCoroutine (_attackingBuildingCoroutine);
                _attackingBuildingCoroutine = null;
            }
            targetToShoot = null;
        }
    }
}