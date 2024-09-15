using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public class RequiredResourceUI : MonoBehaviour {
        public string amountFormat;
        public Image icon;
        public TextMeshProUGUI amountText;

        public void SetAmount (int amount) {
            amountText.text = Helper.Abbreviate (amount);
        }

        public void SetAmount (params int[] args) {
            amountText.text = string.Format (amountFormat, args[0], args[1]);
        }
    }
}