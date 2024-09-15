using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace VIRTUE {
    public class MyAdRewards : MonoBehaviour {
        [SerializeField]
        TextMeshProUGUI energyAmountText, goldAmountText;
        [SerializeField]
        List<int> energyAmounts, goldAmounts;
        int _energyIndex, _goldIndex;

        public void SetButtons () {
            energyAmountText.text = energyAmounts[_energyIndex].ToString ();
            goldAmountText.text = goldAmounts[_goldIndex].ToString ();
        }

        public void CollectEnergy () {
            MainButtons.Instance.ChangeEnergyAmount (energyAmounts[_energyIndex]);
            if (_energyIndex < energyAmounts.Count) {
                _energyIndex++;
            }
        }

        public void CollectGold () {
            // Manager.Instance.UIManager.SetAmountByTypeOfResource ();
        }
    }
}