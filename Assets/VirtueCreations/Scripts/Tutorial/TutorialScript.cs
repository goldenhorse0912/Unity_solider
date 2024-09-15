using BayatGames.SaveGameFree;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace VIRTUE
{
    public class TutorialScript : MonoBehaviour
    {
        public static TutorialScript Instance;

        const float Delay = .3f;
        int _upCount = 0, _spawnTroopCount = 0, _amount;
        Transform _troopSpawnerBuilding, _mainBaseBuilding;

        [SerializeField] VoidGameEvent victory;
        [SerializeField] TextMeshProUGUI freeCoinText;
        [SerializeField] Button addBuildingBtn, speedUpBtn, pistolTroopBtn, checkPostBtn, watchTowerBtn;

        [SerializeField] GameObject arrow;
        /*  [SerializeField]
          AdRewardButton adEnergyButton, adCoinButton;*/

        public CameraController camController;
        public HandAnim_Tap handAnimTap;

        public Transform
            trooperBuildingBtn,
            enemyMoveToPos,
            freeEnergyBtn,
            freeCoinBtn,
            baseUpBtn,
            rifleBtn,
            energyImg,
            coinImg,
            energyDestination,
            coinDestination;

        [HideInInspector] public bool addBuildingBtnClicked,
            troopBuildingBtnClicked,
            troopBuildingClicked,
            troopSpawned,
            clickedSecondTime,
            mainBaseClicked,
            speedUpBtnClicked,
            speedUpComplete,
            freeBtnShowed,
            energyCollected,
            troopsSpawnedAgain,
            coinsChecked,
            mainBaseUpgraded,
            enoughMoney,
            mainBaseUpComplete,
            troopBuildingUpgraded;

        void OnEnable()
        {
            victory.OnEvent += TutorialComplete;
        }

        void OnDisable()
        {
            victory.OnEvent -= TutorialComplete;
        }

        void TutorialComplete()
        {
            if (!Helper.GetBool(PlayerPrefsKey.TUTORIALCOMPLETED, false))
            {
                Helper.SetBool(PlayerPrefsKey.TUTORIALCOMPLETED, true);
                SaveData();
            }
        }

        void Awake()
        {
            Instance = this;
            LoadData();
        }

        void Start()
        {
            if (!Helper.GetBool(PlayerPrefsKey.TUTORIALCOMPLETED, false))
            {
                DOVirtual.DelayedCall(2f, () =>
                {
                    MoveHandToAddBuildingBtn();
                    camController.enabled = false;
                });
            }
            else
            {
                HideHand();
            }
        }

        internal void HideAdButtons()
        {
            //adCoinButton.DonShow ();
            //adEnergyButton.DonShow ();
        }

        public void InteractionWithAddBuildingBtn(bool state)
        {
            addBuildingBtn.interactable = state;
        }

        public void InteractionWithSpeedUpBtn(bool state)
        {
            speedUpBtn.interactable = state;
        }

        public void InteractionWithPistolBtn(bool state)
        {
            pistolTroopBtn.interactable = state;
        }

        public void InteractionWithCheckPostBtn(bool state)
        {
            checkPostBtn.interactable = state;
        }

        public void InteractionWithWatchTowerBtn(bool state)
        {
            watchTowerBtn.interactable = state;
        }

        void MoveHandTo(Vector3 targetPos)
        {
            PauseTime();
            DOTween.Kill(GetHashCode());
            handAnimTap.StopAnim();
            handAnimTap.transform.DOMove(targetPos, .3f).SetId(GetHashCode()).SetUpdate(true).OnComplete(() => { handAnimTap.PlayAnim(); });
        }

        public void MoveHandToAddBuildingBtn()
        {
            DOVirtual.DelayedCall(Delay, () => MoveHandTo(addBuildingBtn.transform.position));
        }

        public void MoveHandToTrooperBuildingBtn()
        {
            if (addBuildingBtnClicked) return;
            addBuildingBtnClicked = true;
            DOVirtual.DelayedCall(Delay, () => MoveHandTo(trooperBuildingBtn.position));
        }

        public void MoveHandToTroopSpawnerBuilding()
        {
            if (troopBuildingBtnClicked) return;
            troopBuildingBtnClicked = true;
            HandToTroopBuilding();
        }

        public void MoveHandToPistolTroopBtn()
        {
            if (troopBuildingClicked) return;
            troopBuildingClicked = true;
            HandToPistolBtn();
        }

        public void HandToPistolBtn()
        {
            DOVirtual.DelayedCall(Delay, () => MoveHandTo(pistolTroopBtn.transform.position));
        }

        void HandToTroopBuilding()
        {
            _troopSpawnerBuilding = Manager.Instance.BuildingManager.levelBuildingData.buildingsData.Find(x => x.buildingType == BuildingType.WalkingTroopSpawner).spawnPos;
            DOVirtual.DelayedCall(Delay + .3f, () => MoveHandTo(Camera.main.WorldToScreenPoint(_troopSpawnerBuilding.position)));
        }

        public void MoveHandToMainBase()
        {
            if (mainBaseClicked) return;
            PauseTime();
            mainBaseClicked = true;
            _mainBaseBuilding = Manager.Instance.BuildingManager.levelBuildingData.buildingsData.Find(x => x.buildingType == BuildingType.MainBase).spawnPos;
            camController.MoveCameraBase(() =>
            {
                MoveHandTo(Camera.main.WorldToScreenPoint(_mainBaseBuilding.position));
                MoveEnemies();
            });
        }

        public void MoveHandToSpeedUpBtn()
        {
            if (speedUpBtnClicked) return;
            speedUpBtnClicked = true;
            DOVirtual.DelayedCall(Delay, () => MoveHandTo(speedUpBtn.transform.position));
        }

        void MoveHandToShowTouchpad()
        {
            arrow.Show();
            handAnimTap.transform.localPosition = Vector3.up * 500f;
            handAnimTap.transform.DOLocalMove(Vector3.up * -300f, 1.5f)
                .SetDelay(.01f)
                .SetEase(Ease.OutQuad)
                .SetLoops(3, LoopType.Restart)
                .OnComplete(() =>
                {
                    arrow.Hide();
                    HideHand();
                });
        }

        public void StartSpawningEnemy()
        {
            if (clickedSecondTime)
            {
                return;
            }

            if (!troopSpawned)
            {
                troopSpawned = true;
                camController.enabled = true;
                MainButtons.Instance.StartGeneratingEnergy();
            }
            else
            {
                ResumeTime();
                handAnimTap.StopAnim();
                clickedSecondTime = true;
                MoveHandToShowTouchpad();
            }
        }

        public void SpeedUpCompleted()
        {
            if (speedUpComplete)
            {
                return;
            }

            if (_upCount == 2)
            {
                speedUpComplete = true;
                ResumeTime();
                handAnimTap.StopAnim();
                HideHand();
                InteractionWithAddBuildingBtn(true);
            }
            else
            {
                _upCount++;
            }
        }

        public void ShowFreeEnergyBtn()
        {
            if (freeBtnShowed) return;
            freeBtnShowed = true;
            freeEnergyBtn.gameObject.Show();
            handAnimTap.gameObject.Show();
            MoveHandTo(freeEnergyBtn.position);
        }

        public void CollectFreeEnergy()
        {
            if (energyCollected) return;
            energyCollected = true;
            bool collectionStarted = false;
            for (int i = 0; i < 10; i++)
            {
                var img = Instantiate(energyImg, freeEnergyBtn.parent);
                img.position = freeEnergyBtn.position + new Vector3(x: Random.Range(-50f, 50f), y: Random.Range(-50f, 50f));
                img.DOMove(energyDestination.position, .5f).SetUpdate(true).SetDelay(Random.Range(0f, .3f)).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(img.gameObject);
                    if (collectionStarted) return;
                    collectionStarted = true;
                    MainButtons.Instance.ChangeEnergyWithCounter(15);
                    handAnimTap.StopAnim();
                    if (MainButtons.Instance.ShownBtnIndex != 2)
                    {
                        HandToTroopBuilding();
                    }
                    else
                    {
                        HandToPistolBtn();
                    }
                });
            }
        }

        public void SpawnTroopsAgain()
        {
            if (!energyCollected) return;
            _spawnTroopCount++;
            if (_spawnTroopCount != 5 || troopsSpawnedAgain) return;
            troopsSpawnedAgain = true;
            handAnimTap.StopAnim();
            HideHand();
            ResumeTime();
        }

        void MoveEnemies()
        {
            foreach (var enemyTroop in Manager.Instance.TroopManager.enemyTroops)
            {
                enemyTroop.transform.position = enemyMoveToPos.position;
            }
        }

        public void CheckUserCoins()
        {
            if (coinsChecked || Manager.Instance.BuildingManager.levelBuildingData.buildingsData.Find(x => x.buildingType == BuildingType.WalkingTroopSpawner).refObj == null)
            {
                return;
            }

            coinsChecked = true;
            camController.MoveCameraBase(() =>
            {
                ShowHand();
                InteractionWithAddBuildingBtn(false);
                InteractionWithSpeedUpBtn(false);
                InteractionWithPistolBtn(false);
                InteractionWithCheckPostBtn(false);
                InteractionWithWatchTowerBtn(false);
                var coins = Manager.Instance.UIManager.GetAmountByTypeOfResource(Resource.Coin);
                if (coins < 550)
                {
                    _amount = 550 - coins;
                    freeCoinBtn.gameObject.Show();
                    freeCoinText.text = _amount.ToString();
                    DOVirtual.DelayedCall(Delay, () => MoveHandTo(freeCoinBtn.position));
                }
                else
                {
                    CheckBtnToUpgradeBase();
                }
            });
        }

        public void AddFreeCoins()
        {
            bool collectionStarted = false;
            for (int i = 0; i < 10; i++)
            {
                var img = Instantiate(coinImg, freeCoinBtn.parent);
                img.position = freeCoinBtn.position + new Vector3(x: Random.Range(-50f, 50f), y: Random.Range(-50f, 50f));
                img.DOMove(coinDestination.position, .5f).SetUpdate(true).SetDelay(Random.Range(0f, .3f)).SetEase(Ease.InBack).OnComplete(() =>
                {
                    Destroy(img.gameObject);
                    if (collectionStarted) return;
                    collectionStarted = true;
                    Manager.Instance.UIManager.AddAmountByTypeOfResourceSimple(Resource.Coin, _amount);
                    CheckBtnToUpgradeBase();
                });
            }
        }

        public void CheckBtnToUpgradeBase()
        {
            enoughMoney = true;
            if (MainButtons.Instance.ShownBtnIndex == 1)
            {
                MoveHandToBaseUpBtn();
            }
            else
            {
                _mainBaseBuilding = Manager.Instance.BuildingManager.levelBuildingData.buildingsData.Find(x => x.buildingType == BuildingType.MainBase).spawnPos;
                DOVirtual.DelayedCall(Delay, () => MoveHandTo(Camera.main.WorldToScreenPoint(_mainBaseBuilding.position)));
            }
        }

        public void MoveHandToBaseUpBtn()
        {
            if (mainBaseUpgraded)
            {
                return;
            }

            mainBaseUpgraded = true;
            DOVirtual.DelayedCall(Delay, () => MoveHandTo(baseUpBtn.position));
        }

        public void MoveHandToTroopBuildingForUpgrade()
        {
            if (mainBaseUpComplete) return;
            mainBaseUpComplete = true;
            HandToTroopBuilding();
            SaveDataOfNewLvl();
        }

        public void MoveHandToRifleBtn()
        {
            if (troopBuildingUpgraded) return;
            troopBuildingUpgraded = true;
            DOVirtual.DelayedCall(Delay, () => MoveHandTo(rifleBtn.position));
        }

        public void TroopBuildingUpgradeComplete()
        {
            if (Helper.GetBool(PlayerPrefsKey.TROOPBUILDINGUPCOMPLETE, false)) return;
            Helper.SetBool(PlayerPrefsKey.TROOPBUILDINGUPCOMPLETE, true);
            InteractionWithAddBuildingBtn(true);
            InteractionWithSpeedUpBtn(true);
            InteractionWithCheckPostBtn(true);
            InteractionWithWatchTowerBtn(true);
            ResumeTime();
            HideHand();
            SaveDataOfNewLvl();
            DOVirtual.DelayedCall(10f, () =>
            {
                if (MainButtons.GameOver) return;
                //adCoinButton.TweenAppear ();
                //adEnergyButton.TweenAppear ();
            });
        }

        internal void HideHand() => handAnimTap.gameObject.Hide();
        internal void ShowHand() => handAnimTap.gameObject.Show();
        internal static void PauseTime() => Time.timeScale = 0;
        internal static void ResumeTime() => Time.timeScale = 1;

        void SaveData()
        {
            SaveGame.Save("addBuildingBtnClicked", addBuildingBtnClicked);
            SaveGame.Save("troopBuildingBtnClicked", troopBuildingBtnClicked);
            SaveGame.Save("troopBuildingClicked", troopBuildingClicked);
            SaveGame.Save("troopSpawned", troopSpawned);
            SaveGame.Save("clickedSecondTime", clickedSecondTime);
            SaveGame.Save("mainBaseClicked", mainBaseClicked);
            SaveGame.Save("speedUpBtnClicked", speedUpBtnClicked);
            SaveGame.Save("speedUpComplete", speedUpComplete);
            SaveGame.Save("freeBtnShowed", freeBtnShowed);
            SaveGame.Save("energyCollected", energyCollected);
            SaveGame.Save("troopsSpawnedAgain", troopsSpawnedAgain);
            SaveGame.Save("coinsChecked", coinsChecked);
            SaveGame.Save("mainBaseUpgraded", mainBaseUpgraded);
            SaveGame.Save("enoughMoney", enoughMoney);
            SaveGame.Save("mainBaseUpComplete", mainBaseUpComplete);
            SaveGame.Save("troopBuildingUpgraded", troopBuildingUpgraded);
        }

        private void LoadData()
        {
            if (SaveGame.Exists("addBuildingBtnClicked"))
            {
                addBuildingBtnClicked = SaveGame.Load("addBuildingBtnClicked", false);
            }

            if (SaveGame.Exists("troopBuildingBtnClicked"))
            {
                troopBuildingBtnClicked = SaveGame.Load("troopBuildingBtnClicked", false);
            }

            if (SaveGame.Exists("troopBuildingClicked"))
            {
                troopBuildingClicked = SaveGame.Load("troopBuildingClicked", false);
            }

            if (SaveGame.Exists("troopSpawned"))
            {
                troopSpawned = SaveGame.Load("troopSpawned", false);
            }

            if (SaveGame.Exists("clickedSecondTime"))
            {
                clickedSecondTime = SaveGame.Load("clickedSecondTime", false);
            }

            if (SaveGame.Exists("mainBaseClicked"))
            {
                mainBaseClicked = SaveGame.Load("mainBaseClicked", false);
            }

            if (SaveGame.Exists("speedUpBtnClicked"))
            {
                speedUpBtnClicked = SaveGame.Load("speedUpBtnClicked", false);
            }

            if (SaveGame.Exists("speedUpComplete"))
            {
                speedUpComplete = SaveGame.Load("speedUpComplete", false);
            }

            if (SaveGame.Exists("freeBtnShowed"))
            {
                freeBtnShowed = SaveGame.Load("freeBtnShowed", false);
            }

            if (SaveGame.Exists("energyCollected"))
            {
                energyCollected = SaveGame.Load("energyCollected", false);
            }

            if (SaveGame.Exists("troopsSpawnedAgain"))
            {
                troopsSpawnedAgain = SaveGame.Load("troopsSpawnedAgain", false);
            }
        }

        void SaveDataOfNewLvl()
        {
            SaveGame.Save("coinsChecked", coinsChecked);
            SaveGame.Save("mainBaseUpgraded", mainBaseUpgraded);
            SaveGame.Save("enoughMoney", enoughMoney);
            SaveGame.Save("mainBaseUpComplete", mainBaseUpComplete);
            SaveGame.Save("troopBuildingUpgraded", troopBuildingUpgraded);
        }

        internal void LoadDataOfNewLvl()
        {
            if (SaveGame.Exists("coinsChecked"))
            {
                coinsChecked = SaveGame.Load("coinsChecked", false);
            }

            if (SaveGame.Exists("mainBaseUpgraded"))
            {
                mainBaseUpgraded = SaveGame.Load("mainBaseUpgraded", false);
            }

            if (SaveGame.Exists("enoughMoney"))
            {
                enoughMoney = SaveGame.Load("enoughMoney", false);
            }

            if (SaveGame.Exists("mainBaseUpComplete"))
            {
                mainBaseUpComplete = SaveGame.Load("mainBaseUpComplete", false);
            }

            if (SaveGame.Exists("troopBuildingUpgraded"))
            {
                troopBuildingUpgraded = SaveGame.Load("troopBuildingUpgraded", false);
            }

            if (Helper.GetBool(PlayerPrefsKey.TROOPBUILDINGUPCOMPLETE, false))
            {
                //adCoinButton.TweenAppear ();
                //adEnergyButton.TweenAppear ();
            }
        }
    }
}