namespace VIRTUE {
    public abstract class PooledObject : CachedMonoBehaviour {
        public string Id { get; private set; }
        public void SetId (string id) => Id = id;
        public abstract void OnGetFromPool ();
        public abstract void OnReleaseToPool ();
        internal void OnDestroyObject () {
            Destroy (CachedGameObject);
        }

        protected virtual void Release () {
            Manager.Instance.PoolManager.ReturnToPoolById (this);
        }
    }
}