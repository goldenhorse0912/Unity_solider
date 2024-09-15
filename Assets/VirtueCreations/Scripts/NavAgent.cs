using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace VIRTUE {
    public abstract class NavAgent : CachedAnimator {
        public enum State {
            Idle,
            SearchTarget,
            ChaseTarget,
            AttackTarget,
            Taunt
        }

        public event EventHandler OnDead;

        bool _isMoving;
        bool _targetFound;
        bool _playerFound;
        protected bool TargetIsSet;
        protected Transform Target;
        Coroutine _findTargetsWithDelay;
        protected List<Transform> VisibleTargets;
        HealthSystemComponent _hsc;
        protected NavMeshAgent Agent;

        [FoldoutGroup ("Field Of View")]
        [SerializeField]
        float viewRadius;

        [FoldoutGroup ("Field Of View")]
        [Range (0, 360)]
        [SerializeField]
        float viewAngle;

        [FoldoutGroup ("Field Of View")]
        [SerializeField]
        LayerMask targetMask;

        [FoldoutGroup ("Field Of View")]
        [SerializeField]
        LayerMask obstacleMask;

        [Tooltip ("Current State of the Enemy")]
        [DisplayAsString]
        [SerializeField]
        protected State currentState;

        [SerializeField]
        float rotationDamping = 10f;

        [SerializeField]
        protected float damage = 5f;

        [SerializeField]
        protected float attackDelay = 1f;

        [SerializeField]
        protected float stoppingDistance = 1f;

        float _initialStoppingDistance;
        // WeaponController _weaponController;
        // HitFlash[] _hitFlashes;

        protected override void OnAwake () {
            base.OnAwake ();
            VisibleTargets = new List<Transform> ();
            Agent = GetComponent<NavMeshAgent> ();
            // _weaponController = GetComponent<WeaponController> ();
            currentState = State.Idle;
            _initialStoppingDistance = stoppingDistance;
            // _hitFlashes = GetComponentsInChildren<HitFlash> ();
        }

        void Start () {
            _hsc = GetComponent<HealthSystemComponent> ();
            _hsc.GetHealthSystem ().OnDead += Dead;
            /*foreach (var flash in _hitFlashes) {
                _hsc.GetHealthSystem ().OnDamaged += flash.Flash;
            }*/
        }

        void OnEnable () {
            Agent.enabled = true;
        }

        void OnDisable () {
            Agent.enabled = false;
        }

        void Update () {
            switch (currentState) {
                case State.Idle:
                    State_Idle ();
                    break;

                case State.SearchTarget:
                    //do nothing because searching in progress
                    break;

                case State.ChaseTarget:
                    State_ChaseTarget ();
                    break;

                case State.AttackTarget:
                    State_AttackTarget ();
                    break;

                case State.Taunt:
                    // State_Taunt ();
                    break;
            }
        }

        protected void ResetStoppingDistance () {
            stoppingDistance = _initialStoppingDistance;
        }

        protected virtual void State_Idle () {
            if (_findTargetsWithDelay != null) {
                StopCoroutine (_findTargetsWithDelay);
            }
            _findTargetsWithDelay = StartCoroutine (FindTargetsWithDelay ());
            currentState = State.SearchTarget;
        }

        void State_ChaseTarget () {
            /*if (!_targetIsSet) {
                CurrentState = State.Idle;
                return;
            }*/
            if (Target.CompareTag (Tags.DEAD)) {
                RemoveTarget ();
                return;
            }
            var distToTarget = CachedTransform.DistanceTo (Target);
            if (distToTarget < stoppingDistance) {
                currentState = State.AttackTarget;
                Move (false);
                PlayAnim (AnimatorParams.Attack, Random.Range (1, 3));
                DOVirtual.DelayedCall (attackDelay, () => {
                              currentState = State.ChaseTarget;
                          }, false)
                         .SetId (GetHashCode ());
            } else {
                Move (true);
                Agent.SetDestination (Target.position);
            }
        }

        void State_AttackTarget () {
            LookAtTarget ();
        }

        protected void Move (bool state) {
            if (_isMoving == state) return;
            _isMoving = state;
            PlayAnim (AnimatorParams.IsMoving, state);
            PlayAnim (AnimatorParams.Move, state ? 1f : 0f);
            Agent.ResetPath ();
        }

        void LookAtTarget () {
            if (!TargetIsSet) {
                return;
            }
            var dir = Target.position - CachedTransform.position;
            if (dir != Vector3.zero) {
                var lookRotation = Quaternion.LookRotation (dir);
                var rotation = Quaternion.Lerp (CachedTransform.rotation, lookRotation, rotationDamping * Time.deltaTime).eulerAngles;
                CachedTransform.rotation = Quaternion.Euler (0f, rotation.y, 0f);
            }
        }

        void Dead (object sender, EventArgs eventArgs) {
            _hsc.GetHealthSystem ().OnDead -= Dead;
            OnDead?.Invoke (this, EventArgs.Empty);
            CachedGameObject.tag = Tags.DEAD;
            CachedGameObject.layer = LayerMask.NameToLayer ("Default");
            enabled = false;
            DOTween.Kill (GetHashCode ());
            if (_findTargetsWithDelay != null) {
                StopCoroutine (_findTargetsWithDelay);
            }
            Rebind ();
            PlayAnim (AnimatorParams.Dead);
            CachedTransform.DOMoveY (-2, 2f)
                           .SetDelay (2f)
                           .OnComplete (CachedGameObject.Hide);
        }

        IEnumerator FindTargetsWithDelay () {
            while (true) {
                yield return Helper.WaitFor (.2f);
                FindVisibleTargets ();
            }
        }

        void FindVisibleTargets () {
            VisibleTargets.Clear ();
            var targetsInViewRadius = Physics.OverlapSphere (CachedTransform.position, viewRadius, targetMask);
            foreach (var col in targetsInViewRadius) {
                var target = col.transform;
                var dirToTarget = (target.position - CachedTransform.position).normalized;
                if (Vector3.Angle (CachedTransform.forward, dirToTarget) < viewAngle / 2) {
                    var dstToTarget = Vector3.Distance (CachedTransform.position, target.position);
                    if (!Physics.Raycast (CachedTransform.position, dirToTarget, dstToTarget, obstacleMask)) {
                        VisibleTargets.Add (target);
                    }
                }
            }
            _targetFound = VisibleTargets.Count > 0;
            if (_targetFound) {
                //sort target by distance only if found
                VisibleTargets = VisibleTargets.OrderBy (target => Vector3.Distance (target.position, CachedTransform.position)).ToList ();
                SetTarget ();
            } else {
                RemoveTarget ();
            }
        }

        protected Transform GetRandomTarget () => VisibleTargets.FindAll (x => x != Manager.Instance.PlayerController.CachedTransform).Random ();

        protected virtual void SetTarget () {
            if (TargetIsSet) {
                if (Target == Manager.Instance.PlayerController.CachedTransform && VisibleTargets.Count > 1) {
                    Target = GetRandomTarget ();
                }
            } else {
                TargetIsSet = true;
                if (VisibleTargets.Contains (Manager.Instance.PlayerController.CachedTransform)) {
                    Target = VisibleTargets.Count switch {
                        1 => VisibleTargets.First (),
                        _ => GetRandomTarget ()
                    };
                } else {
                    Target = VisibleTargets.Random ();
                }
            }
            // StopCoroutine (_findTargetsWithDelay);
            if (currentState == State.SearchTarget) {
                currentState = State.ChaseTarget;
            }
        }

        void RemoveTarget () {
            Target = null;

            //reset other variables
            _targetFound = false;
            TargetIsSet = false;

            //stop animations
            PlayAnim (AnimatorParams.Attack, 0);
            DOTween.Kill (GetHashCode ());
            currentState = State.Idle;
            OnRemoveTarget ();
        }

        protected virtual void OnRemoveTarget () { }

        public virtual void StartSearchingEnemies () {
            enabled = true;
        }

        public void OnResult (bool state) {
            if (enabled) {
                enabled = false;
                if (state) { } else {
                    DOTween.Kill (GetHashCode ());
                    PlayAnim (AnimatorParams.Victory);
                }
            }
        }

        //animation event
        public void OnAttack () {
            // _weaponController.PlaySlashEffect ();
            if (_hsc.GetHealthSystem ().IsDead ()) {
                return;
            }
            if (TargetIsSet) {
                if (Target.TryGetComponent (out HealthSystemComponent hsc)) {
                    hsc.GetHealthSystem ().Damage (damage);
                }
            }
            PlayAnim (AnimatorParams.Attack, 0);
        }
    }
}