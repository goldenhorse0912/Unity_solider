using UnityEngine;

namespace Flexalon
{
    /// <summary>
    /// The curve animator applies a curve the the position, rotation, and scale
    /// of the object. The curve is restarted each time the layout position changes.
    /// This is ideal for scenarios in which the layout position does not change often.
    /// </summary>
    [AddComponentMenu("Flexalon/Flexalon Curve Animator"), HelpURL("https://www.flexalon.com/docs/animators")]
    public class FlexalonCurveAnimator : MonoBehaviour, TransformUpdater
    {
        private FlexalonNode _node;

        [SerializeField]
        private AnimationCurve _curve = AnimationCurve.Linear(0, 0, 1, 1);
        /// <summary> The curve to apply. Should begin at 0 and end at 1. </summary>
        public AnimationCurve Curve
        {
            get => _curve;
            set { _curve = value; }
        }

        [SerializeField]
        private bool _animatePosition = true;
        /// <summary> Determines if the position should be animated. </summary>
        public bool AnimatePosition
        {
            get => _animatePosition;
            set { _animatePosition = value; }
        }

        [SerializeField]
        private bool _animateRotation = true;
        /// <summary> Determines if the rotation should be animated. </summary>
        public bool AnimateRotation
        {
            get => _animateRotation;
            set { _animateRotation = value; }
        }

        [SerializeField]
        private bool _animateScale = true;
        /// <summary> Determines if the scale should be animated. </summary>
        public bool AnimateScale
        {
            get => _animateScale;
            set { _animateScale = value; }
        }

        private Vector3 _startPosition;
        private Quaternion _startRotation;
        private Vector3 _startScale;

        private Vector3 _endPosition;
        private Quaternion _endRotation;
        private Vector3 _endScale;

        private float _positionTime;
        private float _rotationTime;
        private float _scaleTime;

        private Vector3 _fromPosition;
        private Quaternion _fromRotation;
        private Vector3 _fromScale;

        void OnEnable()
        {
            _startPosition = _endPosition = new Vector3(float.NaN, float.NaN, float.NaN);
            _startRotation = _endRotation = new Quaternion(float.NaN, float.NaN, float.NaN, float.NaN);
            _startScale = _endScale = new Vector3(float.NaN, float.NaN, float.NaN);
            _positionTime = _rotationTime = _scaleTime = 0;

            _node = Flexalon.GetOrCreateNode(gameObject);
            _node.SetTransformUpdater(this);
        }

        void OnDisable()
        {
            _node?.SetTransformUpdater(null);
            _node = null;
        }

        /// <inheritdoc />
        public void PreUpdate(FlexalonNode node)
        {
            _fromPosition = transform.position;
            _fromRotation = transform.rotation;
            _fromScale = transform.lossyScale;
        }

        /// <inheritdoc />
        public bool UpdatePosition(FlexalonNode node, Vector3 position)
        {
            var worldPosition = transform.parent ? transform.parent.localToWorldMatrix.MultiplyPoint(position) : position;
            if (worldPosition != _endPosition)
            {
                _startPosition = _fromPosition;
                _endPosition = worldPosition;
                _positionTime = 0;
            }

            _positionTime += Time.smoothDeltaTime;

            if (!_animatePosition || _positionTime > _curve.keys[_curve.keys.Length - 1].time)
            {
                transform.localPosition = position;
                _endPosition = new Vector3(float.NaN, float.NaN, float.NaN);
                return true;
            }
            else
            {
                transform.position = Vector3.Lerp(_startPosition, _endPosition, _curve.Evaluate(_positionTime));
                return false;
            }
        }

        /// <inheritdoc />
        public bool UpdateRotation(FlexalonNode node, Quaternion rotation)
        {
            var worldRotation = transform.parent ? transform.parent.rotation * rotation : rotation;
            if (worldRotation != _endRotation)
            {
                _startRotation = _fromRotation;
                _endRotation = worldRotation;
                _rotationTime = 0;
            }

            _rotationTime += Time.smoothDeltaTime;

            if (!_animateRotation || _rotationTime > _curve.keys[_curve.keys.Length - 1].time)
            {
                transform.localRotation = rotation;
                _endRotation = new Quaternion(float.NaN, float.NaN, float.NaN, float.NaN);
                return true;
            }
            else
            {
                transform.rotation = Quaternion.Slerp(_startRotation, _endRotation, _curve.Evaluate(_rotationTime));
                return false;
            }
        }

        /// <inheritdoc />
        public bool UpdateScale(FlexalonNode node, Vector3 scale)
        {
            var worldScale = transform.parent ? Math.Mul(scale, transform.parent.lossyScale) : scale;
            if (worldScale != _endScale)
            {
                _startScale = _fromScale;
                _endScale = worldScale;
                _scaleTime = 0;
            }

            _scaleTime += Time.smoothDeltaTime;

            if (!_animateScale || _scaleTime > _curve.keys[_curve.keys.Length - 1].time)
            {
                transform.localScale = scale;
                _endScale = new Vector3(float.NaN, float.NaN, float.NaN);
                return true;
            }
            else
            {
                var newWorldScale = Vector3.Lerp(_startScale, _endScale, _curve.Evaluate(_scaleTime));
                transform.localScale = transform.parent ? Math.Div(newWorldScale, transform.parent.lossyScale) : newWorldScale;
                return false;
            }
        }
    }
}