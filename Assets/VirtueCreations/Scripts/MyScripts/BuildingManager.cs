using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace VIRTUE {
    public class BuildingManager : MonoBehaviour {
#region Main Base Data

        [SerializeField, FoldoutGroup ("Main Base Data")]
        GrayScalableButton mainBaseUpBtn;
        [SerializeField, FoldoutGroup ("Main Base Data")]
        TextMeshProUGUI mainBaseUpCost, mainBaseLvlText;
        [SerializeField, FoldoutGroup ("Main Base Data")]
        GameObject mainBaseMaxText;
        [SerializeField, FoldoutGroup ("Main Base Data")]
        IntEvent baseUpEvent;
        [SerializeField, FoldoutGroup ("Main Base Data")]
        int[] mainBaseUpPrices;

#endregion

        [SerializeField]
        TextMeshProUGUI watchTowerCountText;

        internal int _currentWt, _totalWt;

        [SerializeField, ReadOnly]
        internal LevelBuildingData levelBuildingData;

        public void UpgradeMainBase () {
            var mainBaseData = levelBuildingData.buildingsData.Find (x => x.buildingType == BuildingType.MainBase);
            var baseBuilding = mainBaseData.refObj.GetComponent<MainBaseBuilding> ();
            if (mainBaseData.buildingLvl < baseBuilding.buildingGfx.Count - 1) {
                var coins = Manager.Instance.UIManager.GetAmountByTypeOfResource (Resource.Coin);
                var cost = mainBaseUpPrices[mainBaseData.buildingLvl + 1];
                if (coins < cost) return;
                Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, -cost);
                mainBaseData.buildingLvl++;
                baseBuilding.TweenUpgrade (mainBaseData.buildingLvl);
                baseUpEvent.Raise (mainBaseData.buildingLvl);
                CheckMainBaseData ();
            } else {
                this.Log ("Building At Max Lvl...");
            }
        }

        internal void CheckMainBaseData () {
            var mainBaseData = levelBuildingData.buildingsData.Find (x => x.buildingType == BuildingType.MainBase);
            var baseLvl = mainBaseData.buildingLvl;
            if (baseLvl < mainBaseUpPrices.Length - 1) {
                var newLvl = baseLvl + 1;
                var price = mainBaseUpPrices[newLvl];
                mainBaseUpBtn.SetCurrentCostValue (price);
                mainBaseUpCost.text = Helper.Abbreviate (price);
                mainBaseLvlText.text = $"Upgrade Base \n lv {newLvl + 1}";
                mainBaseUpBtn.ButtonAtMax (false);
                mainBaseUpCost.transform.parent.gameObject.Show ();
                mainBaseMaxText.Hide ();
            } else {
                mainBaseLvlText.text = "Max";
                mainBaseUpBtn.ButtonAtMax (true);
                mainBaseUpCost.transform.parent.gameObject.Hide ();
                mainBaseMaxText.Show ();
            }
        }

        public void UpgradeWalkingTroop (IntVariable upgradeCost) {
            var building = levelBuildingData.buildingsData.Find (x => x.buildingType == BuildingType.WalkingTroopSpawner);
            building.buildingLvl++;
            building.refObj.TweenUpgrade (building.buildingLvl);
            Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, -upgradeCost.Value);
        }

        public void UpgradeVehicleTroop (IntVariable upgradeCost) {
            var building = levelBuildingData.buildingsData.Find (x => x.buildingType == BuildingType.VehicleTroopSpawner);
            building.buildingLvl++;
            building.refObj.TweenUpgrade (building.buildingLvl);
            Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, -upgradeCost.Value);
        }

        public void UpgradeFlyingVehicleTroop (IntVariable upgradeCost) {
            var building = levelBuildingData.buildingsData.Find (x => x.buildingType == BuildingType.FlyingVehicleTroopSpawner);
            building.buildingLvl++;
            building.refObj.TweenUpgrade (building.buildingLvl);
            Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, -upgradeCost.Value);
        }

        internal static void CheckBuildings () {
            if (Manager.Instance.TroopManager.IsMainBaseDead (true)) {
                Manager.Instance.LevelManager.Result (true);
            } else if (Manager.Instance.TroopManager.IsMainBaseDead (false)) {
                Manager.Instance.LevelManager.Result (false);
            }
        }

        internal void WTroopSpawnerDead (BuildingType buildingType) {
            var building = levelBuildingData.buildingsData.Find (x => x.buildingType == buildingType);
            building.refObj = null;
            building.buildingDead.Raise ();
        }

        internal void CheckPostDead () {
            var building = levelBuildingData.buildingsData.Find (x => x.buildingType == BuildingType.CheckPost);
            building.refObj = null;
            building.buildingDead.Raise ();
        }

        internal void WatchTowerDead () {
            SetWatchTowerText (--Manager.Instance.BuildingManager._currentWt);
            var building = levelBuildingData.buildingsData.FindAll (x => x.buildingType == BuildingType.WatchTower && x.isActive);
            if (building.Count <= 0) return;
            var wt = building[^1];
            wt.refObj = null;
            wt.isActive = false;
            wt.buildingDead.Raise ();
        }

        internal void SetWatchTowerText (int amount) {
            watchTowerCountText.text = $"Watch Tower\n{amount}/{_totalWt}";
        }
    }
}