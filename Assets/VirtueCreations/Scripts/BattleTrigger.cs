using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace VIRTUE {
    public class BattleTrigger : MonoBehaviour {
        const int REPEAT_BATTLE_INDEX = 3;

        AsyncOperationHandle<SceneInstance> _loadingOpHandle;
        AssetReference _sceneToLoad, _currentlyLoadedScene;

        [SerializeField]
        LoadEvent loadLocation;

        [SerializeField]
        VoidGameEvent enterBattle, victory, defeat;

        [SerializeField]
        AssetReference victoryScene, defeatScene;

        [SerializeField]
        AssetReference[] battleScenes;

        void OnEnable () {
            enterBattle.OnEvent += EnterBattle;
            victory.OnEvent += Victory;
            defeat.OnEvent += Defeat;
        }

        void OnDisable () {
            enterBattle.OnEvent -= EnterBattle;
            victory.OnEvent -= Victory;
            defeat.OnEvent -= Defeat;
        }

        void OnTriggerEnter (Collider other) {
            if (other.CompareTag (Tags.PLAYER)) {
                Manager.Instance.PlayerController.ToPyramid ();
                Manager.Instance.CameraFollow.ToPyramid (transform);
            }
        }

        void EnterBattle () {
            var index = PlayerPrefs.GetInt (PlayerPrefsKey.BATTLE_SCENE, 0);
            loadLocation.Raise (battleScenes[index]);
        }

        void SetNextBattleScene () {
            var index = PlayerPrefs.GetInt (PlayerPrefsKey.BATTLE_SCENE, 0);
            index++;
            if (index == battleScenes.Length) {
                index = REPEAT_BATTLE_INDEX;
            }
            PlayerPrefs.SetInt (PlayerPrefsKey.BATTLE_SCENE, index);
        }

        void Victory () {
            SetNextBattleScene ();
            _sceneToLoad = victoryScene;
            _loadingOpHandle = victoryScene.LoadSceneAsync (LoadSceneMode.Additive);
            _loadingOpHandle.Completed += OnNewSceneLoaded;
        }

        void Defeat () {
            _sceneToLoad = defeatScene;
            _loadingOpHandle = defeatScene.LoadSceneAsync (LoadSceneMode.Additive);
            _loadingOpHandle.Completed += OnNewSceneLoaded;
        }

        void OnNewSceneLoaded (AsyncOperationHandle<SceneInstance> obj) {
            //Save loaded scenes (to be unloaded at next load request)
            _currentlyLoadedScene = _sceneToLoad;
            // SceneManager.SetActiveScene (obj.Result.Scene);
        }

        public void UnloadPreviousScene () {
            loadLocation.Raise (null);
            StartCoroutine (Unload ());

            IEnumerator Unload () {
                yield return Helper.WaitFor (FadeScreen.DURATION);
                if (_currentlyLoadedScene != null && _currentlyLoadedScene.OperationHandle.IsValid ()) {
                    _currentlyLoadedScene.UnLoadScene ();
                }
            }
        }
    }
}