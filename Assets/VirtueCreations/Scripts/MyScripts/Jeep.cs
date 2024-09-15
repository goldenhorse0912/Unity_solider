using DG.Tweening;
using UnityEngine;
using System.Collections;

namespace VIRTUE {
    public class Jeep : Vehicle {
        Coroutine _attackingBuildingCoroutine;

        [SerializeField]
        Transform gun, gunPoint;
        [SerializeField]
        ParticleSystem muzzleFlash;
        [SerializeField]
        int noOfBulletsToShootAtATime = 5;
        [SerializeField]
        float gapWithinBullets = .5f;
        
        protected override void OnStart () {
            base.OnStart ();
            SearchEnemy ();
        }

        protected override void ShootEnemy () {
            if (Hsc.GetHealthSystem ().IsDead ()) {
                return;
            }
            gun.DOLookAt (targetToShoot.transform.position /*.With (y: gun.position.y)*/, .01f).OnComplete (() => {
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
            // targetToShoot = null;
            gun.DOLocalRotate (Vector3.zero, .3f);
        }
    }
}