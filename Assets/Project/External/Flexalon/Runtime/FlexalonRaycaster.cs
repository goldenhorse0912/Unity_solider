#if UNITY_PHYSICS

using System.Collections.Generic;
using UnityEngine;

namespace Flexalon
{
    internal class FlexalonRaycaster
    {
        public Vector3 hitPosition;

        private RaycastHit[] _raycastHits = new RaycastHit[10];
        private int _raycastFrame = 0;
        private FlexalonInteractable _hitInteractable;
        private readonly Dictionary<Collider, FlexalonInteractable> _colliderToInteractable = new Dictionary<Collider, FlexalonInteractable>();

        public void Register(Collider collider, FlexalonInteractable interactable)
        {
            _colliderToInteractable[collider] = interactable;
        }

        public void Unregister(Collider collider)
        {
            _colliderToInteractable.Remove(collider);
        }

        public bool IsHit(Ray ray, FlexalonInteractable interactable)
        {
            // Check if we've already casted this frame.
            if (_raycastFrame != Time.frameCount)
            {
                _hitInteractable = null;
                _raycastFrame = Time.frameCount;
                int hits = Physics.RaycastNonAlloc(ray, _raycastHits, 1000);
                float minDistance = float.MaxValue;

                // Find the nearest hit interactable.
                for (int i = 0; i < hits; i++)
                {
                    var hit = _raycastHits[i];
                    if (hit.distance < minDistance && _colliderToInteractable.TryGetValue(hit.collider, out var hitInteractable))
                    {
                        _hitInteractable = hitInteractable;
                        minDistance = hit.distance;
                        hitPosition = hit.point;
                    }
                }
            }

            return _hitInteractable == interactable;
        }
    }
}

#endif