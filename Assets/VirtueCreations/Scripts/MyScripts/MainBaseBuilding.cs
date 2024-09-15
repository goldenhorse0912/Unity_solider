using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class MainBaseBuilding : Building {
        [SerializeField]
        DOTweenAnimation antennaAnimation;

        protected override void OnStart () {
            base.OnStart ();
            if (isPlayer) {
                Manager.Instance.TroopManager.PlayerBase = this;
            } else {
                Manager.Instance.TroopManager.EnemyBase = this;
            }
        }

        protected override void OnDie () {
            base.OnDie ();
            if (antennaAnimation) {
                antennaAnimation.DOKill ();
            }
        }
    }
}