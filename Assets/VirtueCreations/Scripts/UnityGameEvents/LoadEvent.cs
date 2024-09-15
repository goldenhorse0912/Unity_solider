using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Load Event", menuName = "Unity Game Events/Create New Loading Event")]
    public class LoadEvent : ScriptableObject {
        public UnityAction<AssetReference> OnLoadingRequested;

        public void Raise (AssetReference sceneReference) {
            OnLoadingRequested?.Invoke (sceneReference);
        }
    }
}