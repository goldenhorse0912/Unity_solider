using UnityEngine;
using VIRTUE;

//DISPLAYNAME:AlignTransform

namespace HotTotemAssets.EpicMenu {
	public class Aligntransform : EpicMenuAction {
		public override void Action (Ray sceneViewToEpicMenuCenterRay) {
			Developer.PerformTransformAlign ();
		}
	}
}