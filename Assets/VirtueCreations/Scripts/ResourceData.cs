using System;
using System.Collections.Generic;
using System.Linq;

namespace VIRTUE {
    [Serializable]
    public class ResourceData {
        public List<RequiredResources> resources;

        public float Progress () {
            var qty = resources.Sum (x => x.qty);
            var maxQty = resources.Sum (x => x.maxQty);
            return Helper.ClampToPer (qty, maxQty);
        }

        public void Init () {
            var length = resources.Count;
            for (int i = 0; i < length; i++) {
                resources[i].maxQty = resources[i].qty;
            }
        }

        public bool IsUnlocked () => Math.Abs (Progress () - 1f) == 0;
    }
}