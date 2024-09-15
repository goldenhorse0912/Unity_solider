using UnityEngine;

//DISPLAYNAME:AlignTools

namespace HotTotemAssets.EpicMenu {
	public class Aligntools : EpicMenuAction {
		public override void Action (Ray sceneViewToEpicMenuCenterRay) {
			EpicMenuStarter.ActivateMenu ("AlignTools");
		}
	}
}