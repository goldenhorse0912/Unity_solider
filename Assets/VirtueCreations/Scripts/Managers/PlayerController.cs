using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;

namespace VIRTUE {
    [RequireComponent (typeof (CapsuleCollider))]
    [RequireComponent (typeof (Rigidbody))]
    public sealed partial class PlayerController : CachedAnimator {
        [FoldoutGroup ("Movement")]
        [SerializeField]
        float moveSpeed = 5f;

        [FoldoutGroup ("Movement")]
        [SerializeField, Range (0f, 1f)]
        float turnSmoothTime = 0.2f;

        [FoldoutGroup ("Movement")]
        [SerializeField, Range (0f, 1f)]
        float speedSmoothTime = 0.1f;

        // internal bool IsMoving { get; private set; }
        internal bool IsMoving => _inputDir.magnitude > 0.05f;

        float _inputX;
        float _inputY;
        float _turnSmoothVelocity;
        float _speedSmoothVelocity;
        float _currentSpeed;
        Vector2 _cachedInput;
        Vector2 _inputDir;
        Rigidbody _rb;
        PlayerInputs _input;
        // ToolController _toolController;
        // WeaponController _weaponController;

#region UNITY_CALLBACKS

        protected override void OnAwake () {
            base.OnAwake ();
            _rb = GetComponent<Rigidbody> ();
            // _toolController = GetComponent<ToolController> ();
            // _weaponController = GetComponent<WeaponController> ();
            _hsc = GetComponent<HealthSystemComponent> ();
            _visibleTargets = new List<Transform> ();
            _input = new PlayerInputs ();
            _input.Player.Movement.performed += GetInputs;
            _input.Player.Movement.canceled += GetInputs;
        }

        void OnEnable () {
            _input.Player.Enable ();
            _hsc.GetHealthSystem ().OnDead += Dead;
            if (_isDead) {
                Rebind ();
                _isDead = false;
            }
        }

        void OnDisable () {
            _input.Player.Disable ();
            _rb.velocity = Vector3.zero;
            _hsc.GetHealthSystem ().OnDead -= Dead;
            if (_isDead) {
                return;
            }
            HandleAnimation ();
        }

        void Update () {
            HandleRotation ();
            HandleAnimation ();
        }

        void FixedUpdate () {
            Movement ();
        }

#endregion

#region PRIVATE_CALLBACKS

        void GetInputs (InputAction.CallbackContext context) {
            _inputDir = context.ReadValue<Vector2> ().normalized;
            if (context.performed) {
                StopSearchingResourceObject ();
            } else {
                StartSearchingResourceObject ();
            }
        }

        void Movement () {
            var targetSpeed = moveSpeed * _inputDir.magnitude;
            _currentSpeed = Mathf.SmoothDamp (_currentSpeed, targetSpeed, ref _speedSmoothVelocity, speedSmoothTime);
            var vel = CachedTransform.forward * _currentSpeed;
            // _rb.MovePosition (_rb.position + vel * Time.deltaTime);
            _rb.velocity = vel;
        }

        void HandleRotation () {
            if (_targetIsSet) {
                var dir = CachedTransform.DirectionTo (_target);
                if (dir != Vector3.zero) {
                    var targetRotation = Mathf.Atan2 (dir.x, dir.z) * Mathf.Rad2Deg;
                    CachedTransform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle (CachedTransform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, turnSmoothTime);
                }
            } else {
                if (_inputDir != Vector2.zero) {
                    var targetRotation = Mathf.Atan2 (_inputDir.x, _inputDir.y) * Mathf.Rad2Deg;
                    CachedTransform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle (CachedTransform.eulerAngles.y, targetRotation, ref _turnSmoothVelocity, turnSmoothTime);
                }
            }
        }

        void HandleAnimation () {
            var animationSpeedPercent = 1f * _inputDir.magnitude;
            PlayAnim (AnimatorParams.IsMoving, _inputDir != Vector2.zero);
            Anim.SetFloat (AnimatorParams.Move, animationSpeedPercent, speedSmoothTime, Time.deltaTime);
        }

        void ToggleScript (bool state) => enabled = state;

#endregion

        internal void Enable () => ToggleScript (true);

        internal void Disable () => ToggleScript (false);

        internal void ToPyramid () {
            Disable ();
            PlayAnim (AnimatorParams.IsMoving, _inputDir != Vector2.zero);
            Anim.SetFloat (AnimatorParams.Move, 1f, speedSmoothTime, Time.deltaTime);
            var position = CachedTransform.position;
            position.z += 2f;
            CachedTransform.DOMove (position, .1f);
        }

        internal void ResetPositionAndRotation () {
            CachedTransform.SetPositionAndRotation (Vector3.zero, Quaternion.identity);
            Enable ();
            smokeTrail.Play ();
        }

        public void SetPosition (Vector3Variable positionVar) {
            CachedTransform.position = positionVar.value;
        }

        public void SetMoveSpeed (float value) {
            moveSpeed = value;
        }
    }
}