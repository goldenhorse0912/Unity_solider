using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public class ResourceUI : MonoBehaviour {
        int _index;
        public Resource typeOfResource;
        public int amount;
        public Image icon;
        public TextMeshProUGUI amountText;

        void Awake () {
            _index = typeOfResource.ToInt ();
        }

        public void SetAmount (int value) {
            amount = value;
            amountText.text = Helper.Abbreviate (amount);
            Show ();
        }

        public void Add (int value) {
            amount += value;
            amountText.text = Helper.Abbreviate (amount);
            Show ();
        }

        public void AddWithCounter (int value, Action action = null) {
            var currentAmount = amount;
            amount += value;
            DOTween.To (SetEarnedCurrencyText, currentAmount, amount, .5f).SetUpdate (true).OnComplete (() => action?.Invoke ());

            void SetEarnedCurrencyText (float count) {
                amountText.text = Helper.Abbreviate ((int)count);
            }
        }

        public void Show () {
            gameObject.Show ();
            if (_index < 4) return;
            DOTween.Kill (GetHashCode ());
            DOVirtual.DelayedCall (10, gameObject.Hide)
                     .SetId (GetHashCode ());
        }

        public void Hide () {
            DOTween.Kill (GetHashCode ());
            gameObject.Hide ();
        }
    }
}