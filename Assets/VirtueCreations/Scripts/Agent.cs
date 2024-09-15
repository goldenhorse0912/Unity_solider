using System;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace VIRTUE {
    [RequireComponent (typeof (NavMeshAgent))]
    public class Agent : CachedAnimator {
        float _stoppingDistance;
        UnityAction _onDestinationReached;
        public Vector3 Destination { get; private set; }
        protected NavMeshAgent NavMeshAgent { get; private set; }

        /*[SerializeField]
        float moveSpeed;*/

        protected override void OnAwake () {
            base.OnAwake ();
            ResetStoppingDistance ();
            NavMeshAgent = GetComponent<NavMeshAgent> ();
        }

        protected virtual void OnEnable () {
            NavMeshAgent.enabled = true;
        }

        void OnDisable () {
            NavMeshAgent.enabled = false;
        }

        protected void SetStoppingDistance (float distance) {
            _stoppingDistance = distance;
        }

        protected void ResetStoppingDistance () {
            SetStoppingDistance (.1f);
        }

        internal void LookForward () {
            CachedTransform.DORotateQuaternion (Quaternion.identity, .2f);
        }

        internal void SetAction (UnityAction onDestinationReached) {
            _onDestinationReached += onDestinationReached;
        }

        public void StopAndReset () {
            NavMeshAgent.ResetPath ();
            CachedTransform.DOKill ();
            // CachedTransform.rotation = Quaternion.identity;
            PlayAnim (AnimatorParams.IsMoving, false);
            // PlayAnim (AnimatorParams.Move, 0f);
            // enabled = false;
        }

        internal void SetDestination (Vector3 destination, UnityAction onDestinationReached = null) {
            NavMeshAgent.ResetPath ();
            CachedTransform.DOKill ();
            _onDestinationReached = onDestinationReached;
            PlayAnim (AnimatorParams.IsMoving, true);
            // PlayAnim (AnimatorParams.Move, moveSpeed);
            NavMesh.SamplePosition (destination, out var navMeshHit, 4f, NavMesh.AllAreas);
            Destination = navMeshHit.position;
            CalculateAndMoveAlongPathAsync (Destination);
        }

        internal void OnTouchPress (Vector2 touchPos) {
            NavMeshAgent.ResetPath (); // Stop the agent
            Ray ray = Helper.Camera.ScreenPointToRay (touchPos);
            if (Physics.Raycast (ray, out var hit)) {
                PlayAnim (AnimatorParams.IsMoving, true);
                // PlayAnim (AnimatorParams.Move, moveSpeed);
                CalculateAndMoveAlongPathAsync (hit.point);
            }
        }

        void OnReachedDestination () {
            PlayAnim (AnimatorParams.IsMoving, false);
            // PlayAnim (AnimatorParams.Move, 0f);
            _onDestinationReached?.Invoke ();
        }

        async void CalculateAndMoveAlongPathAsync (Vector3 destination) {
            NavMeshPath path = await CalculatePathAsync (destination);
            if (path.status == NavMeshPathStatus.PathComplete) {
                // Successfully calculated the path, move along it using DOPath
                Vector3[] pathPoints = path.corners;

                // Maintain stopping distance from last point
                if (_stoppingDistance > 0) {
                    switch (pathPoints.Length) {
                        case 1: {
                            var pointZ = pathPoints[^1];
                            pointZ += pointZ.DirectionTo (CachedTransform.position) * _stoppingDistance;
                            pathPoints[^1] = pointZ;
                        }
                            break;

                        default: {
                            var pointZ = pathPoints[^1];
                            pointZ += pointZ.DirectionTo (pathPoints[^2]) * _stoppingDistance;
                            pathPoints[^1] = pointZ;
                        }
                            break;
                    }
                }

                // Use DOPath to move along the calculated path with speed-based movement
                await MoveAlongPathAsync (pathPoints);
            } else {
                this.LogError ("Path calculation failed.");
            }
        }

        async Task<NavMeshPath> CalculatePathAsync (Vector3 targetPosition) {
            var path = new NavMeshPath ();
            var taskCompletionSource = new TaskCompletionSource<NavMeshPath> ();
            NavMeshAgent.CalculatePath (targetPosition, path);
            if (path.status == NavMeshPathStatus.PathComplete) {
                taskCompletionSource.SetResult (path);
            } else {
                taskCompletionSource.SetException (new Exception ("[VIRTUE]: Path calculation failed."));
            }
            return await taskCompletionSource.Task;
        }

        async Task MoveAlongPathAsync (Vector3[] pathPoints) {
            // Set up DOPath with speed-based movement
            await CachedTransform.DOPath (pathPoints, NavMeshAgent.speed)
                                 .SetSpeedBased ()
                                 .SetOptions (false, AxisConstraint.Y)
                                 .SetLookAt (.001f)
                                 .SetUpdate (UpdateType.Fixed)
                                 .OnComplete (OnReachedDestination)
                                 .AsyncWaitForCompletion ();

            // Stop the NavMeshAgent when done
            // NavMeshAgent.isStopped = true;
        }
    }
}