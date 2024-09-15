using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    public class ResourceManager : MonoBehaviour {
        public static ResourceManager Instance;

        [SerializeField]
        Transform coinUiTransform;
        [SerializeField]
        Transform canvasTransform;

        [ValueDropdown ("@VIRTUE.PooledObjectIds.PoolIds")]
        [SerializeField]
        string resourceId;
        [SerializeField]
        Transform coinUiPrefab;

        Camera _mainCamera;

        void Awake () {
            Instance = this;
            _mainCamera = Camera.main;
        }

        internal void SpawnCoinAt (Vector3 pos, int collectibleValue) {
            const float radius = 2f;
            var coin = Manager.Instance.PoolManager.GetFromPoolByName (resourceId).transform;
            coin.position = pos;
            var jumpPos = pos + Vector3.right * Random.Range (-radius, radius) + Vector3.forward * Random.Range (-radius, radius);
            coin.GetComponent<Collectible> ().JumpAt (jumpPos, collectibleValue);
        }

        internal void SpawnCoinUiAt (Transform targetPos, int collectibleValue) {
            var icon = Instantiate (coinUiPrefab, canvasTransform);
            var screenPoint = _mainCamera.WorldToScreenPoint (targetPos.position);
            icon.position = screenPoint;
            icon.DOMove (coinUiTransform.position, 1500f).SetUpdate (true).SetEase (Ease.InBack).SetSpeedBased ().OnComplete (() => {
                Manager.Instance.UIManager.AddAmountByTypeOfResource (Resource.Coin, collectibleValue);
                Destroy (icon.gameObject);
            });
        }
    }
}