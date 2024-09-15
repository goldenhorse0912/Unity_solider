using UnityEditor;
using UnityEngine;

//DISPLAYNAME:ResetTransform

namespace HotTotemAssets.EpicMenu {
	public class Resettransform : EpicMenuAction {
		public override void Action (Ray sceneViewToEpicMenuCenterRay) {
			var dst = Selection.transforms;
			Undo.RecordObjects (dst, "Reset transforms");
			foreach (var t in Selection.transforms) {
				t.localPosition = Vector3.zero;
				t.localRotation = Quaternion.Euler (Vector3.zero);
				t.localScale = Vector3.one;
			}
		}
	}
}