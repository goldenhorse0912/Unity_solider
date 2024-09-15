using UnityEngine;
using VIRTUE;

//DISPLAYNAME:AlignPosition

namespace HotTotemAssets.EpicMenu {
	public class Alignposition : EpicMenuAction {
		public override void Action (Ray sceneViewToEpicMenuCenterRay) {
			Developer.PerformPositionAlign ();
		}
	}
}