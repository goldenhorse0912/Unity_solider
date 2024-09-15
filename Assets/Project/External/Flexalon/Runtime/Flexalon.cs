using System.Collections.Generic;
using UnityEngine;

namespace Flexalon
{
    /// <summary>
    /// Singleton class which tracks and updates all FlexalonNodes in the scene.
    /// See [core concepts](/docs/coreConcepts) for more information.
    /// </summary>
    [ExecuteAlways, HelpURL("https://www.flexalon.com/docs/coreConcepts")]
    public class Flexalon : MonoBehaviour
    {
        [SerializeField]
        private bool _updateInEditMode = true;
        /// <summary> Determines if Flexalon should automatically update in edit mode. </summary>
        public bool UpdateInEditMode
        {
            get { return _updateInEditMode; }
            set { _updateInEditMode = value; }
        }

        [SerializeField]
        private bool _updateInPlayMode = true;
        /// <summary> Determines if Flexalon should automatically update in play mode. </summary>
        public bool UpdateInPlayMode
        {
            get { return _updateInPlayMode; }
            set { _updateInPlayMode = value; }
        }

        [SerializeField]
        private GameObject _inputProvider = null;
        private InputProvider _input;
        /// <summary>
        /// Override the default InputProvider used by FlexalonInteractables to support other input devices.
        /// </summary>
        public InputProvider InputProvider {
            get => _input;
            set => _input = value;
        }

        /// <summary>
        /// Set of nodes representing GameObjects tracked by Flexalon.
        /// </summary>
        public IReadOnlyCollection<FlexalonNode> Nodes => _nodes;

        private static Flexalon _instance;

        private HashSet<Node> _nodes = new HashSet<Node>();
        private Dictionary<GameObject, Node> _gameObjects = new Dictionary<GameObject, Node>();
        private DefaultTransformUpdater _defaultTransformUpdater = new DefaultTransformUpdater();
        private HashSet<Node> _roots = new HashSet<Node>();
        private static Vector3 defaultSize = Vector3.one;
        private List<GameObject> _destroyed = new List<GameObject>();

        internal bool RecordFrameChanges = false;

        /// <summary> Event invoked before Flexalon updates. </summary>
        public System.Action PreUpdate;

        /// <summary> Returns the singleton Flexalon component. </summary>
        /// <returns> The singleton Flexalon component, or null if it doesn't exist. </returns>
        public static Flexalon Get()
        {
            return _instance;
        }

        /// <summary> Returns the singleton Flexalon component, or creates one if it doesn't exist. </summary>
        /// <returns> The singleton Flexalon component. </returns>
        public static Flexalon GetOrCreate()
        {
            if (!_instance)
            {
                _instance = FindObjectOfType<Flexalon>();
                if (!_instance)
                {
                    FlexalonLog.Log("New Flexalon Instance Created");
                    var FlexalonGO = new GameObject("Flexalon");
                    _instance = FlexalonGO.AddComponent<Flexalon>();
                }
                else
                {
                    FlexalonLog.Log("Flexalon Instance Found in Scene");
                }
            }

            return _instance;
        }

        /// <summary> Returns the FlexalonNode associated with the gameObject. </summary>
        /// <param name="go"> The gameObject to get the FlexalonNode for. </param>
        /// <returns> The FlexalonNode associated with the gameObject, or null if it doesn't exist. </returns>
        public static FlexalonNode GetNode(GameObject go)
        {
            if (_instance != null && _instance && _instance._gameObjects.TryGetValue(go, out var node))
            {
                return node;
            }

            return null;
        }

