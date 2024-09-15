using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    public class TroopManager : MonoBehaviour {
        [SerializeField]
        internal AddBuildingButtons addBuildingButtons;
        [SerializeField]
        GameObject[] upgradeButtons;

        [SerializeField, ReadOnly]
        internal List<Troop> playerTroops = new();
        [SerializeField, ReadOnly]
        internal List<Troop> enemyTroops = new();
        [SerializeField, ReadOnly]
        List<Building> playerBuildings = new();
        [SerializeField, ReadOnly]
        List<Building> enemyBuildings = new();

        [SerializeField, ReadOnly]
        internal AutoSpawnManager currentAutoSpawnManager;

        internal MainBaseBuilding PlayerBase, EnemyBase;
        bool _trooperInScene;

        public void ResetData () {
            playerTroops.Clear ();
            enemyTroops.Clear ();
            playerBuildings.Clear ();
            enemyBuildings.Clear ();
            addBuildingButtons.CheckButtonsToShow ();
            _trooperInScene = false;
        }

        internal void AddMeToTroopList (Troop troop, bool toPlayerList) {
            if (toPlayerList) {
                playerTroops.Add (troop);
            } else {
                enemyTroops.Add (troop);
            }
        }

        internal void RemoveMeFromTroopList (Troop troop, bool fromPlayerList) {
            if (fromPlayerList) {
                playerTroops.Remove (troop);
            } else {
                enemyTroops.Remove (troop);
            }
        }

        internal void ModifyBuildingList (Building building, bool adding, bool isPlayer) {
            if (isPlayer) {
                if (adding) {
                    playerBuildings.Add (building);
                } else {
                    playerBuildings.Remove (building);
                }
            } else {
                if (adding) {
                    enemyBuildings.Add (building);
                } else {
                    enemyBuildings.Remove (building);
                }
            }
        }

        internal Building FindClosestTargetWatchTower (Transform source, bool isPlayerTroop) {
            if (IsMainBaseDead (isPlayerTroop)) return null;
            var closestTarget = isPlayerTroop ? enemyBuildings.FindNearest (source) : playerBuildings.FindNearest (source);
            if (closestTarget is WatchTower or CheckPostBuilding) {
                return closestTarget;
            }
            return null;
        }

        internal Troop FindClosestTargetToMe (Transform source, bool isPlayerTroop) {
            if (!IsEnemyAvailable (isPlayerTroop)) {
                if (!Helper.GetBool (PlayerPrefsKey.TUTORIALCOMPLETED, false) && !isPlayerTroop) {
                    DOVirtual.DelayedCall (.5f, () => TutorialScript.Instance.MoveHandToMainBase ());
                }
                return null;
            }
            var WT = FindClosestTargetWatchTower (source, isPlayerTroop);
            var closestTarget = isPlayerTroop ? enemyTroops.FindNearest (source) : playerTroops.FindNearest (source);
            var distanceToTarget = Vector3.Distance (source.position, closestTarget.CachedTransform.position);
            if (!WT) return closestTarget;
            var WTDistance = Vector3.Distance (source.position, WT.CachedTransform.position);
            return WTDistance + 2f > distanceToTarget ? closestTarget : null;
        }

        internal Building FindClosestTargetBuilding (Transform source, bool isPlayerTroop) {
            if (IsMainBaseDead (isPlayerTroop)) return null;
            var closestTarget = isPlayerTroop ? enemyBuildings.FindNearest (source) : playerBuildings.FindNearest (source);
            return closestTarget;
        }

        bool IsEnemyAvailable (bool isPlayer) {
            if (isPlayer) {
                return enemyTroops.Count > 0;
            }
            return playerTroops.Count > 0;
        }

        internal bool IsMainBaseDead (bool isPlayer) {
            return isPlayer ? EnemyBase.Hsc.GetHealthSystem ().IsDead () : PlayerBase.Hsc.GetHealthSystem ().IsDead ();
        }

        //If troopSpawner or WatchTower exist in scene then only show baseUp and energyUp btns
        internal void CheckBuildingToShowBtn () {
            if (_trooperInScene) return;
            if (BuildingExistInScene ()) {
                _trooperInScene = true;
                foreach (var upgradeButton in upgradeButtons) {
                    upgradeButton.Show ();
                }
            } else {
                foreach (var upgradeButton in upgradeButtons) {
                    upgradeButton.Hide ();
                }
            }
        }

        internal bool BuildingExistInScene () {
            return playerBuildings.Exists (x => x.GetComponent<TroopSpawner> () || x.GetComponent<WatchTower> ());
        }
    }
}