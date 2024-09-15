using UnityEngine;

namespace Flexalon
{
    /// <summary> To control the size of an object, add a Flexalon Object
    /// component to it and edit the width, height, or depth properties. </summary>
    [ExecuteAlways, DisallowMultipleComponent, AddComponentMenu("Flexalon/Flexalon Object"), HelpURL("https://www.flexalon.com/docs/flexalonObject")]
    public class FlexalonObject : FlexalonComponent
    {
        /// <summary> The fixed size of the object. </summary>
        public Vector3 Size
        {
            get => new Vector3(_width, _height, _depth);
            set
            {
                Width = value.x;
                Height = value.y;
                Depth = value.z;
            }
        }

        /// <summary> The relative size of the object. </summary>
        public Vector3 SizeOfParent
        {
            get => new Vector3(_widthOfParent, _heightOfParent, _depthOfParent);
            set
            {
                WidthOfParent = value.x;
                HeightOfParent = value.y;
                DepthOfParent = value.z;
            }
        }

        [SerializeField]
        private SizeType _widthType = SizeType.Component;
        /// <summary> The width type of the object. </summary>
        public SizeType WidthType
        {
            get { return _widthType; }
            set {
                _widthType = value;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _width = 1;
        /// <summary> The fixed width of the object. </summary>
        public float Width
        {
            get { return _width; }
            set {
                _width = Mathf.Max(value, 0);
                _widthType = SizeType.Fixed;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _widthOfParent = 1;
        /// <summary> The relative width of the object. </summary>
        public float WidthOfParent
        {
            get { return _widthOfParent; }
            set {
                _widthOfParent = Mathf.Max(value, 0);
                _widthType = SizeType.Fill;
                MarkDirty();
            }
        }

        [SerializeField]
        private SizeType _heightType = SizeType.Component;
        /// <summary> The height type of the object. </summary>
        public SizeType HeightType
        {
            get { return _heightType; }
            set {
                _heightType = value;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _height = 1;
        /// <summary> The fixed height of the object. </summary>
        public float Height
        {
            get { return _height; }
            set {
                _height = Mathf.Max(value, 0);
                _heightType = SizeType.Fixed;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _heightOfParent = 1;
        /// <summary> The relative height of the object. </summary>
        public float HeightOfParent
        {
            get { return _heightOfParent; }
            set {
                _heightOfParent = Mathf.Max(value, 0);
                _heightType = SizeType.Fill;
                MarkDirty();
            }
        }

        [SerializeField]
        private SizeType _depthType = SizeType.Component;
        /// <summary> The depth type of the object. </summary>
        public SizeType DepthType
        {
            get { return _depthType; }
            set {
                _depthType = value;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _depth = 1;
        /// <summary> The fixed depth of the object. </summary>
        public float Depth
        {
            get { return _depth; }
            set {
                _depth = Mathf.Max(value, 0);
                _depthType = SizeType.Fixed;
                MarkDirty();
            }
        }

        [SerializeField]
        private float _depthOfParent = 1;
        /// <summary> The relative depth of the object. </summary>
        public float DepthOfParent
        {
            get { return _depthOfParent; }
            set {
                _depthOfParent = Mathf.Max(value, 0);
                _depthType = SizeType.Fill;
                MarkDirty();
            }
        }

        [SerializeField]
        private Vector3 _offset = Vector3.zero;
        /// <summary> Use offset to add an offset to the final position of the gameObject after layout is complete. </summary>
        public Vector3 Offset
        {
            get { return _offset; }
            set { _offset = value; MarkDirty(); }
        }

        [SerializeField]
        private Vector3 _scale = Vector3.one;
        /// <summary> Use rotation to scale the size of the gameObject before layout runs.
        /// This will generate a new size to encapsulate the scaled object. </summary>
        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; MarkDirty(); }
        }

        [SerializeField]
        private Quaternion _rotation = Quaternion.identity;
        /// <summary> Use rotation to set the rotation of the gameObject before layout runs.
        /// This will generate a new size to encapsulate the rotated object. </summary>
        public Quaternion Rotation
        {
            get { return _rotation; }
            set { _rotation = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginLeft;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginLeft
        {
            get { return _marginLeft; }
            set { _marginLeft = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginRight;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginRight
        {
            get { return _marginRight; }
            set { _marginRight = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginTop;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginTop
        {
            get { return _marginTop; }
            set { _marginTop = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginBottom;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginBottom
        {
            get { return _marginBottom; }
            set { _marginBottom = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginFront;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginFront
        {
            get { return _marginFront; }
            set { _marginFront = value; MarkDirty(); }
        }

        [SerializeField]
        private float _marginBack;
        /// <summary> Margin to add additional space around a gameObject. </summary>
        public float MarginBack
        {
            get { return _marginBack; }
            set { _marginBack = value; MarkDirty(); }
        }

        /// <summary> Margin to add additional space around a gameObject. </summary>
        public Directions Margin
        {
            get => new Directions(new float[] {
                _marginRight, _marginLeft, _marginTop, _marginBottom, _marginBack, _marginFront});
            set
            {
                _marginRight = value.Values[0];
                _marginLeft = value.Values[1];
                _marginTop = value.Values[2];
                _marginBottom = value.Values[3];
                _marginBack = value.Values[4];
                _marginFront = value.Values[5];
                MarkDirty();
            }
        }

        [SerializeField]
        private float _paddingLeft;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingLeft
        {
            get { return _paddingLeft; }
            set { _paddingLeft = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingRight;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingRight
        {
            get { return _paddingRight; }
            set { _paddingRight = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingTop;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingTop
        {
            get { return _paddingTop; }
            set { _paddingTop = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingBottom;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingBottom
        {
            get { return _paddingBottom; }
            set { _paddingBottom = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingFront;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingFront
        {
            get { return _paddingFront; }
            set { _paddingFront = value; MarkDirty(); }
        }

        [SerializeField]
        private float _paddingBack;
        /// <summary> Padding to reduce available space inside a layout. </summary>
        public float PaddingBack
        {
            get { return _paddingBack; }
            set { _paddingBack = value; MarkDirty(); }
        }

        /// <summary> Padding to reduce available space inside a layout. </summary>
        public Directions Padding
        {
            get => new Directions(new float[] {
                _paddingRight, _paddingLeft, _paddingTop, _paddingBottom, _paddingBack, _paddingFront});
            set
            {
                _paddingRight = value.Values[0];
                _paddingLeft = value.Values[1];
                _paddingTop = value.Values[2];
                _paddingBottom = value.Values[3];
                _paddingBack = value.Values[4];
                _paddingFront = value.Values[5];
                MarkDirty();
            }
        }

        /// <inheritdoc />
        protected override void ResetProperties()
        {
            _node.SetFlexalonObject(null);
        }

        /// <inheritdoc />
        protected override void UpdateProperties()
        {
            _node.SetFlexalonObject(this);
        }

#if UNITY_EDITOR
        /// <inheritdoc />
        public override void DoUpdate()
        {
            if (Application.isPlaying || Node.Dirty)
            {
                return;
            }

            // Detect changes to the object's position rotation and scale, which may happen
            // when the developer uses the transform control, enters new values in the
            // inspector, or various other scenarios. Maintain those edits
            // by modifying the offset, rotation, and scale on the FlexalonObject.
            var result = _node.Result;
            if ((_node.Parent != null && !_node.Parent.Dirty && transform.parent == _node.Parent.GameObject.transform) ||
                (_node.Constraint != null && _node.Constraint.Target != null))
            {
                if (result.TransformScale != transform.localScale)
                {
                    UnityEditor.Undo.RecordObject(this, "Scale change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                    UnityEditor.Undo.RecordObject(result, "Scale change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                    Flexalon.GetOrCreate().RecordFrameChanges = true;
                    _scale = Math.Mul(Scale, Math.Div(transform.localScale, result.TransformScale));
                    result.TransformScale = transform.localScale;
                    _node.ApplyScaleAndRotation();
                    _node.Parent?.MarkDirty();
                    _node.Constraint?.MarkDirty();

                    // The scale and rect transform controls affect both position and scale,
                    // That's not expected in a layout, so early out here to avoid setting the position.
                    return;
                }

                if (result.TransformPosition != transform.localPosition)
                {
                    UnityEditor.Undo.RecordObject(this, "Offset change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                    UnityEditor.Undo.RecordObject(result, "Offset change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);

                    if (Node.Constraint != null && Node.Constraint.Target != null)
                    {
                        _offset += Quaternion.Inverse(Node.Constraint.Target.transform.rotation) * (transform.localPosition - result.TransformPosition);
                    }
                    else
                    {
                        _offset += Math.Mul(Node.Parent.Result?.ComponentScale ?? Vector3.one, (transform.localPosition - result.TransformPosition));
                    }

                    result.TransformPosition = transform.localPosition;
                }

                if (result.TransformRotation != transform.localRotation)
                {
                    UnityEditor.Undo.RecordObject(this, "Rotation change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
                    UnityEditor.Undo.RecordObject(result, "Rotation change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                    Flexalon.GetOrCreate().RecordFrameChanges = true;

                    if (Node.Constraint != null && Node.Constraint.Target != null)
                    {
                        _rotation = Quaternion.Inverse(Node.Constraint.Target.transform.rotation) * transform.rotation;
                    }
                    else
                    {
                        _rotation *= transform.localRotation * Quaternion.Inverse(result.TransformRotation);
                    }

                    _rotation.Normalize();
                    result.TransformRotation = transform.localRotation;
                    _node.ApplyScaleAndRotation();
                    _node.Parent?.MarkDirty();
                    _node.Constraint?.MarkDirty();
                }
            }
            else
            {
                if (result.TransformRotation != transform.localRotation)
                {
                    UnityEditor.Undo.RecordObject(result, "Rotation change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                    result.TransformRotation = transform.localRotation;
                    _node.ApplyScaleAndRotation();
                }

                if (result.TransformScale != transform.localScale)
                {
                    UnityEditor.Undo.RecordObject(result, "Scale change");
                    UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(result);
                    result.TransformScale = transform.localScale;
                    _node.ApplyScaleAndRotation();
                }
            }
        }
#endif
    }
}