using System.Collections;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public abstract class ResultPanel : MonoBehaviour {
        [SerializeField]
        protected Image blackBg;

        [SerializeField]
        protected Image bgImg;

        [SerializeField]
        protected Image headerImg;

        [SerializeField]
        protected Image decorLeft;

        [SerializeField]
        protected Image decorRight;

        [SerializeField]
        protected Image head;

        [SerializeField]
        protected TextMeshProUGUI earnedCurrencyText;

        [SerializeField]
        protected Button resultBtn;

        [SerializeField]
        protected IntVariable currency;

        void Awake () {
            blackBg.color = Color.clear;
            headerImg.rectTransform.localScale = Vector3.zero;
            bgImg.fillAmount = 0f;
            head.enabled = false;
            head.rectTransform.localPosition = Vector3.zero;
            decorLeft.enabled = false;
            decorRight.enabled = false;
            resultBtn.gameObject.Hide ();
            resultBtn.transform.localScale = Vector3.zero;
            Init ();
        }

        IEnumerator Start () {
            yield return Helper.WaitFor (1f);
            PlayAnim ();
        }

        public void CountEarnedCurrency (bool isVictory) {
            /*if (currency.Value > 0) {
                Manager.Instance.AudioManager.Play (SoundConstants.Clip_CurrencyCounter);
                DOTween.To (SetEarnedCurrencyText, 0, currency.Value, 1.3f);
            }*/
            var rewardCoins = Manager.Instance.TroopManager.currentAutoSpawnManager.GetReward ();
            if (isVictory) {
                if (rewardCoins < 100) {
                    rewardCoins = 100;
                }
            }
            if (rewardCoins > 0) {
                Manager.Instance.AudioManager.Play (SoundConstants.Clip_CurrencyCounter);
                DOTween.To (SetEarnedCurrencyText, 0, rewardCoins, 1.3f).OnComplete (() => {
                    Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, rewardCoins);
                });
            }
        }

        void SetEarnedCurrencyText (float value) {
            earnedCurrencyText.text = $"+{(int)value}";
        }

        [Button]
        protected abstract void PlayAnim ();

        protected abstract void Init ();
    }
}