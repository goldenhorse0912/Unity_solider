using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public class VictoryPanel : ResultPanel {
        [SerializeField]
        Image imageEffect;

        [SerializeField]
        Image imageGlow;

        [SerializeField]
        Image decorLeftExtra;

        [SerializeField]
        Image decorRightExtra;

        protected override void Init () {
            imageEffect.enabled = false;
            imageGlow.enabled = false;
            decorLeft.rectTransform.localPosition = Vector3.zero.With (y: 195);
            decorRight.rectTransform.localPosition = Vector3.zero.With (y: 195);
            decorLeftExtra.enabled = false;
            decorLeftExtra.rectTransform.localPosition = Vector3.zero.With (y: 86);
            decorRightExtra.enabled = false;
            decorRightExtra.rectTransform.localPosition = Vector3.zero.With (y: 86);
        }

        protected override void PlayAnim () {
            DOTween.Sequence ()
                   .Append (blackBg.DOFade (.98f, .5f))
                   .Append (headerImg.rectTransform.DOScale (1f, .5f))
                   .Append (bgImg.DOFillAmount (1f, .5f))
                   .AppendCallback (() => {
                        decorLeft.enabled = true;
                        decorRight.enabled = true;
                        decorLeftExtra.enabled = true;
                        decorRightExtra.enabled = true;
                    })
                   .AppendCallback (() => {
                        head.enabled = true;
                    })
                   .AppendCallback (DoImageEffect)
                   .AppendCallback (() => CountEarnedCurrency (true))
                   .Append (head.rectTransform.DOLocalMoveY (430, .5f).SetEase (Ease.OutBack))
                   .Join (decorLeft.rectTransform.DOLocalMove (Vector3.zero.With (x: -231, y: 426), .5f).SetEase (Ease.OutBack))
                   .Join (decorRight.rectTransform.DOLocalMove (Vector3.zero.With (x: 231, y: 426), .5f).SetEase (Ease.OutBack))
                   .Append (decorLeftExtra.rectTransform.DOLocalMove (Vector3.zero.With (x: -301, y: 387), .5f).SetEase (Ease.OutBack))
                   .Join (decorRightExtra.rectTransform.DOLocalMove (Vector3.zero.With (x: 301, y: 387), .5f).SetEase (Ease.OutBack))
                   .AppendInterval (1f)
                   .AppendCallback (() => {
                        resultBtn.gameObject.Show ();
                        resultBtn.transform.DOScale (1f, .5f)
                                 .SetEase (Ease.OutBack);
                    });
        }

        void DoImageEffect () {
            imageEffect.enabled = true;
            imageGlow.enabled = true;
            imageEffect.rectTransform.DORotate (Vector3.zero.With (z: -180), 5f)
                       .SetLoops (-1, LoopType.Restart);
            imageEffect.rectTransform.DOScale (.85f, 1f)
                       .SetEase (Ease.OutQuad)
                       .SetLoops (-1, LoopType.Yoyo);
        }
    }
}