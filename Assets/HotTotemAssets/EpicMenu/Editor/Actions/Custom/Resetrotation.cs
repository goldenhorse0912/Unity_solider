using UnityEditor;
using UnityEngine;

//DISPLAYNAME:ResetRotation

namespace HotTotemAssets.EpicMenu {
	public class Resetrotation : EpicMenuAction {
		public override void Action (Ray sceneViewToEpicMenuCenterRay) {
			var dst = Selection.transforms;
			Undo.RecordObjects (dst, "Reset rotations");
			foreach (var t in Selection.transforms) {
				t.localRotation = Quaternion.Euler (Vector3.zero);
			}
		}
	}
}