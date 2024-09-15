using System;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE
{
    public enum BuildingType
    {
        MainBase,
        WalkingTroopSpawner,
        CheckPost,
        WatchTower,
        VehicleTroopSpawner,
        FlyingVehicleTroopSpawner
    }

    [Serializable]
    public class SpawnableBuildingData<T>
    {
        [ReadOnly] public int buildingLvl;
        [ReadOnly] public bool isBuildingSpawned, isActive;
        [ReadOnly] public T refObj;

        public BuildingType buildingType;
        public GameObject buildingPrefab;
        public Transform spawnPos;
        public VoidEvent buildingDead;
    }

    public class SavableAbleBuildingData
    {
        public int BuildingLvl;
        public bool IsBuildingSpawned;
    }

    [DefaultExecutionOrder(-1)]
    public class LevelBuildingData : MonoBehaviour
    {
        [SerializeField] internal List<SpawnableBuildingData<Abst>> buildingsData;

        public List<SavableAbleBuildingData> savableAbleData = new();

        // Set in inspector
        public int levelCount;

        [SerializeField] CameraClampArea_SO cameraClampAreaSo;
        [SerializeField] VoidGameEvent victory, defeat;

        void OnEnable()
        {
            PauseEvent.AddListener(SaveData);
            defeat.OnEvent += SaveData;
            victory.OnEvent += ClearDataForThisLevel;
        }

        void OnDisable()
        {
            PauseEvent.RemoveListener(SaveData);
            defeat.OnEvent -= SaveData;
            victory.OnEvent -= ClearDataForThisLevel;
        }

        void Awake()
        {
            Manager.Instance.BuildingManager.levelBuildingData = this;
            if (Camera.main != null) Camera.main.GetComponent<CameraController>().clampArea = cameraClampAreaSo;
            LoadData();
        }

        void SaveData()
        {
            savableAbleData.Clear();
            for (var i = 0; i < buildingsData.Count; i++)
            {
                var sd = new SavableAbleBuildingData
                {
                    BuildingLvl = buildingsData[i].buildingLvl,
                    IsBuildingSpawned = buildingsData[i].isBuildingSpawned
                };
                savableAbleData.Add(sd);
            }

            SaveGame.Save(gameObject.name, savableAbleData);
        }

        void LoadData()
        {
            if (Helper.GetBool(PlayerPrefsKey.TUTORIALCOMPLETED, false))
            {
                if (SaveGame.Exists(gameObject.name))
                {
                    savableAbleData = SaveGame.Load(gameObject.name, savableAbleData);
                    for (var i = 0; i < savableAbleData.Count; i++)
                    {
                        buildingsData[i].isBuildingSpawned = savableAbleData[i].IsBuildingSpawned;
                        buildingsData[i].buildingLvl = savableAbleData[i].BuildingLvl;
                    }
                }
            }

            ResetLvl();
            TutorialScript.Instance.LoadDataOfNewLvl();
            if (levelCount == 1)
            {
                if (TutorialScript.Instance.mainBaseUpComplete && !Helper.GetBool(PlayerPrefsKey.TROOPBUILDINGUPCOMPLETE, false))
                {
                    DOVirtual.DelayedCall(2f, () =>
                    {
                        TutorialScript.Instance.ShowHand();
                        TutorialScript.Instance.InteractionWithAddBuildingBtn(false);
                        TutorialScript.Instance.InteractionWithSpeedUpBtn(false);
                        TutorialScript.Instance.InteractionWithPistolBtn(false);
                        TutorialScript.Instance.InteractionWithCheckPostBtn(false);
                        TutorialScript.Instance.InteractionWithWatchTowerBtn(false);
                        TutorialScript.PauseTime();
                        TutorialScript.Instance.mainBaseUpComplete = false;
                        TutorialScript.Instance.MoveHandToTroopBuildingForUpgrade();
                    });
                }
            }
        }

        void ResetLvl()
        {
            Manager.Instance.CameraController.ResetCamPos();
            Manager.Instance.TroopManager.ResetData();
            SpawnMainBuilding();
            if (IsBuildingSpawned(BuildingType.WalkingTroopSpawner))
            {
                SpawnWalkingTroopBuilding(true);
            }

            if (IsBuildingSpawned(BuildingType.VehicleTroopSpawner))
            {
                SpawnVehicleTroopBuilding(true);
            }

            if (IsBuildingSpawned(BuildingType.CheckPost))
            {
                SpawnCheckPost(true);
            }

            if (IsBuildingSpawned(BuildingType.FlyingVehicleTroopSpawner))
            {
                SpawnFlyingVehicleTroopBuilding(true);
            }

            var watchTowers = buildingsData.FindAll(x => x.buildingType == BuildingType.WatchTower && x.isBuildingSpawned);
            foreach (var wt in watchTowers)
            {
                wt.isActive = true;
                wt.refObj = Instantiate(wt.buildingPrefab, wt.spawnPos.position, wt.spawnPos.rotation).GetComponent<WatchTower>();
                wt.refObj.InstantShow();
            }

            Manager.Instance.BuildingManager._currentWt = watchTowers.Count;
            Manager.Instance.BuildingManager._totalWt = buildingsData.FindAll(x => x.buildingType == BuildingType.WatchTower).Count;
            Manager.Instance.BuildingManager.SetWatchTowerText(watchTowers.Count);
        }

        void ClearDataForThisLevel()
        {
            SaveGame.Delete(gameObject.name);
        }

        internal bool IsWatchTowerSpawned()
        {
            var watchTowers = buildingsData.FindAll(x => x.buildingType == BuildingType.WatchTower && x.isActive);
            return watchTowers.Count < buildingsData.FindAll(x => x.buildingType == BuildingType.WatchTower).Count;
        }

        bool IsBuildingSpawned(BuildingType buildingType)
        {
            var data = buildingsData.Find(x => x.buildingType == buildingType);
            return data is { isBuildingSpawned: true };
        }

        internal bool IsBuildingInScene(BuildingType buildingType)
        {
            var data = buildingsData.Find(x => x.buildingType == buildingType);
            return data != null ? data.refObj : false;
        }

        void SpawnMainBuilding()
        {
            var mainBaseData = buildingsData.Find(x => x.buildingType == BuildingType.MainBase);
            mainBaseData.refObj = Instantiate(mainBaseData.buildingPrefab, mainBaseData.spawnPos.position, mainBaseData.spawnPos.rotation).GetComponent<MainBaseBuilding>();
            mainBaseData.refObj.InstantShow(mainBaseData.buildingLvl);
            Manager.Instance.BuildingManager.CheckMainBaseData();
        }

        internal void SpawnWalkingTroopBuilding(bool instant)
        {
            var troopSpawner = buildingsData.Find(x => x.buildingType == BuildingType.WalkingTroopSpawner);
            troopSpawner.refObj = Instantiate(troopSpawner.buildingPrefab, troopSpawner.spawnPos.position, troopSpawner.spawnPos.rotation).GetComponent<TroopSpawner>();
            troopSpawner.isBuildingSpawned = true;
            if (instant)
            {
                troopSpawner.refObj.InstantShow(troopSpawner.buildingLvl);
            }
            else
            {
                troopSpawner.refObj.TweenUpgrade(troopSpawner.buildingLvl);
            }
        }

        internal void SpawnVehicleTroopBuilding(bool instant)
        {
            var troopSpawner = buildingsData.Find(x => x.buildingType == BuildingType.VehicleTroopSpawner);
            troopSpawner.refObj = Instantiate(troopSpawner.buildingPrefab, troopSpawner.spawnPos.position, troopSpawner.spawnPos.rotation).GetComponent<TroopSpawner>();
            troopSpawner.isBuildingSpawned = true;
            if (instant)
            {
                troopSpawner.refObj.InstantShow(troopSpawner.buildingLvl);
            }
            else
            {
                troopSpawner.refObj.TweenUpgrade(troopSpawner.buildingLvl);
            }
        }

        internal void SpawnFlyingVehicleTroopBuilding(bool instant)
        {
            var troopSpawner = buildingsData.Find(x => x.buildingType == BuildingType.FlyingVehicleTroopSpawner);
            troopSpawner.refObj = Instantiate(troopSpawner.buildingPrefab, troopSpawner.spawnPos.position, troopSpawner.spawnPos.rotation).GetComponent<TroopSpawner>();
            troopSpawner.isBuildingSpawned = true;
            if (instant)
            {
                troopSpawner.refObj.InstantShow(troopSpawner.buildingLvl);
            }
            else
            {
                troopSpawner.refObj.TweenUpgrade(troopSpawner.buildingLvl);
            }
        }

        internal void SpawnCheckPost(bool instant)
        {
            var checkPostData = buildingsData.Find(x => x.buildingType == BuildingType.CheckPost);
            checkPostData.refObj = Instantiate(checkPostData.buildingPrefab, checkPostData.spawnPos.position, checkPostData.spawnPos.rotation).GetComponent<CheckPostBuilding>();
            checkPostData.isBuildingSpawned = true;
            if (instant)
            {
                checkPostData.refObj.InstantShow();
            }
            else
            {
                checkPostData.refObj.TweenUpgrade();
            }
        }

        internal void SpawnWatchTower(bool instant, Action<bool> callback)
        {
            var watchTower = buildingsData.FindAll(x => x.buildingType == BuildingType.WatchTower && !x.isActive);
            foreach (var wt in watchTower)
            {
                wt.refObj = Instantiate(wt.buildingPrefab, wt.spawnPos.position, wt.spawnPos.rotation).GetComponent<WatchTower>();
                wt.isBuildingSpawned = true;
                wt.isActive = true;
                if (instant)
                {
                    wt.refObj.InstantShow();
                }
                else
                {
                    wt.refObj.TweenUpgrade();
                }

                Manager.Instance.BuildingManager.SetWatchTowerText(++Manager.Instance.BuildingManager._currentWt);
                break;
            }

            if (watchTower.Count > 1)
            {
                callback?.Invoke(false);
            }
            else
            {
                callback?.Invoke(true);
            }
        }
    }
}