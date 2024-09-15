using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public class HandAnim_Tap : HandAnim {
        [SerializeField]
        Image hand, circle;

        void Awake () {
            ResetCircle ();
        }

        /*public void PlayOnce () {
            DOTween.Sequence ()
                   .SetId (GetHashCode ())
                   .Append (hand.DOScale (0.8f, .3f))
                   .Append (circle.DOScale (Vector3.one, .4f))
                   .Join (hand.DOScale (Vector3.one, .5f))
                   .AppendCallback (ResetCircle);
        }*/

        public override void PlayAnim () {
            gameObject.Show ();
            transform.SetAsLastSibling ();
            DOTween.Sequence ()
                   .SetUpdate (true)
                   .SetLoops (-1, LoopType.Restart)
                   .SetId (GetHashCode ())
                   .Append (hand.transform.DOScale (0.8f, .15f))
                   .Join (circle.transform.DOScale (Vector3.one, .2f).SetDelay (0.075f))
                   .Join (circle.DOFade (0, 0.075f).SetDelay (.2f))
                   .Append (hand.transform.DOScale (Vector3.one, .25f))
                   .AppendCallback (ResetCircle);
        }

        public override void StopAnim () {
            DOTween.Kill (GetHashCode ());
            hand.transform.localScale = Vector3.one;
            ResetCircle ();
        }

        public override void StopAnimAndHide () {
            DOTween.Kill (GetHashCode ());
            gameObject.Hide ();
        }

        void ResetCircle () {
            circle.transform.localScale = Vector3.zero;
            circle.color = Color.white;
        }
    }
}