using UnityEngine;

namespace Flexalon
{
    /// <summary>
    /// Base type for many Flexalon components. Deals with FlexalonNode lifecycle,
    /// and provides the ForceUpdate and MarkDirty methods to trigger a Flexalon update.
    /// </summary>
    public abstract class FlexalonComponent : MonoBehaviour
    {
        protected FlexalonNode _node;

        /// <summary> The FlexalonNode associated with this gameObject. </summary>
        public FlexalonNode Node => _node;

        [SerializeField, HideInInspector]
        private bool _initialized;

        void Update()
        {
            DoUpdate();
        }

        void OnEnable()
        {
            _node = Flexalon.GetOrCreateNode(gameObject);

            DoOnEnable();

            if (!_node.HasResult || !_initialized)
            {
                _initialized = true;
                MarkDirty();
            }
            else
            {
                UpdateProperties();
            }
        }

        void OnDisable()
        {
            DoOnDisable();
        }

        void OnDestroy()
        {
            if (_node != null)
            {
                ResetProperties();
                _node.MarkDirty();
                _node.Detach();
                _node = null;
            }
        }

        /// <summary> Marks this component needing an update. The Flexalon singleton
        /// will visit it in dependency order on LateUpdate. </summary>
        public void MarkDirty()
        {
            if (_node != null)
            {
                UpdateProperties();
                _node.MarkDirty();
            }
        }

        /// <summary> Forces this component, its parent nodes, and its children nodes to update immediately. </summary>
        public void ForceUpdate()
        {
            _node = Flexalon.GetOrCreateNode(gameObject);
            MarkDirty();
            _node.ForceUpdate();
        }

        void OnDidApplyAnimationProperties()
        {
            MarkDirty();
        }

        /// <summary> Called when the component is enabled to apply properties to the FlexalonNode. </summary>
        protected virtual void UpdateProperties() {}

        /// <summary> Called when the component is destroyed to reset properties on the FlexalonNode. </summary>
        protected virtual void ResetProperties() {}

        /// <summary> Called when the component is enabled. </summary>
        protected virtual void DoOnEnable() {}

        /// <summary> Called when the component is disabled. </summary>
        protected virtual void DoOnDisable() {}

        /// <summary> Called when the component is updated. </summary>
        public virtual void DoUpdate() {}
    }
}