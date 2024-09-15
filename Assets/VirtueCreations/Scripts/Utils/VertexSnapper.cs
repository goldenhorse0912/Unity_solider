using Sirenix.OdinInspector;
using UnityEngine;

namespace VIRTUE {
	public class VertexSnapper : MonoBehaviour {
		[SerializeField]
		Transform targetSnapVertex;

		[SerializeField]
		Transform selectionSnapVertex;
		
		[Button]
		void Snap () {
			// Vector3 positionBeforeSnap = transform.position;
			Vector3 offset = targetSnapVertex.position - selectionSnapVertex.position;
			Vector3 newCenter = transform.position + offset;
			transform.position = newCenter;
		}
	}
}