        /// <summary>
        /// Returns the FlexalonNode associated with the gameObject,
        /// or creates it if it doesn't exist.
        /// </summary>
        /// <param name="go"> The gameObject to get the FlexalonNode for. </param>
        /// <param name="result"> The FlexalonResult to use for the FlexalonNode. </param>
        /// <returns> The FlexalonNode associated with the gameObject. </returns>
        public static FlexalonNode GetOrCreateNode(GameObject go, FlexalonResult result = null)
        {
            if (go == null)
            {
                return null;
            }

            GetOrCreate();

            if (!_instance._gameObjects.TryGetValue(go, out var node))
            {
                node = _instance.CreateNode();
                node._gameObject = go;
                node._adapter = new DefaultAdapter(go);
                node._result = node._gameObject.GetComponent<FlexalonResult>();
                node._hasResult = node._result != null;
                if (!node._hasResult)
                {
                    node._result = node._gameObject.AddComponent<FlexalonResult>();
                    node._dirty = true;
                }

                node.SetResultToCurrentTransform();
                _instance._gameObjects.Add(go, node);
            }

            return node;
        }

        /// <summary> Gets the current InputProvider used by FlexalonInteractables. </summary>
        public static InputProvider GetInputProvider()
        {
            GetOrCreate();

            if (_instance)
            {
                if (_instance._input == null)
                {
                    if (_instance._inputProvider)
                    {
                        _instance._input = _instance._inputProvider.GetComponent<InputProvider>();
                    }

                    if (_instance._input == null)
                    {
                        _instance._input = new FlexalonMouseInputProvider();
                    }
                }

                return _instance._input;
            }

            return null;
        }

        /// <summary> Marks every node and FlexalonComponent as dirty and calls UpdateDirtyNodes. </summary>
        public void ForceUpdate()
        {
            foreach (var node in _nodes)
            {
                foreach (var flexalonComponent in node.GameObject.GetComponents<FlexalonComponent>())
                {
                    flexalonComponent.MarkDirty();
                }

                node.MarkDirty();
            }

            UpdateDirtyNodes();
        }

        private Node CreateNode()
        {
            var node = new Node();
            node._transformUpdater = _defaultTransformUpdater;
            _nodes.Add(node);
            _roots.Add(node);
            return node;
        }

        private void DestroyNode(GameObject go)
        {
            if (_instance != null && _instance._gameObjects.TryGetValue(go, out var node))
            {
                _instance._gameObjects.Remove(go);
                node.Detach();
                node.DetachAllChildren();
                node.SetDependency(null);
                node.ClearDependents();
                _nodes.Remove(node);
                _roots.Remove(node);
            }
        }

        void LateUpdate()
        {
            if (Application.isPlaying && _updateInPlayMode)
            {
                UpdateDirtyNodes();
            }

            if (!Application.isPlaying && _updateInEditMode)
            {
                UpdateDirtyNodes();
            }

            RecordFrameChanges = false;
        }

        /// <summary> Updates all dirty nodes. </summary>
        public void UpdateDirtyNodes()
        {
            PreUpdate?.Invoke();

            _destroyed.Clear();
            foreach (var kv in _gameObjects)
            {
                var go = kv.Key;
                var node = kv.Value;
                if (!go)
                {
                    _destroyed.Add(go);
                }
                else if (!Application.isPlaying && (node._parent != null || node._dependency != null || node.FlexalonObject || node.Method != null))
                {
                    node.CheckDefaultAdapter();
                }
            }

            foreach (var go in _destroyed)
            {
                DestroyNode(go);
            }

            foreach (var root in _roots)
            {
                if (root._dependency == null && root.GameObject.activeInHierarchy)
                {
                    Compute(root, defaultSize);
                }
            }
        }

