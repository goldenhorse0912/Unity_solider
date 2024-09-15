using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    [Serializable]
    public class EnemyGroup {
        public int noOfEnemy;
        public Troop typeOfEnemy;
        public Transform spawnPoint;
        public bool isUpgrade;
        [ShowIf ("isUpgrade")]
        public int upgradeIndex;
    }

    [Serializable]
    public class EnemyWave {
        public AutoTroopSpawner autoTroopSpawner;
        public List<EnemyGroup> enemyGroup;
    }

    public class AutoTroopSpawner : Building {
        [SerializeField]
        public Transform spawnPoint;
        internal bool CanSpawn;

        bool _upgradeOne, _upgradeTwo;

        protected override void OnStart () {
            base.OnStart ();
            CanSpawn = true;
        }

        public void UpgradeBase (int index) {
            if (index == 1 && !_upgradeOne) {
                InstantShow (1);
                _upgradeOne = true;
            } else if (index == 2 && !_upgradeTwo) {
                InstantShow (2);
                _upgradeTwo = true;
            }
        }

        protected override void OnDie () {
            base.OnDie ();
            CanSpawn = false;
        }
    }
}