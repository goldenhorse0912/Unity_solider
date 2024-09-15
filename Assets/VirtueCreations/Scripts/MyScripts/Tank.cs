using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class Tank : Vehicle {
        Coroutine _attackingBuildingCoroutine;

        [SerializeField]
        Transform gun, gunPoint;
        [SerializeField]
        ParticleSystem muzzleFlash;
        [SerializeField]
        float intervalAfterShot = 1f;
        
        protected override void OnStart () {
            base.OnStart ();
            SearchEnemy ();
        }

        protected override void ShootEnemy () {
            if (Hsc.GetHealthSystem ().IsDead ()) {
                return;
            }
            gun.DOKill ();
            gun.DOLookAt (targetToShoot.transform.position.With (y: gun.position.y), .5f).OnComplete (() => {
                _attackingBuildingCoroutine = StartCoroutine (Shoot ());
            });

            IEnumerator Shoot () {
                if (targetToShoot) {
                    muzzleFlash.Play ();
                    var missile = (Missile)Manager.Instance.PoolManager.GetFromPoolByName (troopData.bulletId);
                    missile.transform.position = gunPoint.position;
                    missile.ShootAt (targetToShoot, troopData.layerToIgnore);
                    BulletSound.Play();
                    yield return Helper.WaitFor (intervalAfterShot);
                    CheckEnemyHealth ();
                }
            }
        }

        protected override void StopShootingEnemy () {
            if (_attackingBuildingCoroutine != null) {
                StopCoroutine (_attackingBuildingCoroutine);
                _attackingBuildingCoroutine = null;
            }
            // targetToShoot = null;
            gun.DOLocalRotate (Vector3.zero, .5f);
        }
    }
}