        private void UpdateTransforms(Node node)
        {
            if (!node.ReachedTargetPosition)
            {
                node.ReachedTargetPosition = node._transformUpdater.UpdatePosition(node, node._result.TargetPosition);
                foreach (Node child in node.Children)
                {
                    child.ReachedTargetPosition = false;
                }
            }

            if (!node.ReachedTargetRotation)
            {
                node.ReachedTargetRotation = node._transformUpdater.UpdateRotation(node, node._result.TargetRotation);
                foreach (Node child in node.Children)
                {
                    child.ReachedTargetRotation = false;
                }
            }

            if (!node.ReachedTargetScale)
            {
                node.ReachedTargetScale = node._transformUpdater.UpdateScale(node, node._result.TargetScale);
                foreach (Node child in node.Children)
                {
                    child.ReachedTargetScale = false;
                }
            }

            node._result.TransformPosition = node.GameObject.transform.localPosition;
            node._result.TransformRotation = node.GameObject.transform.localRotation;
            node._result.TransformScale = node.GameObject.transform.localScale;

            node.NotifyResultChanged();

            foreach (var child in node._children)
            {
                UpdateTransforms(child);
            }
        }

        void OnDestroy()
        {
            FlexalonLog.Log("Flexalon Instance Destroyed");
            _instance = null;
        }

        private void Compute(Node node, Vector3 fillSize)
        {
            if (node.Dirty)
            {
                FlexalonLog.Log("LAYOUT COMPUTE", node);
                Measure(node, fillSize);
                Arrange(node);
                Constrain(node);
            }

            if (node.HasResult)
            {
                ComputeTransforms(node);
                UpdateTransforms(node);
                ComputeDependents(node);
            }
        }

        private void ComputeDependents(Node node)
        {
            if (node._dependents != null)
            {
                var size = Math.Mul(node._result.AdapterBounds.size, node.GetWorldBoxScale(true));
                foreach (var dep in node._dependents)
                {
                    dep._dirty = dep._dirty || node.UpdateDependents;
                    Compute(dep, size);
                }
            }

            node.UpdateDependents = false;

            foreach (var child in node._children)
            {
                ComputeDependents(child);
            }
        }

        private static Vector3 GetChildAvailableSize(Node node)
        {
            return Vector3.Max(Vector3.zero,
                node._result.AdapterBounds.size - node.Padding.Size);
        }

        private void MeasureAdapterSize(Node node, Bounds bounds)
        {
            var adapterBounds = node.Adapter.Measure(node, bounds.size);
            node.RecordResultUndo();
            node._result.AdapterBounds = adapterBounds;
            FlexalonLog.Log("MeasureAdapterSize", node, adapterBounds);

            bounds.size = adapterBounds.size;
            bounds.center += adapterBounds.center;
            node._result.LayoutBounds = bounds;
            FlexalonLog.Log("LayoutBounds", node, bounds);
        }

        private void Measure(Node node, Vector3 fillSize)
        {
            FlexalonLog.Log("Measure", node, fillSize);

            node.RecordResultUndo();

            // Start by measuring whatever size we can. This might change after we
            // run the layout method later if the size is set to children.
            var size = MeasureSize(node, fillSize);
            MeasureAdapterSize(node, new Bounds(Vector3.zero, size));

            if (node.Method != null)
            {
                MeasureLayout(node);
            }

            node.ApplyScaleAndRotation();
        }

        private void MeasureFixedOnly(Node node)
        {
            node.RecordResultUndo();
            node._result.RotatedAndScaledBounds = new Bounds(Vector3.zero, MeasureSize(node, Vector3.zero));
        }

