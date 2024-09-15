using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Color Event", menuName = "Game Events/Color Event")]
    public class ColorEvent : BaseGameEvent<Color> {
        public void Raise () => Raise (Color.white);
    }
}