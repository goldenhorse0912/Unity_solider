using System.Collections;
using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Animations.Rigging;

namespace VIRTUE {
    [RequireComponent (typeof (SimpleAgent))]
    public class WalkingTroop : Troop {
        [SerializeField]
        Troop_SO troopData;
        [SerializeField, HideIf ("isPlayer")]
        int collectibleValue;
        [SerializeField]
        Transform gunPoint, aimTargetPoint;
        [SerializeField, ReadOnly]
        HealthSystemComponent targetToShoot;

        Vector3 _defaultAimPos;
        Rig _rig;
        SimpleAgent _agent;
        RagDoll _ragDoll;
        bool _attackingEnemy, _searchingBuilding, _checkingAttackRadius, _targetIsBuilding;
        AudioSource _bulletSound;
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
            TryGetComponent (out _ragDoll);
            TryGetComponent (out _bulletSound);
            _hitFlashes = GetComponentsInChildren<HitFlash> ();
            _rig = GetComponentInChildren<Rig> ();
            _defaultAimPos = aimTargetPoint.localPosition;
        }

        protected override void OnStart () {
            base.OnStart ();
            StartAiming (false);
            SearchEnemy ();
        }

        void StartAiming (bool state) {
            if (state) {
                DOTween.To (x => _rig.weight = x, _rig.weight, 1f, .01f);
            } else {
                DOTween.To (x => _rig.weight = x, _rig.weight, 0f, .01f);
            }
        }

        /// <summary>
        /// called when no enemy/building available
        /// </summary>
        void ResetAiming () {
            aimTargetPoint.localPosition = _defaultAimPos;
        }

        protected override void OnDie () {
            base.OnDie ();
            _agent.Agent.enabled = false;
            _ragDoll.ToggleRagDoll (true);
            _ragDoll.GetThrown ();
            if (!isPlayer) {
                ResourceManager.Instance.SpawnCoinAt (CachedTransform.position, collectibleValue);
            }
        }

        protected override void OnDamage () {
            base.OnDamage ();
            foreach (var flash in _hitFlashes) {
                flash.Flash ();
            }
        }

#region Animation Callback

        //Animation Callback
        public void AttackEnemy () {
            if (!targetToShoot || Hsc.GetHealthSystem ().IsDead ()) return;
            var point = targetToShoot.transform.position + Vector3.up;
            CachedTransform.DOLookAt (point.With (y: CachedTransform.position.y), .1f).OnComplete (() => {
                aimTargetPoint.position = point;
                var bullet = (Bullet)Manager.Instance.PoolManager.GetFromPoolByName (troopData.bulletId);
                bullet.transform.position = gunPoint.position;
                bullet.ShootAt (targetToShoot, troopData.layerToIgnore);
                _bulletSound.Play ();
            });
        }

        //Animation Callback
        public void CheckEnemyHealth () {
            if (targetToShoot && !targetToShoot.GetHealthSystem ().IsDead ()) return;
            targetToShoot = null;
            _attackingEnemy = false;
            _checkingAttackRadius = false;
            ResetAiming ();
            StopAttack ();
            SearchEnemy ();
        }

#endregion

        void StopAttack () {
            _agent.PlayAnim (AnimatorParams.Shoot, false);
        }

        public void AttackBuilding () {
            if (Hsc.GetHealthSystem ().IsDead ()) return;
            var nearestBuilding = Manager.Instance.TroopManager.FindClosestTargetBuilding (CachedTransform, isPlayer);
            if (!nearestBuilding) return;
            StartAiming (false);
            target = nearestBuilding.gameObject;
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
                    SearchEnemy ();
                    yield break;
                }
                yield return Helper.WaitFor (0.2f);
            }
            if (_searchingBuilding) {
                _searchingBuilding = false;
                _agent.StopHere ();
                targetToShoot = buildingHealth;
                StartAiming (true);
                _agent.PlayAnim (AnimatorParams.Shoot, true);
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
                    _searchingBuilding = false;
                    _targetIsBuilding = false;
                    StopAttack ();
                    _attackingEnemy = true;
                    StartAiming (false);
                    target = closestEnemy.gameObject;
                    _agent.SetDestination (closestEnemy.transform.position.With (y: CachedTransform.position.y));
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
                StartAiming (true);
                _agent.PlayAnim (AnimatorParams.Shoot, true);
            }
        }

        public void SearchEnemy () {
            if (Hsc.GetHealthSystem ().IsDead () || _attackingEnemy) {
                return;
            }
            AttackBuilding ();
        }
    }
}