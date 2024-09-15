using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Vector2 Event", menuName = "Game Events/Vector2 Event")]
    public class Vector2Event : BaseGameEvent<Vector2> {
        public void Raise () => Raise (Vector2.zero);
    }
}