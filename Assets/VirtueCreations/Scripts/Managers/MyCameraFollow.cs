using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public sealed partial class CameraFollow {
        [SerializeField]
        VoidGameEvent enterBattle;

        Vector3 GetterFollowOffset () => followOffset;
        void SetterFollowOffset (Vector3 x) => followOffset = x;

        Vector3 GetterLookAtOffset () => lookAtOffset;
        void SetterLookAtOffset (Vector3 x) => lookAtOffset = x;

        void ResetTarget () {
            SetTarget (Manager.Instance.PlayerController.CachedTransform);
            CachedTransform.DOMove (GetDesiredPosition (), 1f);
        }

        void EnableComponents () {
            Enable ();
            Manager.Instance.PlayerController.Enable ();
        }

        void SetTarget (Transform newTarget) {
            target = newTarget;
        }

        internal void ToGamePlayScene () {
            Disable ();
            SetterFollowOffset (Vector3.zero.With (y: 12, z: -15.5f));
            SetterLookAtOffset (Vector3.zero.With (x: 40));
            CachedTransform.SetPositionAndRotation (GetDesiredPosition (), GetDesiredRotation ());
            Enable ();
        }

        internal void ToPyramid (Transform newTarget) {
            Disable ();
            SetTarget (newTarget);
            var targetPosition = target.position;
            targetPosition.y = 0f;
            targetPosition.z -= 1f;
            DOTween.Sequence ()
                   .Append (CachedTransform.DOMove (targetPosition, .5f))
                   .Join (CachedTransform.DORotateQuaternion (Quaternion.identity, .5f))
                   .AppendCallback (enterBattle.Raise);
        }

        internal void ToBattleScene () {
            SetTarget (Manager.Instance.PlayerController.CachedTransform);
            SetterFollowOffset (Vector3.zero.With (y: 3, z: -7));
            SetterLookAtOffset (Vector3.zero.With (x: 31));
            DOTween.Sequence ()
                   .Append (CachedTransform.DOMove (GetDesiredPosition (), .5f))
                   .Join (CachedTransform.DORotateQuaternion (GetDesiredRotation (), .5f))
                   .AppendCallback (Manager.Instance.FadeScreen.FadeIn)
                   .AppendCallback (Enable);
        }
    }
}