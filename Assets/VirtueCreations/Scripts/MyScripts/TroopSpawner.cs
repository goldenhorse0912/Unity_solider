using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace VIRTUE {
    [Serializable]
    public class TroopData {
        public Troop troop;
        public IntVariable cost;
    }

    public class TroopSpawner : Building {
        [SerializeField]
        BuildingType buildingType;

        [SerializeField]
        Transform[] spawnPoint;
        [SerializeField]
        List<TroopData> spawnableTroops;

        protected override void OnDie () {
            base.OnDie ();
            Manager.Instance.BuildingManager.WTroopSpawnerDead (buildingType);
        }

        protected override void OnDamage () {
            base.OnDamage ();
            if (!Helper.GetBool (PlayerPrefsKey.TUTORIALCOMPLETED, false)) {
                // show free btn and move and there
            }
        }

        public void SpawnTroop (int index) {
            if (Hsc.GetHealthSystem ().IsDead ()) return;
            var troopData = spawnableTroops[index];
            if (MainButtons.Instance.EnergyAmount >= troopData.cost.Value) {
                MainButtons.Instance.ChangeEnergyAmount (-troopData.cost.Value);
                Manager.Instance.AudioManager.PlayHaptic ();
                var troop = Instantiate (troopData.troop);
                Vector3 pos;
                if (spawnPoint.Length > 1) {
                    pos = spawnPoint[index].position;
                } else {
                    pos = spawnPoint[0].position + (Vector3.right * Random.Range (-1f, 1f) + (Vector3.forward * Random.Range (0, 1f)));
                }
                troop.CachedTransform.position = pos;
                troop.isPlayer = isPlayer;
            }
        }
    }
}