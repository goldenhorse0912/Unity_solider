using UnityEditor;
using UnityEngine;

//DISPLAYNAME:ResetPosition

namespace HotTotemAssets.EpicMenu {
	public class Resetposition : EpicMenuAction {
		public override void Action (Ray sceneViewToEpicMenuCenterRay) {
			var dst = Selection.transforms;
			Undo.RecordObjects (dst, "Reset positions");
			foreach (var t in dst) {
				t.localPosition = Vector3.zero;
			}
		}
	}
}