using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class StartScreen : MonoBehaviour {
        [SerializeField]
        Transform icon;

        [SerializeField]
        GameObject slider;

        void Start () {
            DOTween.Sequence ()
                   .Append (icon.DOScale (Vector3.one, 1f).From (Vector3.zero).SetEase (Ease.OutBack))
                   .AppendCallback (ShowSlider);
        }

        void ShowSlider () {
            slider.Show ();
        }
    }
}