        private void MeasureLayout(Node node)
        {
            // Now let the children run their measure before running our own.
            // Assume empty available size for now just to gather fixed and component values.
            foreach (var child in node._children)
            {
                if (AnyChildAxisDependsOnParent(child))
                {
                    MeasureFixedOnly(child);
                }
                else if (child.Dirty || !child.FlexalonObject || !child.HasResult)
                {
                    Measure(child, Vector3.zero);
                }
            }

            // Figure out how much space we have for the children
            var childAvailableSize = GetChildAvailableSize(node);
            FlexalonLog.Log("Measure | ChildAvailableSize", node, childAvailableSize);

            // Measure what this node's size is given child sizes.
            var layoutBounds = node.Method.Measure(node, childAvailableSize);
            FlexalonLog.Log("Measure | LayoutBounds 1", node, layoutBounds);
            var paddedBounds = new Bounds(layoutBounds.center, layoutBounds.size + node.Padding.Size);
            MeasureAdapterSize(node, paddedBounds);

            // Measure any children that depend on our size
            bool anyChildDependsOnParent = false;
            foreach (var child in node._children)
            {
                if (AnyChildAxisDependsOnParent(child))
                {
                    anyChildDependsOnParent = true;
                    child._fillSizeChanged = false;
                    Measure(child, child._fillSize);
                    child._dirty = true;
                }
            }

            if (AnyAxisDependsOnLayout(node) && anyChildDependsOnParent)
            {
                // Re-measure given final child sizes.
                layoutBounds = node.Method.Measure(node, childAvailableSize);
                FlexalonLog.Log("Measure | LayoutBounds 2", node, layoutBounds);
                paddedBounds = new Bounds(layoutBounds.center, layoutBounds.size + node.Padding.Size);
                MeasureAdapterSize(node, paddedBounds);

                // Measure any children that depend on our size in case it was wrong the first time.
                // This cycle can continue forever, but this is the last time we'll do it.
                foreach (var child in node._children)
                {
                    if (child._fillSizeChanged && AnyChildAxisDependsOnParent(child))
                    {
                        child._fillSizeChanged = false;
                        Measure(child, child._fillSize);
                        child._dirty = true;
                    }
                }
            }
        }

        private void Arrange(Node node)
        {
            node._dirty = false;
            node._hasResult = true;
            node.SetPositionResult(Vector3.zero);
            node.SetRotationResult(Quaternion.identity);

            // If there's no children, there's nothing left to do.
            if (node.Children.Count == 0 || node.Method == null)
            {
                return;
            }

            FlexalonLog.Log("Arrange", node, node._result.AdapterBounds.size);

            // Run child arrange algorithm
            foreach (var child in node._children)
            {
                if (child._dirty)
                {
                    Arrange(child);
                }
            }

            // Figure out how much space we have for the children
            var childAvailableSize = GetChildAvailableSize(node);
            FlexalonLog.Log("Arrange | ChildAvailableSize", node, childAvailableSize);

            // Run our arrange algorithm
            node.Method.Arrange(node, childAvailableSize);

            // Run any attached modifiers
            if (node.Modifiers != null)
            {
                foreach (var modifier in node.Modifiers)
                {
                    modifier.PostArrange(node);
                }
            }
        }

        private Vector3 ComputeComponentScale(Node node)
        {
            var componentScale = Vector3.one;
            if (node._adapter != null)
            {
                componentScale = node._adapter.UpdateSize(node);
            }

            node.SetComponentScale(componentScale);
            return componentScale;
        }

