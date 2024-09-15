using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Vector3 Event", menuName = "Game Events/Vector3 Event")]
    public class Vector3Event : BaseGameEvent<Vector3> {
        public void Raise () => Raise (Vector3.zero);
    }
}