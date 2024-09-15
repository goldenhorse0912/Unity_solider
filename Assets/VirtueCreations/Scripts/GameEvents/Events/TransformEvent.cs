using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New Transform Event", menuName = "Game Events/Transform Event")]
    public class TransformEvent : BaseGameEvent<Transform> {
        //override only if necessary
        //public void Raise () => Raise ();
    }
}