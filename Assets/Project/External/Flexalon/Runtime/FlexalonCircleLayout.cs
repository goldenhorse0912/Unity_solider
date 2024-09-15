using UnityEngine;

namespace Flexalon
{
    /// <summary> Use a circle layout to position children along a circle or spiral. </summary>
    [ExecuteAlways, AddComponentMenu("Flexalon/Flexalon Circle Layout"), HelpURL("https://www.flexalon.com/docs/circleLayout")]
    public class FlexalonCircleLayout : LayoutBase
    {
        [SerializeField]
        private float _radius = 1;
        /// <summary> Radius of the circle. </summary>
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private bool _useWidth = false;
        /// <summary>
        /// Instead of specifying the radius, this option will match the radius to
        /// half of the width determined by the Flexalon Object Component. This can
        /// be useful if the size of the circle should be determined by another layout
        /// or constraint.
        /// </summary>
        public bool UseWidth
        {
            get { return _useWidth; }
            set { _useWidth = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private bool _spiral = false;
        /// <summary> If checked, positions each object at increasing heights to form a spiral. </summary>
        public bool Spiral
        {
            get { return _spiral; }
            set { _spiral = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _spiralSpacing = 0;
        /// <summary> Vertical spacing between objects in the spiral. </summary>
        public float SpiralSpacing
        {
            get { return _spiralSpacing; }
            set { _spiralSpacing = value; _node.MarkDirty(); }
        }

        /// <summary> Determines how the space between children is distributed. </summary>
        public enum SpacingOptions
        {
            /// <summary> The Spacing Degrees property determines the space between children. </summary>
            Fixed,

            /// <summary> The space around the circle is distributed between children. </summary>
            Evenly,
        }

        [SerializeField]
        private SpacingOptions _spacingType;
        /// <summary> Determines how the space between children is distributed. </summary>
        public SpacingOptions SpacingType
        {
            get { return _spacingType; }
            set { _spacingType = value; _node.MarkDirty(); }
        }

        [SerializeField]
        /// <summary> Radial space between children when SpacingType is Fixed. </summary>
        private float _spacingDegrees = 30.0f;
        public float SpacingDegrees
        {
            get { return _spacingDegrees; }
            set { _spacingDegrees = value; _node.MarkDirty(); }
        }

        /// <summary> Determines if and how the radius changes. </summary>
        public enum RadiusOptions
        {
            /// <summary> The radius does not change. </summary>
            Constant,

            /// <summary> The radius is incremented for each child by the Radius Step property.
            /// This can be used to create an inward or outward spiral. </summary>
            Step,

            /// <summary> If set to Wrap, the radius is incremented each time around the circle.
            /// This can be used to create concentric circles of objects. </summary>
            Wrap
        }

        [SerializeField]
        private RadiusOptions _radiusType = RadiusOptions.Constant;
        /// <summary> Determines if and how the radius changes. </summary>
        public RadiusOptions RadiusType
        {
            get { return _radiusType; }
            set { _radiusType = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _radiusStep = 0.1f;
        /// <summary> Determines how much the radius should change at each interval. </summary>
        public float RadiusStep
        {
            get { return _radiusStep; }
            set { _radiusStep = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private float _startAtDegrees = 0.0f;
        /// <summary> By default, the first child will be placed at (radius, 0, 0).
        /// Start At Degrees value will add an offset all children around the circle. </summary>
        public float StartAtDegrees
        {
            get { return _startAtDegrees; }
            set { _startAtDegrees = value; _node.MarkDirty(); }
        }

        /// <summary> Determines how children should be rotated. </summary>
        public enum RotateOptions
        {
            /// <summary> Child rotation is set to zero. </summary>
            None,

            /// <summary> Children face out of the circle. </summary>
            Out,

            /// <summary> Children face into the circle. </summary>
            In,

            /// <summary> Children face forward along the circle. </summary>
            Forward,

            /// <summary> Children face backward along the circle. </summary>
            Backwards
        }

        [SerializeField]
        private RotateOptions _rotate = RotateOptions.None;
        /// <summary> Determines how children should be rotated. </summary>
        public RotateOptions Rotate
        {
            get { return _rotate; }
            set { _rotate = value; _node.MarkDirty(); }
        }

        [SerializeField]
        private Align _verticalAlign = Align.Center;
        /// <summary> Vertically aligns the layout with the height set by the Flexalon Object Component.
        /// For a circle, this will align each individual object in the layout. For a spiral,
        /// this will align the entire spiral. </summary>
        public Align VerticalAlign
        {
            get { return _verticalAlign; }
            set { _verticalAlign = value; _node.MarkDirty(); }
        }

        private float GetSpacing(FlexalonNode node)
        {
            var spacing = _spacingDegrees * Mathf.PI / 180;
            if (_spacingType == SpacingOptions.Evenly)
            {
                if (node.Children.Count < 2)
                {
                    spacing = 0;
                }
                else
                {
                    spacing = 2 * Mathf.PI / node.Children.Count;
                }
            }

            return spacing;
        }

        private float GetRadius(Vector3 layoutSize)
        {
            return _useWidth ? layoutSize.x / 2 : _radius;
        }

        /// <inheritdoc />
        public override Bounds Measure(FlexalonNode node, Vector3 size)
        {
            var spacing = GetSpacing(node);

            var diameter = _radius * 2;
            if (_useWidth)
            {
                float maxChildWidth = 0;
                foreach (var child in node.Children)
                {
                    maxChildWidth = Mathf.Max(maxChildWidth, child.GetMeasureSize().x);
                }

                diameter = maxChildWidth / Mathf.Tan(spacing / 2);
            }

            if (_radiusType == RadiusOptions.Step)
            {
                var radius = diameter / 2;
                var stepRadius = Mathf.Abs(radius + _radiusStep * (node.Children.Count - 1));
                if (stepRadius > Mathf.Abs(radius))
                {
                    diameter = stepRadius * 2;
                }
            }
            else if (_radiusType == RadiusOptions.Wrap)
            {
                var startAt = _startAtDegrees * Mathf.PI / 180;
                var lastAngle = (node.Children.Count - 1) * spacing + startAt;
                float timesAroundCircle = Mathf.Floor(lastAngle / (Mathf.PI * 2));
                var radius = diameter / 2;
                var wrapRadius = Mathf.Abs(radius + timesAroundCircle * _radiusStep);
                if (wrapRadius > Mathf.Abs(radius))
                {
                    diameter = wrapRadius * 2;
                }
            }

            diameter = Mathf.Abs(diameter);

            if (node.GetSizeType(Axis.X) == SizeType.Layout)
            {
                size.x = diameter;
            }

            float spiralHeight = 0;
            if (_spiral)
            {
                spiralHeight = _spiralSpacing * (node.Children.Count - 1);
                float minSpiralHeight = 0;
                foreach (var child in node.Children)
                {
                    // Note: this GetMeasureSize will be 0 for any child axis using SizeType.Fill.
                    float childHeight = child.GetMeasureSize().y;
                    spiralHeight += childHeight;
                    minSpiralHeight = Mathf.Max(minSpiralHeight, childHeight);
                }

                spiralHeight = Mathf.Max(spiralHeight, minSpiralHeight);
            }

            if (node.GetSizeType(Axis.Y) == SizeType.Layout)
            {
                if (_spiral)
                {
                    size.y = spiralHeight;
                }
                else
                {
                    foreach (var child in node.Children)
                    {
                        size.y = Mathf.Max(size.y, child.GetMeasureSize().y);
                    }
                }
            }

            if (node.GetSizeType(Axis.Z) == SizeType.Layout)
            {
                size.z = diameter;
            }

            float percentTotal = 0;
            foreach (var child in node.Children)
            {
                if (child.GetSizeType(Axis.Y) == SizeType.Fill)
                {
                    percentTotal += child.SizeOfParent[1];
                }
            }

            float remainingHeight = Mathf.Max(0, size.y - spiralHeight);

            var childAvailableWidth = (node.Children.Count <= 2 && _spacingType == SpacingOptions.Evenly) ? 1 :
                diameter * Mathf.Tan(spacing / 2);

            foreach (var child in node.Children)
            {
                float childAvailableHeight = size.y;
                if (_spiral)
                {
                    var percent = percentTotal <= 1 ?
                        child.SizeOfParent[1] :
                        (child.SizeOfParent[1] / percentTotal);
                    childAvailableHeight = percent * remainingHeight;
                }

                child.SetFillSize(new Vector3(childAvailableWidth, childAvailableHeight, childAvailableWidth));
            }

            return new Bounds(Vector3.zero, size);
        }

        /// <inheritdoc />
        public override void Arrange(FlexalonNode node, Vector3 layoutSize)
        {
            FlexalonLog.Log("CircleArrange | LayoutSize", node, layoutSize);

            var startAt = _startAtDegrees * Mathf.PI / 180;
            var spacing = GetSpacing(node);
            var startRadius = GetRadius(layoutSize);

            var spiralHeight = _spiralSpacing * (node.Children.Count - 1);
            float minSpiralHeight = 0;
            foreach (var child in node.Children)
            {
                float childHeight = child.GetArrangeSize().y;
                spiralHeight += childHeight;
                minSpiralHeight = Mathf.Max(minSpiralHeight, childHeight);
            }

            spiralHeight = Mathf.Max(spiralHeight, minSpiralHeight);

            float spiralStart = Math.Align(spiralHeight, layoutSize.y, _verticalAlign) - spiralHeight / 2;
            float spiralPos = spiralStart;

            for (int i = 0; i < node.Children.Count; i++)
            {
                var angle = i * spacing + startAt;
                float radius = startRadius;
                if (_radiusType == RadiusOptions.Step)
                {
                    radius += i * _radiusStep;
                }
                else if (_radiusType == RadiusOptions.Wrap)
                {
                    float timesAroundCircle = Mathf.Floor(angle / (Mathf.PI * 2));
                    radius += timesAroundCircle * _radiusStep;
                }

                var child = node.Children[i];
                var childSize = child.GetArrangeSize();
                var pos = new Vector3(
                    radius * Mathf.Cos(angle),
                    0,
                    radius * Mathf.Sin(angle));

                if (_spiral)
                {
                    pos.y = spiralPos + childSize.y * 0.5f;
                    spiralPos += childSize.y + _spiralSpacing;
                    spiralPos = Mathf.Max(spiralStart, spiralPos);
                }
                else
                {
                    pos.y = Math.Align(childSize, layoutSize, Axis.Y, _verticalAlign);
                }

                child.SetPositionResult(pos);

                float rotation = -i * spacing - startAt;
                switch (_rotate)
                {
                    case RotateOptions.None:
                        rotation = 0;
                        break;
                    case RotateOptions.Forward:
                        break;
                    case RotateOptions.Backwards:
                        rotation += Mathf.PI;
                        break;
                    case RotateOptions.In:
                        rotation += Mathf.PI * 0.5f;
                        break;
                    case RotateOptions.Out:
                        rotation -= Mathf.PI * 0.5f;
                        break;
                }

                var q = Quaternion.AngleAxis(rotation * 180.0f / Mathf.PI, Vector3.up);
                child.SetRotationResult(q);
            }
        }

        void OnDrawGizmosSelected()
        {
            if (_node != null)
            {
                // Draw a semitransparent circle at the transforms position
                Gizmos.color = new Color(1, 1, 0, 0.5f);
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                int segments = 30;

                var scale = _node.GetWorldBoxScale(true);

                var radius = _radius;
                if (_useWidth)
                {
                    radius = _node.Result.AdapterBounds.size.x * scale.x / 2;
                }

                for (int i = 0; i < segments; i++)
                {
                    var a1 = Mathf.PI * 2 * (i / (float)segments);
                    var a2 = Mathf.PI * 2 * ((i + 1) / (float)segments);
                    var p1 = new Vector3(radius * Mathf.Cos(a1), 0, radius * Mathf.Sin(a1));
                    var p2 = new Vector3(radius * Mathf.Cos(a2), 0, radius * Mathf.Sin(a2));
                    Gizmos.DrawLine(p1, p2);
                }
            }
        }
    }
}