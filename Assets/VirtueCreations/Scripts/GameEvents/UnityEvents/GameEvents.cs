using System;
using UnityEngine;
using UnityEngine.Events;

namespace VIRTUE {
    [Serializable]
    public class UnityVoidEvent : UnityEvent<Void> { }

    [Serializable]
    public class UnityBoolEvent : UnityEvent<bool> { }

    [Serializable]
    public class UnityIntEvent : UnityEvent<int> { }

    [Serializable]
    public class UnityFloatEvent : UnityEvent<float> { }

    [Serializable]
    public class UnityStringEvent : UnityEvent<string> { }

    [Serializable]
    public class UnityVector2Event : UnityEvent<Vector2> { }

    [Serializable]
    public class UnityVector3Event : UnityEvent<Vector3> { }

    [Serializable]
    public class UnityColorEvent : UnityEvent<Color> { }

    [Serializable]
    public class UnityGameObjectEvent : UnityEvent<GameObject> { }

    [Serializable]
    public class UnityTransformEvent : UnityEvent<Transform> { }
}