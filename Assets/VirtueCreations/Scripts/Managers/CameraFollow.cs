using DG.Tweening;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEditor;

namespace VIRTUE {
    public sealed partial class CameraFollow : CachedMonoBehaviour {
        float _turnSmoothVelocity;
        Vector3 _desiredPosition;
        Vector3 _followSmoothVelocity;

        [SerializeField]
        Transform target;

        [SerializeField]
        bool useSmooth;

        [ShowIf ("@useSmooth==true")]
        [Range (0f, 1f)]
        [SerializeField]
        float followSmoothTime = 0.1f;

        [ShowIf ("@useSmooth==true")]
        [Range (0f, 1f)]
        [SerializeField]
        float turnSmoothTime = 0.1f;

        [SerializeField]
        Vector3 followOffset = new(0f, 10f, -10f);

        [SerializeField]
        Vector3 lookAtOffset = new(40f, 0f, 0f);

#if UNITY_EDITOR
        void OnDrawGizmos () {
            if (EditorApplication.isPlaying) return;
            if (target == null) return;
            transform.SetPositionAndRotation (GetDesiredPosition (), Quaternion.Euler (lookAtOffset));
        }
#endif

        void FixedUpdate () {
            Follow ();
        }

#region PRIVATE_CALLBACKS

        void Follow () {
            if (useSmooth) {
                var targetPosition = Vector3.SmoothDamp (CachedTransform.position, GetDesiredPosition (), ref _followSmoothVelocity, followSmoothTime);
                var targetRotation = Quaternion.Slerp (CachedTransform.rotation, GetDesiredRotation (), Time.deltaTime * turnSmoothTime * 10f * Time.deltaTime);
                CachedTransform.SetPositionAndRotation (targetPosition, targetRotation);
            } else {
                CachedTransform.SetPositionAndRotation (GetDesiredPosition (), GetDesiredRotation ());
            }
        }

        Vector3 GetDesiredPosition () {
            _desiredPosition = target.position;
            _desiredPosition.x += followOffset.x;
            _desiredPosition.y += followOffset.y;
            _desiredPosition.z += followOffset.z;
            return _desiredPosition;
        }

        Quaternion GetDesiredRotation () => Quaternion.Euler (lookAtOffset);

        void ToggleScript (bool state) => enabled = state;

        void Enable () => ToggleScript (true);

        void Disable () => ToggleScript (false);

#endregion

        public void ToTarget (Transform newTarget) {
            Disable ();
            Manager.Instance.PlayerController.Disable ();
            SetTarget (newTarget);
            DOTween.Sequence ()
                   .AppendInterval (.5f)
                   .Append (CachedTransform.DOMove (GetDesiredPosition (), .75f))
                   .AppendInterval (1f)
                   .AppendCallback (ResetTarget)
                   .AppendInterval (1f)
                   .AppendCallback (EnableComponents);
        }
    }
}