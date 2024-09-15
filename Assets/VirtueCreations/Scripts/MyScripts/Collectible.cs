using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class Collectible : PooledObject {
        internal void JumpAt (Vector3 pos, int collectibleValue) {
            CachedTransform.DORotate (CachedTransform.eulerAngles + Vector3.right * 180f, 1 / 5f).SetUpdate (true).SetLoops (5);
            CachedTransform.DOJump (pos, 4, 1, 1f)/*.SetUpdate (true)*/.OnComplete (() => {
                DOVirtual.DelayedCall (.3f, () => {
                    ResourceManager.Instance.SpawnCoinUiAt (CachedTransform, collectibleValue);
                    Release ();
                });
            });
        }

        public override void OnGetFromPool () {
            CachedGameObject.Show ();
        }

        public override void OnReleaseToPool () {
            CachedGameObject.Hide ();
        }
    }
}