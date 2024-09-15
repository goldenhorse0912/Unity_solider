using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace VIRTUE {
    public class AddBuildingButtons : MonoBehaviour {
        [SerializeField, FoldoutGroup ("WTroop")]
        GrayScalableButton wTroopButton;
        [SerializeField, FoldoutGroup ("WTroop")]
        GameObject wTroopCoinText, wTroopMaxText;

        [SerializeField, FoldoutGroup ("VehicleTroop")]
        GrayScalableButton vehicleTroopButton;
        [SerializeField, FoldoutGroup ("VehicleTroop")]
        GameObject vehicleTroopCoinText, vehicleTroopMaxText;

        [SerializeField, FoldoutGroup ("FlyingVehicleTroop")]
        GrayScalableButton flyingVehicleTroopButton;
        [SerializeField, FoldoutGroup ("FlyingVehicleTroop")]
        GameObject flyingVehicleTroopCoinText, flyingVehicleTroopMaxText;

        [SerializeField, FoldoutGroup ("CheckPost")]
        GrayScalableButton checkPostButton;
        [SerializeField, FoldoutGroup ("CheckPost")]
        GameObject checkPostCoinText, checkPostMaxText;

        [SerializeField, FoldoutGroup ("WatchTower")]
        GrayScalableButton watchTowerButton;
        [SerializeField, FoldoutGroup ("WatchTower")]
        GameObject watchTowerCoinText, watchTowerMaxText;

        [SerializeField]
        GameObject snowParticle;

        void OnEnable () {
            // check if building is in scene to toggle max on button
            ToggleWTroopBtnToMax (Manager.Instance.BuildingManager.levelBuildingData.IsBuildingInScene (BuildingType.WalkingTroopSpawner));
            ToggleVehicleTroopBtnToMax (Manager.Instance.BuildingManager.levelBuildingData.IsBuildingInScene (BuildingType.VehicleTroopSpawner));
            ToggleFlyingVehicleTroopBtnToMax (Manager.Instance.BuildingManager.levelBuildingData.IsBuildingInScene (BuildingType.FlyingVehicleTroopSpawner));
            ToggleCheckPostBtnToMax (Manager.Instance.BuildingManager.levelBuildingData.IsBuildingInScene (BuildingType.CheckPost));
            ToggleWatchTowerBtnMax (!Manager.Instance.BuildingManager.levelBuildingData.IsWatchTowerSpawned ());
            MainButtons.Instance.RefreshAllButtons ();
        }

        public void SpawnWTroopBuilding (IntVariable cost) {
            if (MainButtons.GameOver) return;
            ToggleWTroopBtnToMax (true);
            Manager.Instance.BuildingManager.levelBuildingData.SpawnWalkingTroopBuilding (false);
            Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, -cost.Value);
        }

        public void SpawnVehicleTroopBuilding (IntVariable cost) {
            if (MainButtons.GameOver) return;
            ToggleVehicleTroopBtnToMax (true);
            Manager.Instance.BuildingManager.levelBuildingData.SpawnVehicleTroopBuilding (false);
            Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, -cost.Value);
        }

        public void SpawnFlyingVehicleTroopBuilding (IntVariable cost) {
            if (MainButtons.GameOver) return;
            ToggleFlyingVehicleTroopBtnToMax (true);
            Manager.Instance.BuildingManager.levelBuildingData.SpawnFlyingVehicleTroopBuilding (false);
            Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, -cost.Value);
        }

        public void SpawnCheckPost (IntVariable cost) {
            if (MainButtons.GameOver) return;
            ToggleCheckPostBtnToMax (true);
            Manager.Instance.BuildingManager.levelBuildingData.SpawnCheckPost (false);
            Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, -cost.Value);
        }

        public void SpawnWatchTower (IntVariable cost) {
            if (MainButtons.GameOver) return;
            Manager.Instance.BuildingManager.levelBuildingData.SpawnWatchTower (false, ToggleWatchTowerBtnMax);
            Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, -cost.Value);
        }

        public void ToggleWTroopBtnToMax (bool state) {
            if (state) {
                wTroopButton.ButtonAtMax (true);
                wTroopCoinText.Hide ();
                wTroopMaxText.Show ();
            } else {
                wTroopButton.ButtonAtMax (false);
                wTroopCoinText.Show ();
                wTroopMaxText.Hide ();
            }
        }

        public void ToggleVehicleTroopBtnToMax (bool state) {
            if (state) {
                vehicleTroopButton.ButtonAtMax (true);
                vehicleTroopCoinText.Hide ();
                vehicleTroopMaxText.Show ();
            } else {
                vehicleTroopButton.ButtonAtMax (false);
                vehicleTroopCoinText.Show ();
                vehicleTroopMaxText.Hide ();
            }
        }

        public void ToggleFlyingVehicleTroopBtnToMax (bool state) {
            if (state) {
                flyingVehicleTroopButton.ButtonAtMax (true);
                flyingVehicleTroopCoinText.Hide ();
                flyingVehicleTroopMaxText.Show ();
            } else {
                flyingVehicleTroopButton.ButtonAtMax (false);
                flyingVehicleTroopCoinText.Show ();
                flyingVehicleTroopMaxText.Hide ();
            }
        }

        public void ToggleCheckPostBtnToMax (bool state) {
            if (state) {
                checkPostButton.ButtonAtMax (true);
                checkPostCoinText.Hide ();
                checkPostMaxText.Show ();
            } else {
                checkPostButton.ButtonAtMax (false);
                checkPostCoinText.Show ();
                checkPostMaxText.Hide ();
            }
        }

        public void ToggleWatchTowerBtnMax (bool state) {
            if (state) {
                watchTowerButton.ButtonAtMax (true);
                watchTowerCoinText.Hide ();
                watchTowerMaxText.Show ();
            } else {
                watchTowerButton.ButtonAtMax (false);
                watchTowerCoinText.Show ();
                watchTowerMaxText.Hide ();
            }
        }

        internal void CheckButtonsToShow () {
            HideAll ();
            var levelCount = Manager.Instance.BuildingManager.levelBuildingData.levelCount;
            switch (levelCount) {
                case 0:
                    wTroopButton.gameObject.Show ();
                    checkPostButton.gameObject.Show ();
                    break;

                case 1:
                    wTroopButton.gameObject.Show ();
                    checkPostButton.gameObject.Show ();
                    watchTowerButton.gameObject.Show ();
                    break;

                case 2:
                    snowParticle.Show ();
                    wTroopButton.gameObject.Show ();
                    checkPostButton.gameObject.Show ();
                    watchTowerButton.gameObject.Show ();
                    vehicleTroopButton.gameObject.Show ();
                    break;

                case 3:
                    wTroopButton.gameObject.Show ();
                    checkPostButton.gameObject.Show ();
                    watchTowerButton.gameObject.Show ();
                    vehicleTroopButton.gameObject.Show ();
                    flyingVehicleTroopButton.gameObject.Show ();
                    break;
            }

            void HideAll () {
                snowParticle.Hide ();
                wTroopButton.gameObject.Hide ();
                checkPostButton.gameObject.Hide ();
                watchTowerButton.gameObject.Hide ();
                vehicleTroopButton.gameObject.Hide ();
                flyingVehicleTroopButton.gameObject.Hide ();
            }
        }
    }
}