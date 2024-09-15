using System;
using CodeMonkey.HealthSystemCM;
using UnityEngine;

namespace VIRTUE {
    [RequireComponent (typeof (HealthSystemComponent))]
    public abstract class Troop : CachedMonoBehaviour {
        [SerializeField]
        internal bool isPlayer;
        internal HealthSystemComponent Hsc;

        [SerializeField]
        VoidEvent spawnEvent;

        protected override void OnAwake () {
            base.OnAwake ();
            TryGetComponent (out Hsc);
        }

        void Start () => OnStart ();

        void Die (object sender, EventArgs e) => OnDie ();
        void Damaged (object sender, EventArgs e) => OnDamage ();

        protected virtual void OnStart () {
            Hsc.GetHealthSystem ().OnDead += Die;
            Hsc.GetHealthSystem ().OnDamaged += Damaged;
            Manager.Instance.TroopManager.AddMeToTroopList (this, isPlayer);
            spawnEvent.Raise ();
        }

        protected virtual void OnDamage () { }

        protected virtual void OnDie () {
            Manager.Instance.TroopManager.RemoveMeFromTroopList (this, isPlayer);
            GetComponent<Collider> ().enabled = false;
        }
    }
}