#if FLEXALON_OCULUS

using UnityEngine;
using Oculus.Interaction;

namespace Flexalon
{
    public class FlexalonOculusInputProvider : MonoBehaviour, InputProvider
    {
        public InputMode InputMode => InputMode.External;
        public bool Active => _state == InteractableState.Select;
        public Ray Ray => default;
        public GameObject ExternalFocusedObject => (_state == InteractableState.Hover || Active) ? gameObject : null;

        private IInteractable _interactable;
        private InteractableState _state => _interactable?.State ?? InteractableState.Disabled;

        public void Awake()
        {
            _interactable = GetComponent<IInteractable>();
            if (_interactable == null)
            {
                Debug.LogWarning("FlexalonOculusInputProvider should be placed next to Oculus Interactable component.");
            }
        }
    }
}

#endif