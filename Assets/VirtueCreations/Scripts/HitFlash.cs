using System.Collections;
using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class HitFlash : MonoBehaviour {
        Renderer _renderer;

        [SerializeField]
        GradientVariable flashGradient;

        void Awake () {
            _renderer = GetComponent<Renderer> ();
            foreach (var mat in _renderer.materials) {
                mat.EnableKeyword ("_EMISSION");
            }
        }

        public void Flash () {
            foreach (var mat in _renderer.materials) {
                mat.DOGradientColor (flashGradient.value, "_EmissionColor", 0.175f);
            }
        }

        internal void StartFlashing (float immuneDuration) {
            bool canFlash;
            if (!gameObject.activeInHierarchy) {
                return;
            }
            StartCoroutine (ContinueFlash ());

            IEnumerator ContinueFlash () {
                canFlash = true;
                const float duration = 0.4f;
                var wait = new WaitForSecondsRealtime (duration);
                DOVirtual.DelayedCall (immuneDuration, () => canFlash = false);
                while (canFlash) {
                    foreach (var mat in _renderer.materials) {
                        mat.DOGradientColor (flashGradient.value, "_EmissionColor", duration).SetUpdate (true).SetEase (Ease.Linear).SetLoops (1, LoopType.Yoyo);
                    }
                    
                    yield return wait;
                }
            }
        }
    }
}