using System;

namespace VIRTUE {
    [Serializable]
    public class RequiredResources {
        public Resource typeOfResource;
        public int qty;
        public int maxQty;

        public void Add (int amount) {
            qty += amount;
        }
    }
}