using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using Lofelt.NiceVibrations;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
using Sirenix.Utilities.Editor;
#endif

namespace VIRTUE {
    public sealed class AudioManager : MonoBehaviour {
        [SerializeField]
        Button soundOnBtn, soundOffBtn;

        [SerializeField]
        Button hapticsOnBtn, hapticsOffBtn;

        [ListDrawerSettings (OnTitleBarGUI = "GenerateConstants")]
        [SerializeField]
        AudioSource[] sounds;

#if UNITY_EDITOR
        void OnValidate () {
            if (TryGetComponent (out HapticReceiver hr)) {
                hr.hapticsEnabled = !hr.hapticsEnabled;
            }
        }

        void GenerateConstants () {
            if (SirenixEditorGUI.ToolbarButton (EditorIcons.Refresh)) {
                sounds = GetComponentsInChildren<AudioSource> ();
                Helper.SetDirty (this);
                if (sounds.Any ()) {
                    var so = new SerializedObject (Selection.activeGameObject.GetComponent<AudioManager> ());
                    var itemsProp = so.FindProperty ("sounds");
                    var resourceKeys = new List<KeyValuePair<object, object>> ();
                    for (byte i = 0; i < itemsProp.arraySize; i++) {
                        var element = itemsProp.GetArrayElementAtIndex (i);
                        var clipName = element.objectReferenceValue.name;
                        var key = $"Clip_{clipName}";
                        resourceKeys.Add (new KeyValuePair<object, object> (key, i));
                    }
                    if (resourceKeys.Count > 0) {
                        Helper.GenerateConstantsClass ("VIRTUE.SoundConstants", "byte", resourceKeys);
                    } else {
                        Helper.DisplayDialogue ("Constants Class Generation", "Please fill in required information for all items.", "Ok");
                    }
                }
            }
        }
#endif

        void Awake () {
            //load haptics settings
            if (TryGetComponent (out HapticReceiver hr)) {
                var hapticsState = Helper.GetBool (PlayerPrefsKey.HAPTICS, true);
                hr.hapticsEnabled = hapticsState;
                hapticsOnBtn.gameObject.SetActive (hapticsState);
                hapticsOffBtn.gameObject.SetActive (!hapticsState);
            }

            //load audio settings
            var volume = PlayerPrefs.GetInt (PlayerPrefsKey.AUDIO, 1);
            AudioListener.volume = volume;
            soundOnBtn.gameObject.SetActive (volume == 1);
            soundOffBtn.gameObject.SetActive (volume == 0);
            _vibrateIsReady = true;
        }

        void Start () {
            hapticsOnBtn.onClick.AddListener (ToggleHaptics);
            hapticsOffBtn.onClick.AddListener (ToggleHaptics);
            soundOnBtn.onClick.AddListener (ToggleAudio);
            soundOffBtn.onClick.AddListener (ToggleAudio);
        }

        void ToggleHaptics () {
            if (TryGetComponent (out HapticReceiver hr)) {
                hr.hapticsEnabled = !hr.hapticsEnabled;
                Helper.SetBool (PlayerPrefsKey.HAPTICS, hr.hapticsEnabled);
                hapticsOnBtn.gameObject.SetActive (hr.hapticsEnabled);
                hapticsOffBtn.gameObject.SetActive (!hr.hapticsEnabled);
            }
        }

        void ToggleAudio () {
            var volume = (int)AudioListener.volume == 1 ? 0 : 1;
            AudioListener.volume = volume;
            PlayerPrefs.SetInt (PlayerPrefsKey.AUDIO, volume);
            soundOnBtn.gameObject.SetActive (volume == 1);
            soundOffBtn.gameObject.SetActive (volume == 0);
        }

        public void Play (byte id) {
            sounds[id].Play ();
        }

        public void Stop (byte id) {
            sounds[id].Stop ();
        }

        public void PlayIfNotPlaying (byte id) {
            sounds[id].PlayIfNotPlaying ();
        }

        public void PlayHaptic () {
            if (_vibrateIsReady) {
                _vibrateIsReady = false;
                HapticPatterns.PlayPreset (HapticPatterns.PresetType.Selection);
                PrepareVibration ();
            }
        }

        bool _vibrateIsReady;

        async void PrepareVibration () {
            await Task.Delay (0.1f.ToMilliseconds ());
            _vibrateIsReady = true;
        }
    }
}