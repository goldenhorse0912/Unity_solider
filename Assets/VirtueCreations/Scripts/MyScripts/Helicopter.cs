using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class Helicopter : Vehicle {
        Coroutine _attackingBuildingCoroutine;
        [SerializeField]
        DOTweenAnimation[] propellerAnimation;
        /*[SerializeField]
        RagDoll[] soldiers;*/
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
                gfx.DOLocalRotate (Vector3.right * 15f, 3f);
                SearchEnemy ();
            });
        }

        protected override void OnDie () {
            base.OnDie ();
            foreach (var doTweenAnimation in propellerAnimation) {
                doTweenAnimation.DOKill ();
            }
            /*foreach (var soldier in soldiers) {
                soldier.ToggleRagDoll (true);
                soldier.GetThrown ();
            }*/
        }

        protected override void ShootEnemy () {
            if (Hsc.GetHealthSystem ().IsDead ()) {
                return;
            }
            gun.DOLookAt (targetToShoot.transform.position, .01f).OnComplete (() => {
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
        }
    }
}