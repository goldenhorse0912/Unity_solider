using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

namespace VIRTUE {
    public class InitializationLoader : MonoBehaviour {
        [SerializeField]
        AssetReference managersScene;

        [SerializeField]
        AssetReference gameScene;

        void Awake () {
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            Application.backgroundLoadingPriority = ThreadPriority.Low;
            DOTween.SetTweensCapacity (500, 50);
        }

        void Start () {
            managersScene.LoadSceneAsync (LoadSceneMode.Additive);
        }

        void OnGameSceneLoaded (AsyncOperationHandle<SceneInstance> obj) {
            SceneManager.SetActiveScene (obj.Result.Scene);
            SceneLoader.SetGamePlayScene (obj.Result.Scene);
        }

        public void ActivateGameScene () {
            var loadingOperationHandle = gameScene.LoadSceneAsync (LoadSceneMode.Additive, true, 0);
            loadingOperationHandle.Completed += OnGameSceneLoaded;
            SceneManager.UnloadSceneAsync (0);
        }
    }
}