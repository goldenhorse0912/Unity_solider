using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    [DefaultExecutionOrder (-10)]
    public class GrayScalableButton : CachedMonoBehaviour {
        Material _mat;
        Button _button;
        bool _atMax;

        [SerializeField]
        Material greyScaleMat;
        [SerializeField]
        Image[] greyScaleImages;
        [SerializeField]
        bool isEnergyCost;
        [SerializeField]
        IntVariable price;

        protected override void OnAwake () {
            base.OnAwake ();
            _button = GetComponent<Button> ();
            _mat = new Material (greyScaleMat);
            _mat.SetFloat (ShaderParams.GrayscaleAmount, 0f);
            foreach (var img in greyScaleImages) {
                img.material = _mat;
            }
        }

        internal void ToGreyscale () {
            if (_mat == null) {
                return;
            }
            _mat.SetFloat (ShaderParams.GrayscaleAmount, 1f);
            _button.interactable = false;
        }

        internal void ToColor () {
            if (_mat == null) {
                return;
            }
            _mat.SetFloat (ShaderParams.GrayscaleAmount, 0f);
            _button.interactable = true;
        }

        internal void SetCurrentCostValue (int amount) {
            price.Value = amount;
            CheckButtonValue ();
        }

        internal void CheckButtonValue () {
            if (_atMax) return;
            if (isEnergyCost) {
                if (MainButtons.Instance.EnergyAmount >= price.Value) {
                    ToColor ();
                } else {
                    ToGreyscale ();
                }
            } else {
                var coins = Manager.Instance.UIManager.GetAmountByTypeOfResource (Resource.Coin);
                var p = price.Value;
                if (coins >= p) {
                    ToColor ();
                } else {
                    ToGreyscale ();
                }
            }
        }

        internal void ButtonAtMax (bool state) {
            if (state) {
                ToGreyscale ();
                _atMax = true;
            } else {
                _atMax = false;
            }
        }
    }
}