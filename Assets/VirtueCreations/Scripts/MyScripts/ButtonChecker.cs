using System;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public enum ButtonType {
        Pistol,
        Rifle,
        Sniper,
        Bike,
        Jeep,
        Tank,
        Drone,
        BulletStorm,
        FuryPhoenix
    }

    [Serializable]
    public class ButtonData {
        public int unlockAtLvl;
        public ButtonType buttonType;
        public bool isPurchased, isUnlock;
        public GameObject lockBtn;
        public GameObject purchaseBtn;
        public GameObject spawnButton;
    }

    public class SaveAbleBtnData {
        public bool IsPurchased, IsUnlock;
    }

    public class ButtonChecker : MonoBehaviour {
        [SerializeField]
        List<ButtonData> buttons;
        [SerializeField]
        VoidGameEvent victory;

        public List<SaveAbleBtnData> saveAbleBtnData = new();

        void OnEnable () {
            PauseEvent.AddListener (SaveData);
            victory.OnEvent += ResetAllButtons;
        }

        void OnDisable () {
            PauseEvent.RemoveListener (SaveData);
            victory.OnEvent -= ResetAllButtons;
        }

        void Awake () {
            LoadData ();
        }

        /// call when new level start to lock the purchased button
        void LockBtn (ButtonType buttonType) {
            var btnData = buttons.Find (x => x.buttonType == buttonType);
            btnData.isUnlock = false;
            btnData.isPurchased = false;
            btnData.lockBtn.Show ();
            btnData.purchaseBtn.Hide ();
            btnData.spawnButton.Hide ();
        }

        void UnlockBtn (ButtonType buttonType) {
            var btnData = buttons.Find (x => x.buttonType == buttonType);
            btnData.isUnlock = true;
            btnData.purchaseBtn.Show ();
            btnData.lockBtn.Hide ();
            btnData.spawnButton.Hide ();
        }

        /// call when building upgrade
        public void MainBaseUpgraded (int lvl) {
            var btnData = buttons.FindAll (x => x.unlockAtLvl == lvl);
            foreach (var buttonData in btnData) {
                buttonData.isUnlock = true;
                buttonData.purchaseBtn.Show ();
                buttonData.lockBtn.Hide ();
                buttonData.spawnButton.Hide ();
            }
        }

        /// <summary>
        /// call when troop/button purchased
        /// </summary>
        /// <param name="buttonType">button type index from enum</param>
        public void PurchaseBtn (int buttonType) {
            var btnData = buttons.Find (x => x.buttonType == (ButtonType)buttonType);
            btnData.isPurchased = true;
            btnData.lockBtn.Hide ();
            btnData.purchaseBtn.Hide ();
            btnData.spawnButton.Show ();
            btnData.spawnButton.transform.DOLocalRotate (Vector3.zero, .2f).From (Vector3.up * 90);
            MainButtons.Instance.RefreshAllButtons ();
        }

        void SaveData () {
            saveAbleBtnData.Clear ();
            for (var i = 0; i < buttons.Count; i++) {
                var sd = new SaveAbleBtnData {
                    IsPurchased = buttons[i].isPurchased,
                    IsUnlock = buttons[i].isUnlock
                };
                saveAbleBtnData.Add (sd);
            }
            SaveGame.Save (gameObject.name, saveAbleBtnData);
        }

        void LoadData () {
            if (SaveGame.Exists (gameObject.name)) {
                saveAbleBtnData = SaveGame.Load (gameObject.name, saveAbleBtnData);
                for (var i = 0; i < saveAbleBtnData.Count; i++) {
                    buttons[i].isPurchased = saveAbleBtnData[i].IsPurchased;
                    buttons[i].isUnlock = saveAbleBtnData[i].IsUnlock;
                }
            }
            LoadButtons ();
        }

        void LoadButtons () {
            foreach (var btnData in buttons) {
                if (btnData.isPurchased) {
                    PurchaseBtn ((int)btnData.buttonType);
                } else if (btnData.isUnlock) {
                    UnlockBtn (btnData.buttonType);
                } else {
                    LockBtn (btnData.buttonType);
                }
            }
            MainButtons.Instance.RefreshAllButtons ();
        }

        // call when new level starts
        void ResetAllButtons () {
            foreach (var buttonData in buttons) {
                LockBtn (buttonData.buttonType);
            }
        }
    }
}