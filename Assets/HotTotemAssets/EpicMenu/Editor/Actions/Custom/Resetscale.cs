using UnityEditor;
using UnityEngine;

//DISPLAYNAME:ResetScale

namespace HotTotemAssets.EpicMenu {
	public class Resetscale : EpicMenuAction {
		public override void Action (Ray sceneViewToEpicMenuCenterRay) {
			var dst = Selection.transforms;
			Undo.RecordObjects (dst, "Reset scales");
			foreach (var t in Selection.transforms) {
				t.localScale = Vector3.one;
			}
		}
	}
}