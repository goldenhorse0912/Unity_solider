using UnityEngine;
//using GameAnalyticsSDK;
using System.Collections;

//using Facebook.Unity;

namespace VIRTUE
{
    public sealed class VirtueAnalytics : MonoBehaviour
    {
        [SerializeField] string adSdkName = string.Empty;

        void Awake()
        {
            //FacebookInit();
        }

        void Start()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                //GameAnalytics.RequestTrackingAuthorization(this);
            }
            else
            {
               // GameAnalytics.Initialize();
                print("--------------------->Initialize");
                // GameAnalytics.OnRemoteConfigsUpdatedEvent += OnRemoteConfigsUpdated;
            }
        }

        void OnRemoteConfigsUpdated()
        {
           // var value = GameAnalytics.GetRemoteConfigsValueAsString("ITimer", "120");
            // AdsHandler.Instance.SetAndStartTimer(int.Parse(value));
           // Debug.Log("===================> " + value);
        }

       /* public void GameAnalyticsATTListenerNotDetermined() => GameAnalytics.Initialize();
        public void GameAnalyticsATTListenerRestricted() => GameAnalytics.Initialize();
        public void GameAnalyticsATTListenerDenied() => GameAnalytics.Initialize();
        public void GameAnalyticsATTListenerAuthorized() => GameAnalytics.Initialize();*/

    /*    public void LogLevelStartedEvent(int levelNo) => GameAnalytics.NewProgressionEvent(GAProgressionStatus.Start, levelNo.ToString());
        public void LogLevelCompleteEvent(int levelNo) => GameAnalytics.NewProgressionEvent(GAProgressionStatus.Complete, levelNo.ToString());
        public void LogLevelFailedEvent(int levelNo) => GameAnalytics.NewProgressionEvent(GAProgressionStatus.Fail, levelNo.ToString());

        public void LogUnlockItemEvent(string eventName)
        {
            this.Log($"ItemUnlocked : {eventName}");
            GameAnalytics.NewDesignEvent($"ItemUnlocked : {eventName}");
        }

        public void LogUpgradeEvent(string eventName)
        {
            this.Log($"Upgrades : {eventName}");
            GameAnalytics.NewDesignEvent($"Upgrades : {eventName}");
        }

        public void LogInterstitialEvent()
        {
            this.Log("RewardType : Interstitial");
            GameAnalytics.NewDesignEvent("RewardType : Interstitial");
            GameAnalytics.NewAdEvent(GAAdAction.Show, GAAdType.Interstitial, adSdkName, "Interstitial");
        }

        public void LogRewardEvent(string eventName)
        {
            this.Log($"RewardType : {eventName}");
            GameAnalytics.NewDesignEvent($"RewardType : {eventName}");
            GameAnalytics.NewAdEvent(GAAdAction.RewardReceived, GAAdType.RewardedVideo, adSdkName, "Reward");
        }*/

        /*void FacebookInit()
        {
            if (FB.IsInitialized)
            {
                this.Log("FB App Activated");
                FB.ActivateApp();
            }
            else
            {
                this.Log("Start FB Init");
                FB.Init(InitCallback, OnHideUnity);
            }
        }*/

        /* void InitCallback()
         {
             if (FB.IsInitialized)
             {
                 this.Log("FB Initialized Successfully");
                 FB.ActivateApp();
             }
             else
             {
                 this.Log("Failed to Initialize the FB");
             }
         }*/

        void OnHideUnity(bool isGameShown) => Time.timeScale = isGameShown ? 1 : 0;
    }
}