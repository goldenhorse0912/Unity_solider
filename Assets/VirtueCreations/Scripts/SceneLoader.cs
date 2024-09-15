using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace VIRTUE {
    public class SceneLoader : MonoBehaviour {
        bool _isLoadingEnvironment;
        bool _isLoadingLevel;
        int _loadCount;
        static Scene _gamePlayScene;
        AsyncOperationHandle<SceneInstance> _loadingOpHandleEnvironment;
        AsyncOperationHandle<SceneInstance> _loadingOpHandleLevel;

        AssetReference _environmentToLoad;
        AssetReference _levelToLoad;
        AssetReference _currentlyLoadedEnvironment;
        AssetReference _currentlyLoadedLevel;

        [SerializeField]
        LoadEvent loadEnvironment, loadLevel;

        [SerializeField]
        VoidEvent gameSceneLoaded;

        void OnEnable () {
            loadEnvironment.OnLoadingRequested += LoadEnvironment;
            loadLevel.OnLoadingRequested += LoadLevel;
        }

        void OnDisable () {
            loadEnvironment.OnLoadingRequested -= LoadEnvironment;
            loadLevel.OnLoadingRequested -= LoadLevel;
        }

        void LoadEnvironment (AssetReference environmentToLoad) {
            if (_isLoadingEnvironment) {
                return;
            }
            _isLoadingEnvironment = true;
            _loadCount++;
            _environmentToLoad = environmentToLoad;
            StartCoroutine (UnloadPreviousEnvironment ());
        }

        void LoadLevel (AssetReference levelToLoad) {
            if (_isLoadingLevel) {
                return;
            }
            _isLoadingLevel = true;
            _loadCount++;
            _levelToLoad = levelToLoad;
            StartCoroutine (UnloadPreviousLevel ());
        }

        IEnumerator UnloadPreviousEnvironment () {
            Manager.Instance.FadeScreen.FadeOut ();
            yield return Helper.WaitFor (FadeScreen.DURATION);
            if (_currentlyLoadedEnvironment != null && _currentlyLoadedEnvironment.OperationHandle.IsValid ()) {
                var op = _currentlyLoadedEnvironment.UnLoadScene ();
                op.Completed += LoadNewEnvironment;
            } else {
                LoadNewEnvironment ();
            }
        }

        void LoadNewEnvironment (AsyncOperationHandle<SceneInstance> obj) {
            LoadNewEnvironment ();
        }

        IEnumerator UnloadPreviousLevel () {
            Manager.Instance.FadeScreen.FadeOut ();
            yield return Helper.WaitFor (FadeScreen.DURATION);
            if (_currentlyLoadedLevel != null && _currentlyLoadedLevel.OperationHandle.IsValid ()) {
                var op = _currentlyLoadedLevel.UnLoadScene ();
                op.Completed += LoadNewLevel;
            } else {
                LoadNewLevel ();
            }
        }

        void LoadNewLevel (AsyncOperationHandle<SceneInstance> obj) {
            LoadNewLevel ();
        }

        void LoadNewEnvironment () {
            /*if (_environmentToLoad == null) {
                StartCoroutine (ActivateGamePlayScene ());
                return;
            }*/
            _loadingOpHandleEnvironment = _environmentToLoad.LoadSceneAsync (LoadSceneMode.Additive);
            _loadingOpHandleEnvironment.Completed += OnNewEnvironmentLoaded;
        }

        void LoadNewLevel () {
            _loadingOpHandleLevel = _levelToLoad.LoadSceneAsync (LoadSceneMode.Additive);
            _loadingOpHandleLevel.Completed += OnNewLevelLoaded;
        }

        void OnNewEnvironmentLoaded (AsyncOperationHandle<SceneInstance> obj) {
            //Save loaded scenes (to be unloaded at next load request)
            _loadCount--;
            _currentlyLoadedEnvironment = _environmentToLoad;
            OnEverythingLoaded ();
            // SceneManager.SetActiveScene (obj.Result.Scene);
            _isLoadingEnvironment = false;
        }

        void OnNewLevelLoaded (AsyncOperationHandle<SceneInstance> obj) {
            //Save loaded scenes (to be unloaded at next load request)
            _loadCount--;
            _currentlyLoadedLevel = _levelToLoad;
            OnEverythingLoaded ();
            SceneManager.SetActiveScene (obj.Result.Scene);
            _isLoadingLevel = false;
        }

        void OnEverythingLoaded () {
            if (_loadCount == 0) {
                gameSceneLoaded.Raise ();
                Manager.Instance.FadeScreen.FadeIn ();
            }
        }

        /*IEnumerator ActivateGamePlayScene () {
            _isLoading = false;
            _currentlyLoadedLevel = null;
            SceneManager.SetActiveScene (_gamePlayScene);
            yield return Helper.WaitFor (FadeScreen.DURATION);
            gameSceneLoaded.Raise ();
            Manager.Instance.FadeScreen.FadeIn ();
        }*/

        public static void SetGamePlayScene (Scene gamePlayScene) {
            _gamePlayScene = gamePlayScene;
        }
    }
}