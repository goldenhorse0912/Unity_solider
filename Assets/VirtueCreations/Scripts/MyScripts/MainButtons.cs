using System;
using System.Collections.Generic;
using System.Linq;
using BayatGames.SaveGameFree;
using DG.Tweening;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace VIRTUE
{
    public class MainButtons : MonoBehaviour
    {
        public static MainButtons Instance;

        Camera _mainCam;
        internal int ShownBtnIndex = 99;
        float _energySpeed = 0.18f;
        float _time;
        bool _handMoved;
        int _energySpeedUpLvl = 1;

        [SerializeField] VoidGameEvent victory;
        [SerializeField] XpManager xpManager;
        [SerializeField] Image energyFillImg;
        [SerializeField] TextMeshProUGUI energyText;

        [SerializeField, FoldoutGroup("Energy Upgrade")]
        TextMeshProUGUI energyUpText, energyUpCost;

        [SerializeField, FoldoutGroup("Energy Upgrade")]
        GrayScalableButton energyUpBtn;

        [SerializeField] List<GameObject> buttonsGroup;
        [SerializeField] List<GrayScalableButton> grayScalableButtons;

        internal static bool GameOver { get; private set; }
        internal int EnergyAmount { get; private set; }

        void OnEnable()
        {
            victory.OnEvent += ResetData;
        }

        void OnDisable()
        {
            victory.OnEvent -= ResetData;
        }

        public void ResetLvl()
        {
            LoadData();
            ChangeEnergyAmount(-EnergyAmount);
            StopGeneratingEnergy();
            GameOver = false;
            if (Helper.GetBool(PlayerPrefsKey.TUTORIALCOMPLETED, false))
            {
                StartGeneratingEnergy();
            }
            else
            {
                ChangeEnergyAmount(6);
            }

            energyUpBtn.SetCurrentCostValue(GetEnergyUpCostOfCurrentLvl());
            // ShowBtn (!Manager.Instance.TroopManager.BuildingExistInScene () ? 0 : 1);
            ShowBtn(1);
        }

        void Awake()
        {
            Instance = this;
            _mainCam = Camera.main;
        }

        void Update()
        {
            if (EventSystem.current.IsPointerOverUIObject()) return;

            // Check for touch input on mobile
            if (Input.touchCount > 0)
            {
                var touch = Input.GetTouch(0);
                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        _handMoved = false;
                        break;

                    case TouchPhase.Moved:
                        _handMoved = true;
                        break;

                    case TouchPhase.Ended when !_handMoved:
                    {
                        CheckTouchDetect(touch.position);
                        break;
                    }
                }
            }
            // Check for mouse click on PC or editor
            else if (Input.GetMouseButtonDown(0))
            {
                CheckTouchDetect(Input.mousePosition);
            }
        }

        private void CheckTouchDetect(Vector3 position)
        {
            var ray = _mainCam.ScreenPointToRay(position);
            if (Physics.Raycast(ray, out var hit) && !GameOver)
            {
                CheckTargetType(hit.collider.transform);
            }
        }

        public void ScaleBtn(Transform target)
        {
            DOTween.Kill(target);
            target.DOScale(Vector3.one, .1f).From(Vector3.one * .9f).OnStart(() =>
            {
                Manager.Instance.AudioManager.Play(SoundConstants.Clip_ButtonClick);
                Manager.Instance.AudioManager.PlayHaptic();
            }).SetUpdate(true).SetId(target);
        }

        internal void ChangeEnergyAmount(int amount)
        {
            EnergyAmount += amount;
            if (EnergyAmount < 0)
            {
                EnergyAmount = 0;
            }

            energyText.text = EnergyAmount.ToString();
            RefreshAllButtons();
        }

        [Button]
        public void ChangeEnergyWithCounter(int amount)
        {
            var currentAmount = EnergyAmount;
            EnergyAmount += amount;
            if (EnergyAmount < 0)
            {
                EnergyAmount = 0;
            }

            DOTween.To(SetEarnedCurrencyText, currentAmount, EnergyAmount, .5f).SetUpdate(true);

            void SetEarnedCurrencyText(float value)
            {
                energyText.text = $"{(int)value}";
            }

            RefreshAllButtons();
        }

        public void RefreshAllButtons()
        {
            foreach (var scalableButton in grayScalableButtons)
            {
                scalableButton.CheckButtonValue();
            }

            energyUpBtn.CheckButtonValue();
        }

        public void RefreshButtonsExceptEnergyUp()
        {
            foreach (var scalableButton in grayScalableButtons.Where(scalableButton => scalableButton.transform.name is not ("WatchTower" or "CheckPost" or "PistolTroopers")))
            {
                scalableButton.CheckButtonValue();
            }
        }

        public void ShowAddBuildingBtn()
        {
            foreach (var btn in buttonsGroup)
            {
                btn.Hide();
            }

            buttonsGroup[0].Show();
            DOVirtual.DelayedCall(0.05f, () => ShownBtnIndex = 99, false);
        }

        internal void CheckTargetType(Transform target, bool toShow = true)
        {
            var objTag = target.tag;
            if (toShow)
            {
                switch (objTag)
                {
                    case BuildingTags.MainBase:
                        ShowBtn(1, target);
                        break;

                    case BuildingTags.GroundTroops:
                        ShowBtn(2, target);
                        break;

                    case BuildingTags.VehicleTroops:
                        ShowBtn(3, target);
                        break;

                    case BuildingTags.FlyingTroops:
                        ShowBtn(4, target);
                        break;
                }
            }
            else
            {
                switch (objTag)
                {
                    case BuildingTags.MainBase:
                        ShowDefaultBtn(1);
                        break;

                    case BuildingTags.GroundTroops:
                        ShowDefaultBtn(2);
                        break;

                    case BuildingTags.VehicleTroops:
                        ShowDefaultBtn(3);
                        break;

                    case BuildingTags.FlyingTroops:
                        ShowDefaultBtn(4);
                        break;
                }
            }
        }

        void ShowBtn(int index, Transform target = null)
        {
            if (ShownBtnIndex == index) return;
            if (!TutorialScript.Instance.mainBaseClicked)
            {
                if (index == 2)
                {
                    ShownBtnIndex = index;
                    TutorialScript.Instance.MoveHandToPistolTroopBtn();
                }
                else
                {
                    if (target)
                    {
                        return;
                    }
                }
            }
            else if (!TutorialScript.Instance.speedUpBtnClicked)
            {
                if (index == 1)
                {
                    ShownBtnIndex = index;
                    TutorialScript.Instance.MoveHandToSpeedUpBtn();
                    TutorialScript.Instance.InteractionWithAddBuildingBtn(false);
                }
                else
                {
                    if (target)
                    {
                        return;
                    }
                }
            }
            else if (!TutorialScript.Instance.speedUpComplete)
            {
                return;
            }
            else if (TutorialScript.Instance.energyCollected && !TutorialScript.Instance.troopsSpawnedAgain)
            {
                if (index == 2)
                {
                    ShownBtnIndex = index;
                    TutorialScript.Instance.HandToPistolBtn();
                }
                else
                {
                    if (target)
                    {
                        return;
                    }
                }
            }
            else if (!TutorialScript.Instance.mainBaseUpgraded)
            {
                if (TutorialScript.Instance.coinsChecked)
                {
                    if (TutorialScript.Instance.enoughMoney)
                    {
                        if (index == 1)
                        {
                            ShownBtnIndex = index;
                            TutorialScript.Instance.MoveHandToBaseUpBtn();
                        }
                        else
                        {
                            return;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
            else if (TutorialScript.Instance.mainBaseUpgraded && !TutorialScript.Instance.mainBaseUpComplete)
            {
                if (target)
                {
                    return;
                }
            }
            else if (!Helper.GetBool(PlayerPrefsKey.TROOPBUILDINGUPCOMPLETE, false))
            {
                if (index == 2)
                {
                    TutorialScript.Instance.MoveHandToRifleBtn();
                }
                else
                {
                    if (target)
                    {
                        return;
                    }
                }
            }

            ShownBtnIndex = index;
            Manager.Instance.TroopManager.CheckBuildingToShowBtn();
            foreach (var btn in buttonsGroup)
            {
                btn.Hide();
            }

            buttonsGroup[index].Show();
            if (target)
            {
                target.DOPunchScale(Vector3.up * .35f, .3f).OnStart(() =>
                {
                    Manager.Instance.AudioManager.Play(SoundConstants.Clip_BuildingClick);
                    Manager.Instance.AudioManager.PlayHaptic();
                }).SetUpdate(true);
            }
        }

        void ShowDefaultBtn(int index)
        {
            if (ShownBtnIndex == index)
            {
                ShowBtn(1);
            }
        }

        public void StartGeneratingEnergy()
        {
            energyFillImg.DOFillAmount(1, 1 / _energySpeed).From(0).SetId(Tags.GENERATINGENERGY).OnComplete(() =>
            {
                ChangeEnergyAmount(1);
                StartGeneratingEnergy();
            });
        }

        // called when GameOver
        internal void StopGeneratingEnergy()
        {
            GameOver = true;
            DOTween.Kill(Tags.GENERATINGENERGY);
            energyFillImg.fillAmount = 0;
            foreach (var btn in buttonsGroup)
            {
                btn.Hide();
            }

            ShownBtnIndex = 99;
        }

        public void EnergySpeedUpgrade()
        {
            if (GameOver) return;
            var value = GetEnergyUpCostOfCurrentLvl();
            var coinAmount = Manager.Instance.UIManager.GetAmountByTypeOfResource(Resource.Coin);
            if (coinAmount >= value)
            {
                Manager.Instance.UIManager.AddAmountByTypeOfResource(Resource.Coin, -value);
                _energySpeedUpLvl++;
                var val = xpManager.GetXPValue("EnergySpeedGraph", _energySpeedUpLvl) * 0.02f;
                _energySpeed += val;
                var newPrice = GetEnergyUpCostOfCurrentLvl();
                energyUpCost.text = Helper.Abbreviate(newPrice);
                energyUpText.text = $"ENERGY SPEED\n{_energySpeed:0.00}/SEC";
                energyUpBtn.SetCurrentCostValue(newPrice);
                SaveData();
                Manager.Instance.AudioManager.PlayHaptic();
                energyFillImg.DOKill();
                energyFillImg.DOFillAmount(1, _energySpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
                {
                    ChangeEnergyAmount(1);
                    StartGeneratingEnergy();
                });
            }
            else
            {
                this.Log("Insufficient Coins");
            }
        }

        int GetEnergyUpCostOfCurrentLvl()
        {
            var val = xpManager.GetXPValue(xpManager.XPGroupList[0].GraphName, _energySpeedUpLvl);
            return Convert.ToInt32(val);
        }

        internal int GetRewardAccordingToWave(int index)
        {
            var graph = xpManager.XPGroupList[1];
            if (index > graph.levelMax)
            {
                index = graph.levelMax;
            }

            var val = xpManager.GetXPValue(graph.GraphName, index);
            return Convert.ToInt32(val);
        }

        void SaveData()
        {
            SaveGame.Save("energySpeed", _energySpeed);
            SaveGame.Save("energySpeedUpLvl", _energySpeedUpLvl);
        }

        void LoadData()
        {
            if (Helper.GetBool(PlayerPrefsKey.TUTORIALCOMPLETED, false))
            {
                _energySpeed = SaveGame.Load("energySpeed", 0.18f);
                energyUpText.text = $"ENERGY SPEED\n{_energySpeed:0.00}/SEC";
                _energySpeedUpLvl = SaveGame.Load("energySpeedUpLvl", 1);
                energyUpCost.text = Helper.Abbreviate(GetEnergyUpCostOfCurrentLvl());
            }
            else
            {
                _energySpeed = 0.18f;
                energyUpText.text = $"ENERGY SPEED\n{_energySpeed:0.00}/SEC";
                _energySpeedUpLvl = 1;
                energyUpCost.text = Helper.Abbreviate(GetEnergyUpCostOfCurrentLvl());
            }
        }

        // Reset Everything for new level
        void ResetData()
        {
            _energySpeed = 0.18f;
            _energySpeedUpLvl = 1;
            SaveData();
        }
    }
}