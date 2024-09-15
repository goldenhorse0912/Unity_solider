using System.Collections;
using CodeMonkey.HealthSystemCM;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    [RequireComponent (typeof (SimpleAgent))]
    public class FlyingVehicle : Troop {
        [SerializeField]
        internal Troop_SO troopData;
        [SerializeField, HideIf ("isPlayer")]
        int collectibleValue;
        [SerializeField]
        GameObject healthCanvas;
        [SerializeField, ReadOnly]
        internal HealthSystemComponent targetToShoot;

        SimpleAgent _agent;
        Destroyable _destroyable;
        bool _attackingEnemy, _searchingBuilding, _checkingAttackRadius, _attackingBuilding;
        HitFlash[] _hitFlashes;

        public GameObject target;
        
        /*#if UNITY_EDITOR
                void OnDrawGizmos () {
                    Gizmos.color = Color.red;
                    Gizmos.DrawWireSphere (transform.position, troopData.attackRadius);
                }
        #endif*/

        protected override void OnAwake () {
            base.OnAwake ();
            TryGetComponent (out _agent);
            _hitFlashes = GetComponentsInChildren<HitFlash> ();
            _destroyable = GetComponentInChildren<Destroyable> ();
        }

        protected override void OnDie () {
            base.OnDie ();
            _agent.Agent.enabled = false;
            if (!isPlayer) {
                ResourceManager.Instance.SpawnCoinAt (CachedTransform.position, collectibleValue);
            }
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
        }

        public void CheckEnemyHealth (bool isGunner = false) {
            if (targetToShoot && !targetToShoot.GetHealthSystem ().IsDead ()) {
                if (!isGunner) {
                    ShootEnemy ();
                }
                return;
            }
            targetToShoot = null;
            _attackingEnemy = false;
            _attackingBuilding = false;
            _checkingAttackRadius = false;
            StopShootingEnemy ();
            SearchEnemy ();
        }

        public void AttackBuilding () {
            if (Hsc.GetHealthSystem ().IsDead () || _attackingBuilding) return;
            var nearestBuilding = Manager.Instance.TroopManager.FindClosestTargetBuilding (CachedTransform, isPlayer);
            if (!nearestBuilding) return;
            target = nearestBuilding.gameObject;
            _agent.SetDestination (nearestBuilding.CachedTransform.position.With (y: CachedTransform.position.y));
            _attackingBuilding = true;
            StartCoroutine (CheckDistance (nearestBuilding.Hsc));
            StartCoroutine (CheckTroops ());
        }

        IEnumerator CheckDistance (HealthSystemComponent buildingHealth) {
            _searchingBuilding = true;
            while (_searchingBuilding && CachedTransform.DistanceTo (buildingHealth.transform) >= troopData.attackRadius) {
                if (buildingHealth.GetHealthSystem ().IsDead ()) {
                    _agent.StopHere ();
                    /*_attackingEnemy = false;
                    _attackingBuilding = false;
                    _checkingAttackRadius = false;*/
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
            while (_attackingBuilding) {
                yield return Helper.WaitFor (0.5f);
                if (Hsc.GetHealthSystem ().IsDead ()) {
                    yield break;
                }
                var closestEnemy = Manager.Instance.TroopManager.FindClosestTargetToMe (CachedTransform, isPlayer);
                if (closestEnemy) {
                    StopShootingEnemy ();
                    _searchingBuilding = false;
                    _attackingBuilding = false;
                    _attackingEnemy = true;
                    if (CachedTransform.position.DistanceTo (closestEnemy.CachedTransform.position.With (y: CachedTransform.position.y)) >= troopData.attackRadius) {
                        /*_agent.SetDestinationWithoutLookAt (closestEnemy.transform.position);
                    } else {*/
                        target = closestEnemy.gameObject;
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
                    _attackingEnemy = false;
                    _attackingBuilding = false;
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
            if (Hsc.GetHealthSystem ().IsDead () || /*_attackingBuilding ||*/ _attackingEnemy) {
                return;
            }
            var closestEnemy = Manager.Instance.TroopManager.FindClosestTargetToMe (CachedTransform, isPlayer);
            if (closestEnemy) {
                StopShootingEnemy ();
                _searchingBuilding = false;
                _attackingEnemy = true;
                if (CachedTransform.position.DistanceTo (closestEnemy.CachedTransform.position.With (y: CachedTransform.position.y)) >= troopData.attackRadius) {
                    target = closestEnemy.gameObject;
                    _agent.SetDestination (closestEnemy.transform.position);
                }
                StartCoroutine (CheckAttackRadius (closestEnemy.Hsc));
            } else {
                AttackBuilding ();
            }
        }
    }
}