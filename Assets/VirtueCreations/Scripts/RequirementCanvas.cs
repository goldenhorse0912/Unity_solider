using System.Collections.Generic;
using UnityEngine;

namespace VIRTUE {
    public class RequirementCanvas : MonoBehaviour {
        public Transform parent;
        public Dictionary<Resource, RequiredResourceUI> RequirementsUI;

        void Awake () {
            RequirementsUI = new Dictionary<Resource, RequiredResourceUI> ();
        }
        
        public void ToggleVisibility (bool state) => GetComponent<Canvas> ().enabled = state;

        public void SetAmountByTypeOfResource (Resource typeOfResource, int amount) {
            if (RequirementsUI.TryGetValue (typeOfResource, out var ui)) {
                ui.SetAmount (amount);
            }
        }

        public void SetAmountByTypeOfResource (Resource typeOfResource, params int[] args) {
            if (RequirementsUI.TryGetValue (typeOfResource, out var ui)) {
                ui.SetAmount (args);
            }
        }
    }
}