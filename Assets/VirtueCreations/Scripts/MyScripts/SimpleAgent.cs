using System;
using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

namespace VIRTUE {
    [RequireComponent (typeof (NavMeshAgent))]
    public class SimpleAgent : CachedAnimator {
        Action _onDestinationReached;
        internal NavMeshAgent Agent;

        [SerializeField, DisplayAsString]
        bool hasReachedDestination;

        protected override void OnAwake () {
            base.OnAwake ();
            Agent = GetComponent<NavMeshAgent> ();
        }

        internal void SetDestination (Vector3 destination, Action onDestinationReached = null) {
            _onDestinationReached = onDestinationReached;
            hasReachedDestination = false;
            LookTowards (destination);
            Agent.isStopped = false;
            Agent.SetDestination (destination);
            StartCoroutine (CheckRemainingDistance ());
        }
        
        internal void SetDestinationWithoutLookAt (Vector3 destination, Action onDestinationReached = null) {
            _onDestinationReached = onDestinationReached;
            hasReachedDestination = false;
            // LookTowards (destination);
            Agent.isStopped = false;
            Agent.SetDestination (destination);
            StartCoroutine (CheckRemainingDistance ());
        }

        internal void LookTowards (Vector3 destination) {
            CachedTransform.DOLookAt (destination, .5f, AxisConstraint.Y);
        }

        internal void LookTowards (Quaternion rotation) {
            CachedTransform.DORotateQuaternion (rotation, .5f);
        }

        internal void StopHere () {
            hasReachedDestination = true;
            if (Agent.enabled) {
                Agent.isStopped = true;
            }
        }

        IEnumerator CheckRemainingDistance () {
            PlayAnim (AnimatorParams.IsMoving, true);
            var wait = new WaitForSeconds (.1f);
            while (!hasReachedDestination) {
                if (!Agent.enabled) {
                    yield break;
                }
                if (Agent.isStopped) {
                    hasReachedDestination = true;
                    PlayAnim (AnimatorParams.IsMoving, false);
                    yield break;
                }
                if (!Agent.pathPending) {
                    if (Agent.remainingDistance <= Agent.stoppingDistance) {
                        if (!Agent.hasPath || Agent.velocity.sqrMagnitude == 0f) {
                            hasReachedDestination = true;
                            break;
                        }
                    }
                }
                yield return wait;
            }
            PlayAnim (AnimatorParams.IsMoving, false);
            _onDestinationReached?.Invoke ();
        }
    }
}