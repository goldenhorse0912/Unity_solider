using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class Drone : Vehicle {
        Coroutine _attackingBuildingCoroutine;
        [SerializeField]
        DOTweenAnimation[] propellerAnimation;
        [SerializeField]
        Transform[] wings;
        [SerializeField]
        Transform gfx, gun, gunPoint;
        [SerializeField]
        ParticleSystem muzzleFlash;
        [SerializeField]
        int noOfBulletsToShootAtATime = 5;
        [SerializeField]
        float gapWithinBullets = .5f;

        protected override void OnStart () {
            base.OnStart ();
            DOVirtual.DelayedCall (.2f, () => {
                gfx.DOLocalMoveY (0f, 3f).OnComplete (() => {
                    gfx.DOLocalMoveY (.2f, .8f).SetLoops (-1, LoopType.Yoyo);
                });
                RotateWings (true);
                SearchEnemy ();
            });
        }

        void RotateWings (bool state) {
            if (state) {
                foreach (var wing in wings) {
                    wing.DOKill ();
                    wing.DOLocalRotate (Vector3.right * 30f, .5f);
                }
            } else {
                foreach (var wing in wings) {
                    wing.DOKill ();
                    wing.DOLocalRotate (Vector3.zero, .5f);
                }
            }
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
            RotateWings (false);
            CachedTransform.DOLookAt (targetToShoot.transform.position.With (y: gun.position.y), .01f).OnComplete (() => {
                _attackingBuildingCoroutine = StartCoroutine (Shoot ());
            });

            IEnumerator Shoot () {
                for (var i = 0; i < noOfBulletsToShootAtATime; i++) {
                    muzzleFlash.Play ();
                    var bullet = (Bullet)Manager.Instance.PoolManager.GetFromPoolByName (troopData.bulletId);
                    bullet.transform.position = gunPoint.position;
                    bullet.ShootAt (targetToShoot, troopData.layerToIgnore);
                    BulletSound.Play();
                    yield return Helper.WaitFor (.1f);
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
            RotateWings (true);
        }
    }
}