        private void ComputeTransforms(Node node)
        {
            if (node.Dependency != null)
            {
                if (node.HasSizeUpdate)
                {
                    if (node.FlexalonObject)
                    {
                        var scale = ComputeComponentScale(node);
                        FlexalonLog.Log("ComputeTransform:Constrait:Scale", node, scale);
                        scale.Scale(node.Scale);
                        node.RecordResultUndo();
                        node._result.TargetScale = scale;
                        node.ReachedTargetScale = false;
                        node.HasSizeUpdate = false;

                        foreach (var child in node._children)
                        {
                            child.HasSizeUpdate = true;
                        }
                    }
                    else
                    {
                        node.ReachedTargetScale = true;
                        node.HasSizeUpdate = false;
                    }
                }

                if (node.HasPositionUpdate)
                {
                    var position = node._result.LayoutPosition;
                    FlexalonLog.Log("ComputeTransform:Constrait:LayoutPosition", node, position);
                    node.RecordResultUndo();
                    node._result.TargetPosition = position;
                    node.ReachedTargetPosition = false;
                    node.HasPositionUpdate = false;
                }

                if (node.HasRotationUpdate)
                {
                    node.RecordResultUndo();
                    node._result.TargetRotation = node._result.LayoutRotation * node.Rotation;
                    FlexalonLog.Log("ComputeTransform:Constrait:Rotation", node, node._result.TargetRotation);
                    node.ReachedTargetRotation = false;
                    node.HasRotationUpdate = false;
                }
            }
            else if (node.Parent != null)
            {
                var parentComponentScale = node.Parent.Result.ComponentScale;

                if (node.HasSizeUpdate)
                {
                    var componentScale = ComputeComponentScale(node);
                    var scale = Math.Div(componentScale, parentComponentScale);
                    scale.Scale(node.Scale);
                    FlexalonLog.Log("ComputeTransform:Layout:Scale", node, scale);
                    node.RecordResultUndo();
                    node._result.TargetScale = scale;
                    node.ReachedTargetScale = false;
                    node.HasSizeUpdate = false;

                    foreach (var child in node._children)
                    {
                        child.HasSizeUpdate = true;
                    }
                }

                if (node.HasRotationUpdate)
                {
                    node.RecordResultUndo();
                    node._result.TargetRotation = node._result.LayoutRotation * node.Rotation;
                    FlexalonLog.Log("ComputeTransform:Layout:Rotation", node, node._result.TargetRotation);
                    node.ReachedTargetRotation = false;
                    node.HasRotationUpdate = false;
                }

                if (node.HasPositionUpdate)
                {
                    var position = node._result.LayoutPosition
                        - node._parent.Padding.Center
                        + node._parent._result.AdapterBounds.center
                        - node.Margin.Center
                        - node._result.TargetRotation * node._result.RotatedAndScaledBounds.center
                        + node.Offset;

                    position = Math.Div(position, parentComponentScale);

                    FlexalonLog.Log("ComputeTransform:Layout:Position", node, position);
                    node.RecordResultUndo();
                    node._result.TargetPosition = position;
                    node.ReachedTargetPosition = false;
                    node.HasPositionUpdate = false;
                }
            }
            else
            {
                if (!node.FlexalonObject)
                {
                    node.HasSizeUpdate = false;
                    node.ReachedTargetScale = true;
                }
                else if (node.HasSizeUpdate)
                {
                    var scale = ComputeComponentScale(node);
                    scale.Scale(node.Scale);
                    FlexalonLog.Log("ComputeTransform:NoLayout:Scale", node, scale);

                    node.RecordResultUndo();
                    node._result.TargetScale = scale;
                    node.ReachedTargetScale = false;
                    node.HasSizeUpdate = false;

                    foreach (var child in node._children)
                    {
                        child.HasSizeUpdate = true;
                    }
                }

                node.HasPositionUpdate = false;
                node.HasRotationUpdate = false;
                node.ReachedTargetPosition = true;
                node.ReachedTargetRotation = true;
            }

            node._transformUpdater.PreUpdate(node);

            foreach (var child in node._children)
            {
                ComputeTransforms(child);
            }
        }

        private void Constrain(Node node)
        {
            if (node.Constraint != null)
            {
                FlexalonLog.Log("Constrain", node);
                node.Constraint.Constrain(node);
            }
        }

        private static Vector3 MeasureSize(Node node, Vector3 fillSize)
        {
            Vector3 result = new Vector3();
            for (int axis = 0; axis < 3; axis++)
            {
                var unit = node.GetSizeType(axis);
                if (unit == SizeType.Layout)
                {
                    result[axis] = 0;
                }
                else if (unit == SizeType.Component)
                {
                    result[axis] = fillSize[axis];
                }
                else if (unit == SizeType.Fill)
                {
                    result[axis] = (fillSize[axis] * node.SizeOfParent[axis]) - node.Margin.Size[axis];
                }
                else
                {
                    result[axis] = node.Size[axis];
                }
            }

            FlexalonLog.Log("MeasureSize", node, result);
            return result;
        }

