using System.Collections;
using CodeMonkey.HealthSystemCM;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

namespace VIRTUE {
    [RequireComponent (typeof (SimpleAgent))]
    public class Vehicle : Troop {
        [SerializeField]
        internal Troop_SO troopData;
        [SerializeField, HideIf ("isPlayer")]
        int collectibleValue;
        [SerializeField]
        GameObject healthCanvas;
        [SerializeField, ReadOnly]
        internal HealthSystemComponent targetToShoot;
        [SerializeField]
        AudioSource vehicleSound;

        GameObject _target;

        [SerializeField]
        RagDoll[] soldiers;

        SimpleAgent _agent;
        Destroyable _destroyable;
        bool _targetIsEnemy, _searchingBuilding, _checkingAttackRadius, _targetIsBuilding;
        HitFlash[] _hitFlashes;
        internal AudioSource BulletSound;

        [SerializeField]
        VoidGameEvent victory, defeat;

        /*#if UNITY_EDITOR
                void OnDrawGizmos () {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere (transform.position, troopData.attackRadius);
                }
        #endif*/

        void OnEnable () {
            victory.OnEvent += vehicleSound.Stop ;;
            defeat.OnEvent += vehicleSound.Stop ;;
        }

        void OnDisable () {
            victory.OnEvent -= vehicleSound.Stop;
            defeat.OnEvent -= vehicleSound.Stop;
        }
        
        protected override void OnAwake () {
            base.OnAwake ();
            TryGetComponent (out _agent);
            TryGetComponent (out BulletSound);
            _hitFlashes = GetComponentsInChildren<HitFlash> ();
            _destroyable = GetComponentInChildren<Destroyable> ();
        }

        /*protected override void OnStart () {
            base.OnStart ();
            SearchEnemy ();
        }*/

        protected override void OnDie () {
            base.OnDie ();
            _agent.Agent.enabled = false;
            if (!isPlayer) {
                ResourceManager.Instance.SpawnCoinAt (CachedTransform.position, collectibleValue);
            }
            vehicleSound.Stop ();
            healthCanvas.Hide ();
            GetDestroyed ();
        }

        protected override void OnDamage () {
            base.OnDamage ();
            foreach (var flash in _hitFlashes) {
                flash.Flash ();
            }
        }

        protected virtual void ShootEnemy () { }

        protected virtual void StopShootingEnemy () { }

        protected virtual void GetDestroyed () {
            _destroyable.GetDestroyed (gameObject, 3f, false);
            foreach (var soldier in soldiers) {
                soldier.ToggleRagDoll (true);
                soldier.GetThrown ();
            }
        }

        public void CheckEnemyHealth (bool isGunner = false) {
            if (targetToShoot && !targetToShoot.GetHealthSystem ().IsDead ()) {
                if (!isGunner) {
                    ShootEnemy ();
                }
                return;
            }
            targetToShoot = null;
            _targetIsEnemy = false;
            _targetIsBuilding = false;
            _checkingAttackRadius = false;
            StopShootingEnemy ();
            SearchEnemy ();
        }

        public void AttackBuilding () {
            if (Hsc.GetHealthSystem ().IsDead () /*|| _targetIsBuilding*/) return;
            var nearestBuilding = Manager.Instance.TroopManager.FindClosestTargetBuilding (CachedTransform, isPlayer);
            if (!nearestBuilding || _target == nearestBuilding.gameObject) return;
            _target = nearestBuilding.gameObject;
            _agent.SetDestination (nearestBuilding.CachedTransform.position.With (y: CachedTransform.position.y));
            _targetIsBuilding = true;
            StartCoroutine (CheckDistance (nearestBuilding.Hsc));
            StartCoroutine (CheckTroops ());
        }

        IEnumerator CheckDistance (HealthSystemComponent buildingHealth) {
            _searchingBuilding = true;
            while (CachedTransform.DistanceTo (buildingHealth.transform) >= troopData.attackRadius && _searchingBuilding) {
                if (buildingHealth.GetHealthSystem ().IsDead ()) {
                    _agent.StopHere ();
                    _targetIsBuilding = false;
                    SearchEnemy ();
                    yield break;
                }
                yield return Helper.WaitFor (0.2f);
            }
            if (_searchingBuilding) {
                _searchingBuilding = false;
                _agent.StopHere ();
                targetToShoot = buildingHealth;
                ShootEnemy ();
            }
        }

        IEnumerator CheckTroops () {
            while (_targetIsBuilding) {
                yield return Helper.WaitFor (0.5f);
                if (Hsc.GetHealthSystem ().IsDead ()) {
                    yield break;
                }
                var closestEnemy = Manager.Instance.TroopManager.FindClosestTargetToMe (CachedTransform, isPlayer);
                if (closestEnemy) {
                    StopShootingEnemy ();
                    _searchingBuilding = false;
                    _targetIsBuilding = false;
                    _targetIsEnemy = true;
                    if (CachedTransform.DistanceTo (closestEnemy.CachedTransform) >= troopData.attackRadius) {
                        _target = closestEnemy.gameObject;
                        _agent.SetDestination (closestEnemy.transform.position.With (y: CachedTransform.position.y));
                    }
                    StartCoroutine (CheckAttackRadius (closestEnemy.Hsc));
                }
            }
        }

        IEnumerator CheckAttackRadius (HealthSystemComponent enemyHealth) {
            if (_checkingAttackRadius) {
                yield break;
            }
            _checkingAttackRadius = true;
            var wait = new WaitForSeconds (.1f);
            var targets = Physics.OverlapSphere (CachedTransform.position, troopData.attackRadius, troopData.visibleLayer);
            while (targets.Length <= 0) {
                if (enemyHealth.GetHealthSystem ().IsDead ()) {
                    _targetIsEnemy = false;
                    _targetIsBuilding = false;
                    _checkingAttackRadius = false;
                    SearchEnemy ();
                    yield break;
                }
                yield return wait;
                targets = Physics.OverlapSphere (CachedTransform.position, troopData.attackRadius, troopData.visibleLayer);
            }
            _checkingAttackRadius = false;
            if (targets.FindNearest (CachedTransform).TryGetComponent (out targetToShoot)) {
                _agent.StopHere ();
                ShootEnemy ();
            }
        }

        public void SearchEnemy () {
            if (Hsc.GetHealthSystem ().IsDead () || _targetIsEnemy) {
                return;
            }
            var closestEnemy = Manager.Instance.TroopManager.FindClosestTargetToMe (CachedTransform, isPlayer);
            if (closestEnemy) {
                StopShootingEnemy ();
                _searchingBuilding = false;
                _targetIsEnemy = true;
                if (CachedTransform.DistanceTo (closestEnemy.CachedTransform) >= troopData.attackRadius) {
                    _target = closestEnemy.gameObject;
                    _agent.SetDestination (closestEnemy.transform.position.With (y: CachedTransform.position.y));
                }
                StartCoroutine (CheckAttackRadius (closestEnemy.Hsc));
            } else {
                AttackBuilding ();
            }
        }
    }
}