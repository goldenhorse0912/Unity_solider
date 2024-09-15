using TMPro;
using UnityEngine;

namespace VIRTUE {
    public class DebugOptions : MonoBehaviour {
        [SerializeField]
        TextMeshProUGUI battleSceneIndexText;

        public void SetBattleScene (float index) {
            int i = (int)index;
            PlayerPrefs.SetInt (PlayerPrefsKey.BATTLE_SCENE, i);
            battleSceneIndexText.text = $"{i}";
        }

        public void SetTimeScale (float timeScale) {
            Time.timeScale = timeScale;
        }
    }
}