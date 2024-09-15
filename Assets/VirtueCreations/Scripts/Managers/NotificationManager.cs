using UnityEngine;
using UnityEngine.Pool;

namespace VIRTUE {
    public class NotificationManager : MonoBehaviour {
        [SerializeField]
        Notification prefab;

        ObjectPool<Notification> _notificationPool;

        void Start () {
            _notificationPool = new ObjectPool<Notification> (() => {
                                                                  var pooledObjClone = Instantiate (prefab, transform);
                                                                  return pooledObjClone;
                                                              }, pooledObject => pooledObject.OnGetFromPool (),
                                                              pooledObject => pooledObject.OnReleaseToPool (),
                                                              pooledObject => pooledObject.OnDestroyObject (),
                                                              false,
                                                              5,
                                                              10);
        }

        public Notification GetFromPool () {
            return _notificationPool.Get ();
        }

        public void ReturnToPool (Notification notification) {
            _notificationPool.Release (notification);
        }
    }
}