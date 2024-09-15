using System;
using System.Collections.Generic;
using UnityEngine;

namespace VIRTUE {
    public class UnityMainThreadDispatcher : MonoBehaviour {
        static UnityMainThreadDispatcher _instance;
        readonly Queue<Action> _actionQueue = new();

        public static UnityMainThreadDispatcher Instance {
            get {
                if (_instance == null) {
                    GameObject go = new GameObject ("UnityMainThreadDispatcher");
                    _instance = go.AddComponent<UnityMainThreadDispatcher> ();
                }
                return _instance;
            }
        }

        void Update () {
            while (_actionQueue.Count > 0) {
                _actionQueue.Dequeue ().Invoke ();
            }
            enabled = false;
        }

        public void Enqueue (Action action) {
            _actionQueue.Enqueue (action);
            enabled = true;
        }
    }
}