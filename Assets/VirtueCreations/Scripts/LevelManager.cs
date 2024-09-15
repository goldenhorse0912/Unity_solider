using System.Collections;
using BayatGames.SaveGameFree;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace VIRTUE
{
    public class LevelManager : MonoBehaviour
    {
        internal bool _resultDeclared;

        public int levelCount;

        public int levelIndex;
        //public int environmentIndex;

        static bool FileExists => SaveGame.Exists(FileName);
        static string FileName => $"LevelManager{FileExtensions.JSON}";

        [SerializeField] LoadEvent loadEnvironment, loadLevel;

        [SerializeField] VoidGameEvent victory, defeat;

        [SerializeField] AssetReference victoryScene, defeatScene;

        [SerializeField] AssetReference[] levels;

        [SerializeField] AssetReference[] environments;

        AsyncOperationHandle<SceneInstance> _loadingOpHandle;
        AssetReference _sceneToLoad, _currentlyLoadedScene;

        public TextMeshProUGUI levelText;

        #region UNITY_CALLBACKS

        void OnEnable()
        {
            PauseEvent.AddListener(SaveData);
            victory.OnEvent += Victory;
            defeat.OnEvent += Defeat;
        }

        void OnDisable()
        {
            PauseEvent.RemoveListener(SaveData);
            victory.OnEvent -= Victory;
            defeat.OnEvent -= Defeat;
        }

        void Start()
        {
            LoadData();
        }

        #endregion

        #region PRIVATE_CALLBACKS

        void LoadData()
        {
            if (FileExists)
            {
                var data = SaveGame.Load<(int lvlIndex, int envIndex, int levelCount)>(FileName);
                levelIndex = data.lvlIndex;
                levelCount = data.levelCount;

                //environmentIndex = data.envIndex;
            }

            levelText.text = $"Level  {levelCount}";

            LoadLevel(levelIndex);
            // LoadEnvironment (environmentIndex);
            LoadEnvironment(levelIndex);

            DOVirtual.DelayedCall(2, () =>
            {
                Debug.Log("==================>Level Started");
                Manager.Instance.GameController.LogLevelStartedEvent(levelCount);
            });
        }

        void SaveData()
        {
            (int lvlIndex, int envIndex, int levelCount) data = (levelIndex, levelIndex, levelCount);
            SaveGame.Save(FileName, data);
        }

        void Victory()
        {
            _sceneToLoad = victoryScene;
            _loadingOpHandle = victoryScene.LoadSceneAsync(LoadSceneMode.Additive);
            _loadingOpHandle.Completed += OnNewSceneLoaded;
        }

        void Defeat()
        {
            _sceneToLoad = defeatScene;
            _loadingOpHandle = defeatScene.LoadSceneAsync(LoadSceneMode.Additive);
            _loadingOpHandle.Completed += OnNewSceneLoaded;
        }

        #endregion

        #region PUBLIC_CALLBACKS

        public void Result(bool state)
        {
            if (_resultDeclared)
            {
                return;
            }

            _resultDeclared = true;
            MainButtons.Instance.StopGeneratingEnergy();
            TutorialScript.Instance.HideAdButtons();
            if (state)
            {
                OnVictory();
            }
            else
            {
                OnDefeat();
            }
        }

        public void OnVictory()
        {
            Manager.Instance.GameController.LogLevelCompleteEvent(levelCount);

            levelCount++;
            levelIndex++;
            levelText.text = $"Level {levelCount}";
            if (levelIndex == levels.Length)
            {
                levelIndex = 1;
            }

            DOVirtual.DelayedCall(1f, victory.Raise);
        }

        public void OnDefeat()
        {
            Manager.Instance.GameController.LogLevelFailedEvent(levelCount);

            DOVirtual.DelayedCall(1f, defeat.Raise);
        }

        public void OnResultButtonClicked()
        {
            LoadLevel(levelIndex);
        }

        public void LoadLevel(int index)
        {
            loadLevel.Raise(levels[index]);
        }

        public void LoadEnvironment(int index)
        {
            loadEnvironment.Raise(environments[index]);
        }

        void OnNewSceneLoaded(AsyncOperationHandle<SceneInstance> obj)
        {
            //Save loaded scenes (to be unloaded at next load request)
            _currentlyLoadedScene = _sceneToLoad;
            // SceneManager.SetActiveScene (obj.Result.Scene);
            Manager.Instance.GameController.LogLevelStartedEvent(levelCount);
            print("--------------------->LogLevelStartedEvent");
        }

        public void UnloadPreviousScene()
        {
            LoadLevel(levelIndex);
            LoadEnvironment(levelIndex);
            _resultDeclared = false;
            StartCoroutine(Unload());

            IEnumerator Unload()
            {
                yield return Helper.WaitFor(FadeScreen.DURATION);
                if (_currentlyLoadedScene != null && _currentlyLoadedScene.OperationHandle.IsValid())
                {
                    _currentlyLoadedScene.UnLoadScene();
                }
            }
        }

        #endregion
    }
}