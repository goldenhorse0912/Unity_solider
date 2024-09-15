using UnityEngine;

namespace VIRTUE {
    public abstract class HandAnim : MonoBehaviour {
        public abstract void PlayAnim ();
        public abstract void StopAnim ();

        public abstract void StopAnimAndHide ();
    }
}