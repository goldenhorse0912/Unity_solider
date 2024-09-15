using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public class FillCanvas : MonoBehaviour {
        public Image fillImage;
        public Image border;
        public Image icon;
        public TextMeshProUGUI titleText;

        public void ResetFillImage () => fillImage.fillAmount = 0f;

        public void ToggleVisibility (bool state) => GetComponent<Canvas> ().enabled = state;

        public void DownScale () {
            ((RectTransform)transform).sizeDelta = new Vector2 (240, 240);
            border.pixelsPerUnitMultiplier = 3.2f;
            titleText.fontSizeMax = 40;
            titleText.fontSizeMin = 25;
            titleText.GetComponent<LayoutElement> ().minHeight = 50;
        }
    }
}