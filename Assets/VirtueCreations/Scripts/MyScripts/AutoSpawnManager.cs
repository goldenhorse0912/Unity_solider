using System;
using System.Collections;
using System.Collections.Generic;
using BayatGames.SaveGameFree;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VIRTUE
{
    [Serializable]
    public class EnemyParentWave
    {
        public List<EnemyWave> enemyWaves;
        // public float waveDelay;
    }

    public class AutoSpawnManager : MonoBehaviour
    {
        [SerializeField] VoidGameEvent victory;

        public List<EnemyParentWave> enemyParentWaves;
        [SerializeField] float delayBetweenWaves = 8f;
        [SerializeField] int wavesToSubtractFromProgress;

        int _waveCount, _rewardCountIndex;


        void Awake()
        {
            Manager.Instance.TroopManager.currentAutoSpawnManager = this;
            LoadData();
        }

        void OnEnable()
        {
            victory.OnEvent += ClearDataForThisLevel;
        }

        void OnDisable()
        {
            victory.OnEvent -= ClearDataForThisLevel;
        }

        void ClearDataForThisLevel()
        {
            SaveGame.Delete(gameObject.name + "_WaveCount");
        }

        void Start()
        {
            SetCustomWaveIndex();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine(SpawnEnemies());
            }
        }

        void SetCustomWaveIndex()
        {
            _waveCount -= wavesToSubtractFromProgress;
            if (_waveCount <= 0)
            {
                _waveCount = 0;
            }
        }

        internal IEnumerator SpawnEnemies()
        {
            var enemyParentWave = enemyParentWaves[_waveCount];

            if (Manager.Instance.LevelManager._resultDeclared)
            {
                yield break;
            }

            if (!Helper.GetBool(PlayerPrefsKey.TROOPBUILDINGUPCOMPLETE, false) && Manager.Instance.BuildingManager.levelBuildingData.levelCount == 1 && _waveCount >= 3)
            {
                TutorialScript.Instance.CheckUserCoins();
            }

            foreach (var eb in enemyParentWave.enemyWaves)
            {
                if (eb.autoTroopSpawner != null && eb.autoTroopSpawner.CanSpawn)
                {
                    foreach (var eg in eb.enemyGroup)
                    {
                        if (eg.isUpgrade)
                        {
                            eb.autoTroopSpawner.UpgradeBase(eg.upgradeIndex);
                        }

                        for (var i = 0; i < eg.noOfEnemy; i++)
                        {
                            if (eg.spawnPoint == null)
                            {
                                var pos = eb.autoTroopSpawner.spawnPoint.position + (Vector3.right * Random.Range(-1f, 1f));
                                Instantiate(eg.typeOfEnemy, pos, eb.autoTroopSpawner.spawnPoint.rotation);
                                yield return null;
                            }
                            else
                            {
                                var pos = eg.spawnPoint.position;
                                Instantiate(eg.typeOfEnemy, pos, eg.spawnPoint.rotation);
                                yield return null;
                            }
                        }
                    }
                }
            }

            _waveCount++;
            SaveData();
            _rewardCountIndex++;
            if (_waveCount < enemyParentWaves.Count)
            {
                //StartCoroutine(SpawnEnemies());
            }
            else
            {
                _waveCount = enemyParentWaves.Count / 2;
                //StartCoroutine(SpawnEnemies());
            }
        }

        internal int GetReward()
        {
            return MainButtons.Instance.GetRewardAccordingToWave(_rewardCountIndex);
        }

        void SaveData()
        {
            SaveGame.Save(gameObject.name + "_WaveCount", _waveCount);
        }

        void LoadData()
        {
            if (SaveGame.Exists(gameObject.name + "_WaveCount"))
            {
                _waveCount = SaveGame.Load(gameObject.name + "_WaveCount", _waveCount);
            }
        }
    }
}