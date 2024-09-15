using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using System.IO;
#endif
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace VIRTUE {
    public class PoolManager : MonoBehaviour {
        List<PooledObject> _pooledPrefabs;
        Dictionary<string, ObjectPool<PooledObject>> _pools;

        [SerializeField]
        bool check;

        [SerializeField]
        int defaultCap = 10;

        [SerializeField]
        int maxCap = 30;

        void Awake () {
            _pools = new Dictionary<string, ObjectPool<PooledObject>> ();
        }

        public void CreatePools (AsyncOperationHandle handle) {
            if (handle.Result is IList<GameObject> pooledObjects) {
                _pooledPrefabs = new List<PooledObject> (pooledObjects.Select (x => x.GetComponent<PooledObject> ()));
                CreatePools ();
            }
        }

        void CreatePools () {
            _pooledPrefabs = _pooledPrefabs.OrderBy (x => x.name).ToList ();
            var length = _pooledPrefabs.Count;
            for (byte i = 0; i < length; i++) {
                var prefab = _pooledPrefabs[i];
                var id = prefab.name.Remove (0, 3);
                var pool = new ObjectPool<PooledObject> (() => {
                                                             var pooledObjClone = Instantiate (prefab, transform);
                                                             pooledObjClone.SetId (id);
                                                             return pooledObjClone;
                                                         }, pooledObject => pooledObject.OnGetFromPool (),
                                                         pooledObject => pooledObject.OnReleaseToPool (),
                                                         pooledObject => pooledObject.OnDestroyObject (),
                                                         check,
                                                         defaultCap,
                                                         maxCap);
                _pools.Add (id, pool);
            }
            _pooledPrefabs.Clear ();
        }

        internal PooledObject GetFromPoolByName (string id) => _pools.TryGetValue (id, out var pool) ? pool.Get () : null;

        internal void ReturnToPoolById (PooledObject pooledObject) {
            if (_pools.TryGetValue (pooledObject.Id, out var pool)) {
                pool.Release (pooledObject);
            }
        }
    }
}