using UnityEngine;

namespace CodeMonkey.HealthSystemCM {
    /// <summary>
    /// Adds a HealthSystem to a Game Object
    /// </summary>
    public class HealthSystemComponent : MonoBehaviour, IGetHealthSystem {
        [Tooltip ("Maximum Health amount")]
        [SerializeField]
        float healthAmountMax = 100f;

        [Tooltip ("Starting Health amount, leave at 0 to start at full health.")]
        [SerializeField]
        float startingHealthAmount;

        HealthSystem healthSystem;

        void Awake () {
            // Create Health System
            healthSystem = new HealthSystem (healthAmountMax);
            if (startingHealthAmount != 0) {
                healthSystem.SetHealth (startingHealthAmount);
            }
        }

        /// <summary>
        /// Get the Health System created by this Component
        /// </summary>
        public HealthSystem GetHealthSystem () {
            return healthSystem;
        }

        public void HealComplete () {
            GetHealthSystem ().HealComplete ();
        }
    }
}