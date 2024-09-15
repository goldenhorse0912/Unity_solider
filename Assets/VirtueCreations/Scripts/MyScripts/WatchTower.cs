using CodeMonkey.HealthSystemCM;
using UnityEngine;

namespace VIRTUE {
    public class WatchTower : Building {
        FieldOfView _fov;
        WatchTowerTroop _myTroop;
        RagDoll _ragDoll;

        protected override void OnAwake () {
            base.OnAwake ();
            TryGetComponent (out _fov);
            _ragDoll = GetComponentInChildren<RagDoll> ();
            _myTroop = GetComponentInChildren<WatchTowerTroop> ();
            _myTroop.TowerHealth = Hsc;
        }

        protected override void OnDie () {
            base.OnDie ();
            _fov.enabled = false;
            if (isPlayer) {
                Manager.Instance.BuildingManager.WatchTowerDead ();
            }
            _ragDoll.ToggleRagDoll (true);
            _ragDoll.GetThrown ();
            // _myTroop.gameObject.Hide ();
        }

        void Update () {
            if (!_fov.enabled) return;
            if (_fov.isFound && !_myTroop.targetToShoot) {
                AttackEnemy ();
            }
        }

        void AttackEnemy () {
            var enemy = _fov.target.FindNearest (CachedTransform).GetComponent<HealthSystemComponent> ();
            _myTroop.StartAttack (enemy);
        }
    }
}