        private static bool AnyAxisDependsOnLayout(Node node)
        {
            return node.GetSizeType(0) == SizeType.Layout ||
                node.GetSizeType(1) == SizeType.Layout ||
                node.GetSizeType(2) == SizeType.Layout;
        }

        private static bool AnyChildAxisDependsOnParent(Node child)
        {
            return ChildAxisDependsOnParent(child, 0) || ChildAxisDependsOnParent(child, 1) || ChildAxisDependsOnParent(child, 2);
        }

        private static bool ChildAxisDependsOnParent(Node child, int axis)
        {
            return child.GetSizeType(axis) == SizeType.Fill;
        }

        private class Node : FlexalonNode
        {
            public Node _parent;
            public FlexalonNode Parent => _parent;
            public int _index;
            public int Index => _index;
            public List<Node> _children = new List<Node>();
            public IReadOnlyList<FlexalonNode> Children => _children;
            public bool _dirty = false;
            public bool Dirty => _dirty;
            public bool _hasResult = false;
            public bool HasResult => _hasResult;
            public bool HasPositionUpdate = false;
            public bool HasSizeUpdate = false;
            public bool HasRotationUpdate = false;
            public bool ReachedTargetPosition = true;
            public bool ReachedTargetRotation = true;
            public bool ReachedTargetScale = true;
            public bool UpdateDependents = false;

            public GameObject _gameObject;
            public GameObject GameObject => _gameObject;
            public Layout _method;
            public Layout Method { get { return _method; } set { _method = value; } }
            public FlexalonConstraint _constraint;
            public FlexalonConstraint Constraint => _constraint;
            public Adapter _adapter = null;
            public Adapter Adapter => _adapter;
            public bool _customAdapter = false;
            public FlexalonResult _result;
            public FlexalonResult Result => _result;
            public FlexalonObject _flexalonObject;
            public FlexalonObject FlexalonObject => _flexalonObject;
            public Vector3 Size => _flexalonObject ? _flexalonObject.Size : Vector3.one;
            public Vector3 SizeOfParent => _flexalonObject ? _flexalonObject.SizeOfParent : Vector3.one;
            public Vector3 Offset => _flexalonObject ? _flexalonObject.Offset : Vector3.zero;
            public Vector3 Scale => _flexalonObject ? _flexalonObject.Scale : Vector3.one;
            public Quaternion Rotation => _flexalonObject ? _flexalonObject.Rotation : Quaternion.identity;
            public Directions Margin => _flexalonObject ? _flexalonObject.Margin : Directions.zero;
            public Directions Padding => _flexalonObject ? _flexalonObject.Padding : Directions.zero;
            public Node _dependency;
            public FlexalonNode Dependency => _dependency;
            public bool HasDependents => _dependents != null && _dependents.Count > 0;
            public List<Node> _dependents;
            public TransformUpdater _transformUpdater;
            public Vector3 _fillSize = new Vector3();
            public bool _fillSizeChanged = false;
            public List<FlexalonModifier> _modifiers = null;
            public IReadOnlyList<FlexalonModifier> Modifiers => _modifiers;
            public event System.Action<FlexalonNode> ResultChanged;

            public void SetFillSize(Vector3 size)
            {
                if (size != _fillSize)
                {
                    _fillSize = size;
                    _fillSizeChanged = true;
                }
            }

            public SizeType GetSizeType(Axis axis)
            {
                if (_flexalonObject)
                {
                    switch (axis)
                    {
                        case Axis.X: return _flexalonObject.WidthType;
                        case Axis.Y: return _flexalonObject.HeightType;
                        case Axis.Z: return _flexalonObject.DepthType;
                    }
                }

                return SizeType.Component;
            }

            public SizeType GetSizeType(int axis)
            {
                return GetSizeType((Axis)axis);
            }

