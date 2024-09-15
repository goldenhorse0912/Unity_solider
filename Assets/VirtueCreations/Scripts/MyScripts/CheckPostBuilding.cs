using System;
using DG.Tweening;
using UnityEngine;

namespace VIRTUE {
    public class CheckPostBuilding : Building {
        [SerializeField]
        Transform[] borders;
        [SerializeField]
        Transform building;
        [SerializeField]
        Transform pole;

        FieldOfView _fov;
        bool _gateOpened;

        protected override void OnAwake () {
            base.OnAwake ();
            TryGetComponent (out _fov);
            if (!isPlayer) {
                _gateOpened = true;
            }
        }

        protected override void OnDie () {
            base.OnDie ();
            _fov.enabled = false;
            if (isPlayer) {
                Manager.Instance.BuildingManager.CheckPostDead ();
            }
        }

        void Update () {
            if (!_fov.enabled) return;
            if (_fov.isFound) {
                OpenGate ();
            } else {
                CloseGate ();
            }
        }

        void OpenGate () {
            if (_gateOpened) return;
            _gateOpened = true;
            pole.DOLocalRotate (Vector3.left * 70f, .5f).SetEase (Ease.OutBack).SetId (GetInstanceID ());
        }

        void CloseGate () {
            if (!_gateOpened) return;
            _gateOpened = false;
            pole.DOLocalRotate (Vector3.zero, .5f).SetEase (Ease.InBack).SetId (GetInstanceID ());
        }

        void ShowBorder (Transform target, Action onReach = null) {
            target.gameObject.Show ();
            const float duration = .2f;
            var seq = DOTween.Sequence ();
            seq.AppendCallback (() => DOVirtual.DelayedCall (duration / 2f, () => onReach?.Invoke ()));
            seq.Append (target.DOScale (Vector3.one, duration).From (.5f).SetEase (Ease.OutBack));
            seq.AppendCallback (() => {
                var particle = target.GetComponentInChildren<ParticleSystem> ();
                if (particle) {
                    particle.Play ();
                }
            });
        }

        public override void InstantShow (int lvl = 0) {
            _gateOpened = true;
            foreach (var border in borders) {
                border.gameObject.Show ();
            }
        }

        public override void TweenUpgrade (int lvl = 0) {
            foreach (var border in borders) {
                border.gameObject.Hide ();
            }
            TweenBuilding (building, () => {
                _gateOpened = true;
                ShowBorder (borders[0], () => ShowBorder (borders[1], () => ShowBorder (borders[2], () => ShowBorder (borders[3]))));
                ShowBorder (borders[4], () => ShowBorder (borders[5], () => ShowBorder (borders[6])));
            });
            if (lvl == 0) {
                ImmuneObject.StartFlash ();
            }
        }
    }
}