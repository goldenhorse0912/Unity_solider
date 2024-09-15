using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    public sealed partial class PlayerController {
        bool _isDead;
        bool _targetFound;
        bool _targetIsSet;
        Transform _target;
        Coroutine _findTargetsWithDelay;
        HealthSystemComponent _hsc;
        List<Transform> _visibleTargets;

        public float AnimSpeedMultiplier { get; set; } = 1;

        [SerializeField]
        ParticleSystem smokeTrail;

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

        [SerializeField]
        float damage = 5f;

        void StartSearchingResourceObject () {
            if (_isDead) return;
            Anim.speed = AnimSpeedMultiplier;
            _findTargetsWithDelay = StartCoroutine (FindTargetsWithDelay ());
        }

        IEnumerator FindTargetsWithDelay () {
            while (true) {
                yield return Helper.WaitFor (.2f);
                FindVisibleTargets ();
            }
        }

        void FindVisibleTargets () {
            _visibleTargets.Clear ();
            var targetsInViewRadius = Physics.OverlapSphere (CachedTransform.position, viewRadius, targetMask);
            foreach (var col in targetsInViewRadius) {
                var target = col.transform;
                var dirToTarget = (target.position - CachedTransform.position).normalized;
                if (Vector3.Angle (CachedTransform.forward, dirToTarget) < viewAngle / 2) {
                    var dstToTarget = Vector3.Distance (CachedTransform.position, target.position);
                    if (!Physics.Raycast (CachedTransform.position, dirToTarget, dstToTarget, obstacleMask)) {
                        _visibleTargets.Add (target);
                    }
                }
            }
            _targetFound = _visibleTargets.Count > 0;
            if (_targetFound) {
                //sort target by distance only if found
                _visibleTargets = _visibleTargets.OrderBy (target => Vector3.Distance (target.position, CachedTransform.position)).ToList ();
                SetTarget ();
            } else {
                RemoveTarget ();
            }
        }

        void SetTarget () {
            if (_targetIsSet) return;
            _targetIsSet = true;
            _target = _visibleTargets.First ();
            /*if (_target.TryGetComponent (out Source source)) {
                CachedTransform.DOLookAt (_target.position, 0.3f, AxisConstraint.Y);
                _toolController.EquipToolByTypeOfResource (source.TypeOfResource);
                switch (source.TypeOfResource) {
                    case Resource.Wood or Resource.Wheat or Resource.Fig:
                        PlayAnim (AnimatorParams.IsChopping, true);
                        break;

                    case Resource.Emerald or Resource.CopperOre:
                        PlayAnim (AnimatorParams.IsMining, true);
                        break;
                }
            } else if (_target.TryGetComponent (out Enemy _)) {
                _weaponController.Equip ();
                PlayAnim (AnimatorParams.IsAttacking, true);
            }*/
        }

        internal void ResetTarget () {
            if (_targetIsSet) {
                _targetIsSet = false;
                _visibleTargets.Remove (_target);
            }
        }

        void StopSearchingResourceObject () {
            Anim.speed = 1f;
            //stop coroutine if exists
            if (_findTargetsWithDelay != null) {
                StopCoroutine (_findTargetsWithDelay);
            }
            //kill look at tween if running
            CachedTransform.DOKill ();

            //remove target
            RemoveTarget ();
        }

        void RemoveTarget () {
            _target = null;

            //reset other variables
            _targetFound = false;
            _targetIsSet = false;

            //unequip the tool
            // _toolController.UnEquip ();
            // _weaponController.UnEquip ();

            //stop animations
            PlayAnim (AnimatorParams.IsAttacking, false);
            PlayAnim (AnimatorParams.IsMining, false);
            PlayAnim (AnimatorParams.IsChopping, false);
        }

        //animation event
        public void OnCutSource () {
            // _toolController.PlaySlashEffect ();
            if (_targetIsSet) {
                Manager.Instance.AudioManager.PlayHaptic ();
                /*if (_target.TryGetComponent (out Source source)) {
                    if (source.IsAvailable ()) {
                        source.CutAndSpawn ();
                    } else {
                        ResetTarget ();
                    }
                }*/
            }
        }

        //animation event
        public void OnAttack () {
            // _weaponController.PlaySlashEffect ();
            if (_targetIsSet) {
                Manager.Instance.AudioManager.PlayHaptic ();
                if (_target.TryGetComponent (out HealthSystemComponent hsc)) {
                    if (hsc.GetHealthSystem ().IsDead ()) {
                        ResetTarget ();
                    } else {
                        hsc.GetHealthSystem ().Damage (damage);
                    }
                }
            }
        }

        public void OnBattleStart () {
            targetMask = Layers.Enemy.ToInt ();
            Enable ();
        }

        public void OnBattleEnd () {
            targetMask = Layers.Resource.ToInt ();
            Disable ();
        }

        void Dead (object sender, EventArgs e) {
            _isDead = true;
            PlayAnim (AnimatorParams.Dead);
            StopSearchingResourceObject ();
            RemoveTarget ();
            Disable ();
            // Manager.Instance.BattleManager.Result (false);
        }

        public void ActivateSwitch (Transform target) {
            CachedTransform.DOLookAt (target.position, .3f, AxisConstraint.Y)
                           .OnComplete (() => {
                                PlayAnim (AnimatorParams.Activate);
                            });
        }
    }
}