            public void SetPositionResult(Vector3 position)
            {
                RecordResultUndo();
                _result.LayoutPosition = position;
                HasPositionUpdate = true;
                UpdateDependents = true;
            }

            public void SetRotationResult(Quaternion quaternion)
            {
                RecordResultUndo();
                _result.LayoutRotation = quaternion;
                HasRotationUpdate = true;
                UpdateDependents = true;
            }

            public void SetComponentScale(Vector3 scale)
            {
                RecordResultUndo();
                _result.ComponentScale = scale;
            }

            public void SetMethod(Layout method)
            {
                _method = method;
            }

            public void SetConstraint(FlexalonConstraint constraint, FlexalonNode target)
            {
                _constraint = constraint;
                SetDependency(target);
            }

            public void SetTransformUpdater(TransformUpdater updater)
            {
                updater = updater != null ? updater : _instance?._defaultTransformUpdater;
                if (updater != _transformUpdater)
                {
                    _transformUpdater = updater;
                }
            }

            public void SetFlexalonObject(FlexalonObject obj)
            {
                _flexalonObject = obj;
            }

            public void MarkDirty()
            {
                if (Dirty) return;

                FlexalonLog.Log("MarkDirty", this);

                var node = this;
                while (node != null)
                {
                    node._dirty = true;
                    node.HasPositionUpdate = true;
                    node.HasRotationUpdate = true;
                    node.HasSizeUpdate = true;
                    node = node._parent;
                }

                if (_dependency != null && !_dependency.HasResult)
                {
                    _dependency?.MarkDirty();
                }

#if UNITY_EDITOR
                UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
#endif
            }

            public void ForceUpdate()
            {
                MarkDirty();
                MarkDirtyDown();
                Flexalon.GetOrCreate().UpdateDirtyNodes();
            }

            private void MarkDirtyDown()
            {
                foreach (var child in _children)
                {
                    child.MarkDirty();
                    child.MarkDirtyDown();
                }

                if (HasDependents)
                {
                    foreach (var dep in _dependents)
                    {
                        dep.MarkDirty();
                        dep.MarkDirtyDown();
                    }
                }
            }

            public void AddChild(FlexalonNode child)
            {
                InsertChild(child, _children.Count);
            }

            public void InsertChild(FlexalonNode child, int index)
            {
                var childNode = child as Node;
                if (childNode._parent == this && childNode._index == index)
                {
                    return;
                }

                child.Detach();

                childNode._parent = this;
                childNode._index = index;
                _children.Insert(index, childNode);
                _instance?._roots.Remove(childNode);
            }

            public FlexalonNode GetChild(int index)
            {
                return _children[index];
            }

            public void Detach()
            {
                if (_parent != null)
                {
                    _parent._children.Remove(this);
                    _parent = null;
                    _index = 0;
                    _instance?._roots.Add(this);
                }
            }

            public void DetachAllChildren()
            {
                while (Children.Count > 0)
                {
                    Children[Children.Count - 1].Detach();
                }
            }

            public Vector3 GetMeasureSize()
            {
                var size = _result.RotatedAndScaledBounds.size + Margin.Size;
                for (int axis = 0; axis < 3; axis++)
                {
                    if (GetSizeType(axis) == SizeType.Fill)
                    {
                        size[axis] = 0;
                    }
                }

                return size;
            }

            public Vector3 GetArrangeSize()
            {
                return _result.RotatedAndScaledBounds.size + Margin.Size;
            }

            public Vector3 GetBoxScale()
            {
                if (_flexalonObject)
                {
                    // FlexalonObject size/scale always applies, even without a layout.
                    return _flexalonObject.Scale;
                }
                else if (_parent != null)
                {
                    return Vector3.one;
                }
                else
                {
                    return GameObject.transform.localScale;
                }
            }

