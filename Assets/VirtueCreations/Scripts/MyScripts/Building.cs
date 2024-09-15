using System;
using System.Collections.Generic;
using CodeMonkey.HealthSystemCM;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
    [RequireComponent (typeof (HealthSystemComponent))]
    public class Building : Abst {
        Destroyable[] _allDestroyAbles;

        internal HealthSystemComponent Hsc;
        [SerializeField]
        internal bool isPlayer;
        [SerializeField]
        GameObject healthCanvas;
        [SerializeField, ShowIf ("isPlayer")]
        VoidEvent spawnEvent;
        [SerializeField, HideIf ("isPlayer")]
        int collectibleValue;
        [SerializeField]
        float immuneDuration = 5f;
        [SerializeField]
        internal List<GameObject> buildingGfx;
        [SerializeField]
        int[] healthByLvl;

        internal ImmuneObject ImmuneObject;

        protected override void OnAwake () {
            base.OnAwake ();
            TryGetComponent (out Hsc);
            TryGetComponent (out ImmuneObject);
            _allDestroyAbles = GetComponentsInChildren<Destroyable> (true);
        }

        void Start () => OnStart ();

        void Die (object sender, EventArgs e) => OnDie ();
        void Damaged (object sender, EventArgs e) => OnDamage ();

        protected virtual void OnStart () {
            Hsc.GetHealthSystem ().OnDead += Die;
            Hsc.GetHealthSystem ().OnDamaged += Damaged;
            Manager.Instance.TroopManager.ModifyBuildingList (this, true, isPlayer);
            Hsc.GetHealthSystem ().TurnOnImmunity ();
            DOVirtual.DelayedCall (immuneDuration, Hsc.GetHealthSystem ().TurnOffImmunity);
            if (isPlayer) {
                spawnEvent.Raise ();
            }
        }

        protected virtual void OnDie () {
            healthCanvas.Hide ();
            Manager.Instance.CameraController.ShakeCamera ();
            Manager.Instance.TroopManager.ModifyBuildingList (this, false, isPlayer);
            BuildingManager.CheckBuildings ();
            foreach (var destroyable in _allDestroyAbles) {
                destroyable.GetDestroyed (CachedGameObject);
            }
            if (isPlayer) {
                MainButtons.Instance.CheckTargetType (CachedTransform, false);
                if (TryGetComponent (out Collider col)) {
                    col.enabled = false;
                }
            } else {
                ResourceManager.Instance.SpawnCoinAt (CachedTransform.position, collectibleValue);
            }
        }

        protected virtual void OnDamage () {
            if (isPlayer) {
                TutorialScript.Instance.ShowFreeEnergyBtn ();
            }
        }

        internal void TweenBuilding (Transform target, Action onComplete = null) {
            Manager.Instance.AudioManager.Play (SoundConstants.Clip_UnlockItem);
            var seq = DOTween.Sequence ();
            target.position = transform.position + Vector3.up * 1.5f;
            seq.SetUpdate (true);
            seq.Append (target.DOScale (Vector3.one, .3f).From (.5f).SetEase (Ease.OutBack));
            seq.Append (target.DOMoveY (0, .3f).SetEase (Ease.InOutExpo));
            seq.AppendCallback (() => {
                onComplete?.Invoke ();
                var particle = target.GetComponentInChildren<ParticleSystem> ();
                if (particle) {
                    particle.Play ();
                }
            });
        }

        public override void InstantShow (int lvl = 0) {
            foreach (var gfx in buildingGfx) {
                gfx.Hide ();
            }
            buildingGfx[lvl].Show ();
        }

        public override void TweenUpgrade (int lvl = 0) {
            if (lvl >= buildingGfx.Count) return;
            foreach (var gfx in buildingGfx) {
                gfx.Hide ();
            }
            buildingGfx[lvl].Show ();
            TweenBuilding (buildingGfx[lvl].transform);
            Hsc.GetHealthSystem ().SetHealth (healthByLvl[lvl]);
            ImmuneObject.StartFlash ();
        }
    }
}