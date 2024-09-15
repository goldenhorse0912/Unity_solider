using System.Collections.Generic;
using UnityEngine;

namespace VIRTUE {
    public class EmojiController : MonoBehaviour {
        [SerializeField]
        bool loadPositive;

        [SerializeField]
        bool loadNegative;

        List<ParticleSystem> _positiveEmojis;
        List<ParticleSystem> _negativeEmojis;

        void Start () {
            if (loadPositive) LoadPositiveEmojis ();
            if (loadNegative) LoadNegativeEmojis ();
        }

        void LoadPositiveEmojis () {
            _positiveEmojis = new List<ParticleSystem> ();
            var length = Manager.Instance.AssetManager.PositiveEmojis.Count;
            for (var i = 0; i < length; i++) {
                _positiveEmojis.Add (Instantiate (Manager.Instance.AssetManager.PositiveEmojis[i], transform));
            }
        }

        void LoadNegativeEmojis () {
            _negativeEmojis = new List<ParticleSystem> ();
            var length = Manager.Instance.AssetManager.NegativeEmojis.Count;
            for (var i = 0; i < length; i++) {
                _negativeEmojis.Add (Instantiate (Manager.Instance.AssetManager.NegativeEmojis[i], transform));
            }
        }

        public void PlayPositive () {
            _positiveEmojis.Random ().Play ();
        }

        public void PlayNegative () {
            _negativeEmojis.Random ().Play ();
        }
    }
}