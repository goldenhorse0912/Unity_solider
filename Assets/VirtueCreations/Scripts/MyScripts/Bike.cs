using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class Bike : Vehicle {
        [SerializeField]
        Transform gunPoint, gunner;

        Animator _anim;

        protected override void OnAwake () {
            base.OnAwake ();
            _anim = gunner.GetComponentInChildren<Animator> ();
        }
        
        protected override void OnStart () {
            base.OnStart ();
            SearchEnemy ();
        }

        protected override void ShootEnemy () {
            if (Hsc.GetHealthSystem ().IsDead ()) {
                return;
            }
            gunner.DOLookAt (targetToShoot.transform.position.With (y: gunner.position.y), .2f);
            _anim.SetBool (AnimatorParams.Shoot, true);
        }

        protected override void StopShootingEnemy () {
            _anim.SetBool (AnimatorParams.Shoot, false);
            targetToShoot = null;
        }

        //Animation callback
        public void FireBullets () {
            for (var i = 0; i < 5; i++) {
                var bullet = (Bullet)Manager.Instance.PoolManager.GetFromPoolByName (troopData.bulletId);
                bullet.transform.position = gunPoint.position;
                bullet.ShootInRadius (targetToShoot, troopData.layerToIgnore);
                BulletSound.Play();
            }
        }
    }
}