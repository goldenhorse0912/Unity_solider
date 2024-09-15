using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public sealed class FadeScreen : MonoBehaviour {
        public const float DURATION = 0.25f;

        bool _isFading;
        [SerializeField]
        Image fadeImage;
        
        [SerializeField]
        Image loading;

        [SerializeField]
        bool fadeOnStart;

        void Start () {
            if (fadeOnStart) FadeIn ();
        }

        /// <summary>
        /// From Black to Clear
        /// </summary>
        public void FadeIn () {
            if (_isFading) return;
            _isFading = true;
            loading.DOKill ();
            loading.fillAmount = 0f;
            fadeImage.DOFade (0f, DURATION).From (1f).OnComplete (ResetFading);
        }

        /// <summary>
        /// From Clear to Black
        /// </summary>
        public void FadeOut () {
            if (_isFading) return;
            _isFading = true;
            fadeImage.DOFade (1f, DURATION).From (0f).OnComplete (() => {
                ResetFading ();
                loading.DOFillAmount (1f, .5f).SetLoops (-1,LoopType.Yoyo).SetEase (Ease.InOutQuad);
            });
        }

        void ResetFading () {
            _isFading = false;
        }
    }
}