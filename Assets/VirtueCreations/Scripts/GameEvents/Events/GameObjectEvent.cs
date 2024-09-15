using UnityEngine;

namespace VIRTUE {
    [CreateAssetMenu (fileName = "New GameObject Event", menuName = "Game Events/GameObject Event")]
    public class GameObjectEvent : BaseGameEvent<GameObject> {
        //override only if necessary
        //public void Raise () => Raise ();
    }
}