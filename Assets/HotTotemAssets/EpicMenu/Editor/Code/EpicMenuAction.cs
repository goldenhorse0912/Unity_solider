using System;
using UnityEngine;

namespace HotTotemAssets.EpicMenu {
    [Serializable]
    public class EpicMenuAction {
        public virtual void Action (Ray screenToMouseRay) { }
    }
}