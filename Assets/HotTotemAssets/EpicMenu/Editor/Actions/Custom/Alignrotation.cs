using UnityEngine;
using VIRTUE;

//DISPLAYNAME:AlignRotation

namespace HotTotemAssets.EpicMenu {
	public class Alignrotation : EpicMenuAction {
		public override void Action (Ray sceneViewToEpicMenuCenterRay) {
			Developer.PerformRotationAlign ();
		}
	}
}