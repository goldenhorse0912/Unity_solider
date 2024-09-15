using UnityEngine;

//DISPLAYNAME:ResetTools

namespace HotTotemAssets.EpicMenu {
	public class Resettools : EpicMenuAction {
		public override void Action (Ray sceneViewToEpicMenuCenterRay) {
			EpicMenuStarter.ActivateMenu ("ResetTools");
		}
	}
}