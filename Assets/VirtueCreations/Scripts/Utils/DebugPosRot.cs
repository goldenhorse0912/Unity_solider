using System;
using UnityEngine;

namespace VIRTUE {
	[ExecuteAlways]
	public class DebugPosRot : MonoBehaviour {
		[SerializeField]
		Vector3 position;

		[SerializeField]
		Vector3 localPosition;

		void Update () {
			position = transform.position;
			localPosition = transform.localPosition;
		}
	}
}