using UnityEngine;

namespace VIRTUE {
    public class InternetConnection : MonoBehaviour {
        Canvas _canvas;

        void Awake () {
            _canvas = GetComponent<Canvas> ();
        }

        void Start () {
            CheckForInternet ();
        }

        void Update () {
            if (Time.frameCount % 30 == 0) {
                CheckForInternet ();
            }
        }

        void CheckForInternet () {
            _canvas.enabled = !Helper.InternetIsAvailable ();
            // Manager.Instance.AdManager.Init ();
        }
    }
}