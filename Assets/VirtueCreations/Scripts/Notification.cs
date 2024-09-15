using DG.Tweening;
using TMPro;
using UnityEngine;

namespace VIRTUE {
    public class Notification : PooledObject {
        TextMeshProUGUI _text;

        protected override void OnAwake () {
            base.OnAwake ();
            _text = GetComponentInChildren<TextMeshProUGUI> ();
        }

        public override void OnGetFromPool () {
            CachedGameObject.Show ();
        }

        public override void OnReleaseToPool () {
            CachedGameObject.Hide ();
        }

        protected override void Release () {
            Manager.Instance.NotificationManager.ReturnToPool (this);
        }

        public void Show (string message) {
            _text.text = message;
            DOTween.Sequence ()
                   .Append (((RectTransform)CachedTransform).DOSizeDelta (new Vector2 (768, 100), 1f)
                                                            .From (new Vector2 (0, 100))
                                                            .SetEase (Ease.OutBack))
                   .Append (CachedTransform.DOLocalMoveY (512f, 1f)
                                           .From (0)
                                           .SetEase (Ease.InOutQuad))
                   .AppendInterval(.5f)
                   .AppendCallback (Release);
        }
    }
}