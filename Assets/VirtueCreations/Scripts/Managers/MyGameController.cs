using System;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VIRTUE
{
    public sealed partial class GameController
    {
        public int IncomeMultiplier { get; set; } = 1;

        private int tapCount;

        public void ShowMaxDebugger()
        {
            tapCount++;
            Debug.Log($"------->TapCount = {tapCount}");
            if (tapCount <= 10) return;
            tapCount = 0;

            // Show Mediation Debugger
            // MaxSdk.ShowMediationDebugger();
        }

        public void LogLevelStartedEvent(int levelNo)
        {
            //Manager.Instance.VirtueAnalytics.LogLevelStartedEvent(levelNo);
        }

        public void LogLevelCompleteEvent(int levelNo)
        {
            // Manager.Instance.VirtueAnalytics.LogLevelCompleteEvent(levelNo);
        }

        public void LogLevelFailedEvent(int levelNo)
        {
            //  Manager.Instance.VirtueAnalytics.LogLevelFailedEvent(levelNo);
        }

        void OnEnable()
        {
            PauseEvent.AddListener(SaveData);
        }

        void OnDisable()
        {
            PauseEvent.RemoveListener(SaveData);
        }

        void SaveData()
        {
            Helper.SetBool(PlayerPrefsKey.FIRST_TIME_LAUNCH, false);
        }

        public void UnlockResource(int id)
        {
            var unlockedResources = PlayerPrefs.GetString(PlayerPrefsKey.UNLOCKED_RESOURCES, "0").Split(',').Select(int.Parse).ToList();
            if (unlockedResources.Contains(id)) return;
            unlockedResources.Add(id);
            var builder = new StringBuilder();
            foreach (var i in unlockedResources)
            {
                builder.Append(i).Append(',');
            }

            builder.Remove(builder.Length - 1, 1);
            PlayerPrefs.SetString(PlayerPrefsKey.UNLOCKED_RESOURCES, builder.ToString());
        }
    }
}