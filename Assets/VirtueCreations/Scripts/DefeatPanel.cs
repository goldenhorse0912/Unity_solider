using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class DefeatPanel : ResultPanel {
        protected override void Init () {
            decorLeft.rectTransform.rotation = Quaternion.Euler (0, 0, 84);
            decorRight.rectTransform.rotation = Quaternion.Euler (0, 0, -84);
        }

        protected override void PlayAnim () {
            DOTween.Sequence ()
                   .Append (blackBg.DOFade (.98f, .5f))
                   .Append (headerImg.rectTransform.DOScale (1f, .5f))
                   .Append (bgImg.DOFillAmount (1f, .5f))
                   .AppendCallback (() => {
                        decorLeft.enabled = true;
                        decorRight.enabled = true;
                    })
                   .AppendCallback (() => {
                        head.enabled = true;
                    })
                   .AppendCallback (() => CountEarnedCurrency (false))
                   .Append (head.rectTransform.DOLocalMoveY (430, .75f).SetEase (Ease.OutBack))
                   .Join (decorLeft.rectTransform.DOLocalRotate (Vector3.zero, .75f).SetEase (Ease.OutBack))
                   .Join (decorRight.rectTransform.DOLocalRotate (Vector3.zero, .75f).SetEase (Ease.OutBack))
                   .AppendInterval (1f)
                   .AppendCallback (() => {
                        resultBtn.gameObject.Show ();
                        resultBtn.transform.DOScale (1f, .5f)
                                 .SetEase (Ease.OutBack);
                    });
        }
    }
}