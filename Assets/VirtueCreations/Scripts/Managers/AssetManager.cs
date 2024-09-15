using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace VIRTUE {
    public class AssetManager : MonoBehaviour {
        Dictionary<AssetReference, AsyncOperationHandle> _loadingAssetReferences;
        Dictionary<AssetLabelReference, AsyncOperationHandle> _loadingAssetLabelReferences;

        [BoxGroup ("REFERENCES")]
        [SerializeField]
        AssetReference resourceUIReference;

        [BoxGroup ("REFERENCES")]
        [SerializeField]
        AssetReference resourceObjectUIReference;

        [BoxGroup ("REFERENCES")]
        [SerializeField]
        AssetLabelReference resourceSpritesReference;

        [BoxGroup ("REFERENCES")]
        [SerializeField]
        AssetLabelReference positiveEmojiReference;

        [BoxGroup ("REFERENCES")]
        [SerializeField]
        AssetLabelReference negativeEmojiReference;

        [BoxGroup ("REFERENCES")]
        [SerializeField]
        AssetLabelReference pooledObjectsReference;

        [NonSerialized]
        [BoxGroup ("PREFABS")]
        public ResourceUI ResourceUIPrefab;

        [NonSerialized]
        [BoxGroup ("PREFABS")]
        public ResourceObjectUI ResourceObjectUIPrefab;

        [NonSerialized]
        [BoxGroup ("PREFABS")]
        public List<Sprite> ResourceSprites;

        [NonSerialized]
        [BoxGroup ("PREFABS")]
        public List<ParticleSystem> PositiveEmojis;

        [NonSerialized]
        [BoxGroup ("PREFABS")]
        public List<ParticleSystem> NegativeEmojis;

        byte _loadCount;
        int _loadingAssetReferencesCount;
        int _loadingAssetLabelReferencesCount;
        float _totalPercent;

        void Awake () {
            _loadCount = 2;
            _loadingAssetReferences = new Dictionary<AssetReference, AsyncOperationHandle> ();
            _loadingAssetLabelReferences = new Dictionary<AssetLabelReference, AsyncOperationHandle> ();
        }

        void Start () {
            LoadAsset<GameObject> (resourceUIReference, handle => {
                if (handle.Result is GameObject prefab) {
                    ResourceUIPrefab = prefab.GetComponent<ResourceUI> ();
                }
            });
            LoadAsset<GameObject> (resourceObjectUIReference, handle => {
                if (handle.Result is GameObject prefab) {
                    ResourceObjectUIPrefab = prefab.GetComponent<ResourceObjectUI> ();
                }
            });
            LoadAsset<Sprite> (resourceSpritesReference, handle => {
                if (handle.Result is IList<Sprite> sprites) {
                    ResourceSprites = new List<Sprite> (sprites.OrderBy (x => x.name));
                }
            });
            /*LoadAsset<GameObject> (positiveEmojiReference, handle => {
                if (handle.Result is IList<GameObject> emojis) {
                    PositiveEmojis = new List<ParticleSystem> (emojis.Select (x => x.GetComponent<ParticleSystem> ()));
                }
            });
            LoadAsset<GameObject> (negativeEmojiReference, handle => {
                if (handle.Result is IList<GameObject> emojis) {
                    NegativeEmojis = new List<ParticleSystem> (emojis.Select (x => x.GetComponent<ParticleSystem> ()));
                }
            });*/
            LoadAsset<GameObject> (pooledObjectsReference, FindObjectOfType<PoolManager> ().CreatePools);
            _totalPercent = _loadingAssetReferences.Count + _loadingAssetLabelReferences.Count;
            _loadingAssetReferencesCount = _loadingAssetReferences.Count;
            _loadingAssetLabelReferencesCount = _loadingAssetLabelReferences.Count;
        }

        void Update () {
            var allPercent = _loadingAssetReferencesCount + _loadingAssetLabelReferencesCount;
            Helper.LoadingValue = Mathf.Lerp (Helper.LoadingValue, (_totalPercent - allPercent) / _totalPercent, Time.deltaTime);
        }

        void LoadAsset<T> (AssetReference assetReference, Action<AsyncOperationHandle> onLoadedAction) {
            AsyncOperationHandle handle = Addressables.LoadAssetAsync<T> (assetReference);
            _loadingAssetReferences.Add (assetReference, handle);
            handle.Completed += onLoadedAction;
            handle.Completed += OnAssetLoaded;
        }

        void LoadAsset<T> (AssetLabelReference assetLabelReference, Action<AsyncOperationHandle> onLoadedAction) {
            AsyncOperationHandle handle = Addressables.LoadAssetsAsync<T> (assetLabelReference, null);
            _loadingAssetLabelReferences.Add (assetLabelReference, handle);
            handle.Completed += onLoadedAction;
            handle.Completed += OnAssetsLoaded;
        }

        void OnAssetLoaded (AsyncOperationHandle handle) {
            AssetReference assetReference = _loadingAssetReferences.FirstOrDefault (x => x.Value.Equals (handle)).Key;
            if (assetReference != null) {
                _loadingAssetReferencesCount--;
                _loadingAssetReferences.Remove (assetReference);
                if (_loadingAssetReferences.Count == 0) {
                    _loadCount--;
                    AllAssetsLoaded ();
                }
            }
        }

        void OnAssetsLoaded (AsyncOperationHandle handle) {
            AssetLabelReference assetReference = _loadingAssetLabelReferences.FirstOrDefault (x => x.Value.Equals (handle)).Key;
            if (assetReference != null) {
                _loadingAssetLabelReferencesCount--;
                _loadingAssetLabelReferences.Remove (assetReference);
                if (_loadingAssetLabelReferences.Count == 0) {
                    _loadCount--;
                    AllAssetsLoaded ();
                }
            }
        }

        void AllAssetsLoaded () {
            if (_loadCount == 0) {
                FindObjectOfType<InitializationLoader> ()?.ActivateGameScene ();
            }
        }
    }
}