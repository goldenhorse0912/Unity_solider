using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public class Loading : MonoBehaviour {
        [SerializeField]
        Image fillImage;

        [SerializeField]
        TextMeshProUGUI loadingText;

        void Awake () {
            Helper.LoadingValue = 0f;
        }

        void Update () {
            fillImage.fillAmount = Helper.LoadingValue;
            loadingText.text = $"{(int)(fillImage.fillAmount * 100)}%";
        }
    }
}