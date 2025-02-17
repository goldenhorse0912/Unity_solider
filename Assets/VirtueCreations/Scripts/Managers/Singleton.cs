using JetBrains.Annotations;
using UnityEngine;

namespace VIRTUE
{
    public abstract class Singleton<T> : Singleton where T : MonoBehaviour
    {
        #region Fields

        [CanBeNull] static T _instance;
        public bool dontDestroyOnLoad;

        [NotNull]
        // ReSharper disable once StaticMemberInGenericType
        static readonly object Lock = new object();

        #endregion

        #region Properties

        [NotNull]
        public static T Instance
        {
            get
            {
                if (Quitting)
                {
                    print($"[{nameof(Singleton)}<{typeof(T)}>] Instance will not be returned because the application is quitting.");
                    // ReSharper disable once AssignNullToNotNullAttribute
                    return null;
                }

                lock (Lock)
                {
                    if (_instance != null) return _instance;
                    var instances = FindObjectsOfType<T>();
                    var count = instances.Length;
                    if (count > 0)
                    {
                        if (count == 1) return _instance = instances[0];
                        print($"[{nameof(Singleton)}<{typeof(T)}>] There should never be more than one {nameof(Singleton)} of type {typeof(T)} in the scene, but {count} were found. The first instance found will be used, and all others will be destroyed.");
                        for (var i = 1; i < instances.Length; i++) Destroy(instances[i]);
                        return _instance = instances[0];
                    }

                    print($"[{nameof(Singleton)}<{typeof(T)}>] An instance is needed in the scene and no existing instances were found, so a new instance will be created.");
                    return _instance = new GameObject($"({nameof(Singleton)}){typeof(T)}").AddComponent<T>();
                }
            }
        }

        #endregion

        #region Methods

        void Awake()
        {
            if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            OnAwake();
        }

        protected virtual void OnAwake()
        {
        }

        #endregion
    }

    public abstract class Singleton : CachedMonoBehaviour
    {
        #region Properties

        public static bool Quitting { get; private set; }

        #endregion

        #region Methods

        void OnApplicationQuit()
        {
            Quitting = true;
        }

        #endregion
    }

    //Free candy!
}