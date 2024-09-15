using UnityEngine;

namespace VIRTUE {
    public class ScaleByDistance : MonoBehaviour {
        [SerializeField]
        float minDistance = 5f;

        [SerializeField]
        float maxDistance = 20f;

        [SerializeField]
        float minScale = 0.5f;

        [SerializeField]
        float maxScale = 2.0f;

        float _initialScale;

        void Awake () {
            // Store the initial scale of the object
            _initialScale = transform.localScale.x;
        }

        void LateUpdate () {
            // Calculate the distance between the player and the object
            var distance = transform.DistanceTo (Helper.Camera.transform.position);
            // var distance = transform.DistanceTo (PlayerController.Instance.CachedTransform.position);

            // If the object is far from the player (beyond maxDistance), set the scale to zero
            if (distance > maxDistance) {
                transform.localScale = Vector3.zero;
            } else {
                // Clamp the distance between minDistance and maxDistance
                distance = Mathf.Clamp (distance, minDistance, maxDistance);

                // Calculate the normalized scale value based on the distance
                var normalizedScale = Mathf.InverseLerp (maxDistance, minDistance, distance);

                // Calculate the final scale value between minScale and maxScale
                var finalScale = Mathf.Lerp (minScale, maxScale, normalizedScale);

                // Apply the scale to the object
                transform.localScale = new Vector3 (_initialScale * finalScale, _initialScale * finalScale, _initialScale * finalScale);
            }
        }
    }
}