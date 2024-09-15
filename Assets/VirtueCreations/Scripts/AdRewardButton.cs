using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE
{
    public class AdRewardButton : CachedMonoBehaviour
    {
        int _clickCount = 0;
        [SerializeField] bool isEnergy;
        [SerializeField] TextMeshProUGUI amountText;
        [SerializeField] Transform destination;
        [SerializeField] Transform rewardImg;

        [SerializeField] int[] lvlLimit;

        [SerializeField] List<int> rewardAmount;

        void Start()
        {
            DonShow();
            return;
            SetAmountText(rewardAmount[_clickCount]);
        }

        void SetAmountText(int amount)
        {
            // amountText.text = $"{amount}";
            amountText.text = Helper.Abbreviate(amount);
        }

        public void ShowReward()
        {
           // AdsHandler.Instance.ShowRewarded(CollectReward);
        }

        public void CollectReward()
        {
            GetComponent<Button>().interactable = false;

           // Manager.Instance.VirtueAnalytics.LogRewardEvent(isEnergy ? "EnergyReward" : "GoldReward");

            CachedTransform.DOScale(CachedTransform.localScale, .1f).From(Vector3.one * .7f).OnStart(() =>
            {
                Manager.Instance.AudioManager.Play(SoundConstants.Clip_UnlockItem);
                Manager.Instance.AudioManager.PlayHaptic();
            }).SetUpdate(true);
            bool collectionStarted = false;
            for (int i = 0; i < 10; i++)
            {
                var img = Instantiate(rewardImg, CachedTransform.parent);
                img.position = CachedTransform.position + new Vector3(x: Random.Range(-50f, 50f), y: Random.Range(-50f, 50f));
                img.DOMove(destination.position, .5f).SetDelay(Random.Range(0f, .3f)).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(img.gameObject);
                    if (collectionStarted) return;
                    collectionStarted = true;
                    if (isEnergy)
                    {
                        MainButtons.Instance.ChangeEnergyWithCounter(rewardAmount[_clickCount]);
                    }
                    else
                    {
                        Manager.Instance.UIManager.AddAmountWithCounter(Resource.Coin, rewardAmount[_clickCount]);
                    }

                    /*if (_clickCount < rewardAmount.Count - 1) {
                        _clickCount++;
                    }*/
                    CheckLevelAndIncreaseCount();
                    SetAmountText(rewardAmount[_clickCount]);
                    gameObject.Hide();
                    DOVirtual.DelayedCall(10f, () =>
                    {
                        if (MainButtons.GameOver) return;
                        TweenAppear();
                    }).SetId(GetHashCode());
                });
            }
        }

        void CheckLevelAndIncreaseCount()
        {
            var lvl = Manager.Instance.BuildingManager.levelBuildingData.levelCount;
            switch (lvl)
            {
                case 1:
                    if (_clickCount < lvlLimit[0] - 1)
                    {
                        _clickCount++;
                    }

                    break;

                case 2:
                    if (_clickCount < lvlLimit[1] - 1)
                    {
                        _clickCount++;
                    }

                    break;

                case 3:
                    if (_clickCount < lvlLimit[2] - 1)
                    {
                        _clickCount++;
                    }

                    break;
            }
        }

        public void TweenAppear()
        {
            DonShow();
            if (gameObject.activeInHierarchy)
            {
                return;
            }

            GetComponent<Button>().interactable = true;
            gameObject.Show();
            CachedTransform.DOScale(CachedTransform.localScale, .7f).From(Vector3.zero).SetEase(Ease.OutBack);
        }

        public void DonShow()
        {
            DOTween.Kill(GetHashCode());
            gameObject.Hide();
        }
    }
}