            public Quaternion GetBoxRotation()
            {
                // FlexalonObject rotation only takes effect if there's a layout.
                if (_parent != null || _dependency != null)
                {
                    return _flexalonObject?.Rotation ?? Quaternion.identity;
                }
                else
                {
                    return GameObject.transform.localRotation;
                }
            }

            public Vector3 GetWorldBoxScale(bool includeLocalScale)
            {
                Vector3 scale = includeLocalScale ? GetBoxScale() : Vector3.one;
                var node = this;
                while (node._parent != null)
                {
                    scale.Scale(node._parent.GetBoxScale());
                    node = node._parent;
                }

                if (node.GameObject.transform.parent != null)
                {
                    scale.Scale(node.GameObject.transform.parent.lossyScale);
                }

                return scale;
            }

            public Vector3 GetWorldBoxPosition(Vector3 scale, bool includePadding)
            {
                var pos = _result.LayoutBounds.center;
                if (includePadding)
                {
                    pos -= Padding.Center;
                }

                pos.Scale(scale);
                pos = GameObject.transform.rotation * pos + GameObject.transform.position;
                return pos;
            }

            public void SetDependency(FlexalonNode node)
            {
                if (_dependency != node)
                {
                    _dependency?._dependents.Remove(this);

                    _dependency = node as Node;

                    if (node != null)
                    {
                        if (_dependency._dependents == null)
                        {
                            _dependency._dependents = new List<Node>();
                        }

                        _dependency._dependents.Add(this);
                    }
                }
            }

            public void ClearDependents()
            {
                if (_dependents != null)
                {
                    while (_dependents.Count > 0)
                    {
                        _dependents[_dependents.Count - 1].SetDependency(null);
                    }
                }
            }

            public void SetAdapter(Adapter adapter)
            {
                if (_adapter != adapter)
                {
                    _adapter = adapter;

                    if (_adapter == null)
                    {
                        _adapter = new DefaultAdapter(GameObject);
                        _customAdapter = false;
                    }
                    else
                    {
                        _customAdapter = true;
                    }
                }
            }

            public void CheckDefaultAdapter()
            {
                if (!_customAdapter)
                {
                    if ((_adapter as DefaultAdapter).CheckComponent(GameObject))
                    {
                        MarkDirty();
                    }
                }
            }

            public void ApplyScaleAndRotation()
            {
                var bounds = Math.ScaleBounds(_result.LayoutBounds, GetBoxScale());
                bounds = Math.RotateBounds(bounds, GetBoxRotation());
                RecordResultUndo();
                _result.RotatedAndScaledBounds = bounds;
                FlexalonLog.Log("Measure | RotatedAndScaledBounds", this, bounds);
                HasSizeUpdate = true;
                UpdateDependents = true;
            }

            public void SetResultToCurrentTransform()
            {
                _result.TransformPosition = GameObject.transform.localPosition;
                _result.TransformRotation = GameObject.transform.localRotation;
                _result.TransformScale = GameObject.transform.localScale;
                _result.TargetPosition = GameObject.transform.localPosition;
                _result.TargetRotation = GameObject.transform.localRotation;
                _result.TargetScale = GameObject.transform.localScale;
            }

            public void RecordResultUndo()
            {
#if UNITY_EDITOR
                if (Flexalon.GetOrCreate().RecordFrameChanges)
                {
                    UnityEditor.Undo.RecordObject(_result, "Result changed");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(_result);
                }
#endif
            }

            public void AddModifier(FlexalonModifier modifier)
            {
                if (_modifiers == null)
                {
                    _modifiers = new List<FlexalonModifier>();
                }

                _modifiers.RemoveAll(m => m == modifier);
                _modifiers.Add(modifier);
            }

            public void RemoveModifier(FlexalonModifier modifier)
            {
                _modifiers?.Remove(modifier);
            }

            public void NotifyResultChanged()
            {
                ResultChanged?.Invoke(this);
            }
        }
    }
}