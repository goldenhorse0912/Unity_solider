using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.OnScreen;

namespace VIRTUE {
    public class FloatingJoystick : OnScreenControl, IPointerDownHandler, IDragHandler, IPointerUpHandler {
        [InputControl (layout = "Vector2")]
        [SerializeField]
        string m_ControlPath;

        public float Horizontal { get { return (snapX) ? SnapFloat (input.x, AxisOptions.Horizontal) : input.x; } }
        public float Vertical { get { return (snapY) ? SnapFloat (input.y, AxisOptions.Vertical) : input.y; } }
        public Vector2 Direction { get { return new Vector2 (Horizontal, Vertical); } }

        public float HandleRange {
            get { return handleRange; }
            set { handleRange = Mathf.Abs (value); }
        }

        public float DeadZone {
            get { return deadZone; }
            set { deadZone = Mathf.Abs (value); }
        }

        public AxisOptions AxisOptions { get { return AxisOptions; } set { axisOptions = value; } }
        public bool SnapX { get { return snapX; } set { snapX = value; } }
        public bool SnapY { get { return snapY; } set { snapY = value; } }

        [SerializeField]
        float moveThreshold = 1;
        [SerializeField]
        float handleRange = 1;
        [SerializeField]
        float deadZone = 0;
        [SerializeField]
        AxisOptions axisOptions = AxisOptions.Both;
        [SerializeField]
        bool snapX;
        [SerializeField]
        bool snapY;

        [SerializeField]
        protected RectTransform background;
        [SerializeField]
        RectTransform handle;
        RectTransform baseRect;

        Canvas canvas;
        Camera cam;

        Vector2 input = Vector2.zero;
        Vector2 m_PointerDownPos;

        protected virtual void Start () {
            HandleRange = handleRange;
            DeadZone = deadZone;
            baseRect = GetComponent<RectTransform> ();
            canvas = GetComponentInParent<Canvas> ();
            if (canvas == null) this.LogError ("The Joystick is not placed inside a canvas");
            Vector2 center = new Vector2 (0.5f, 0.5f);
            background.pivot = center;
            handle.anchorMin = center;
            handle.anchorMax = center;
            handle.pivot = center;
            handle.anchoredPosition = Vector2.zero;
            background.gameObject.SetActive (false);
        }

        public void OnPointerDown (PointerEventData eventData) {
            background.anchoredPosition = ScreenPointToAnchoredPosition (eventData.position);
            background.gameObject.SetActive (true);
            OnDrag (eventData);
        }

        public void OnDrag (PointerEventData eventData) {
            cam = null;
            if (canvas.renderMode == RenderMode.ScreenSpaceCamera) cam = canvas.worldCamera;
            Vector2 position = RectTransformUtility.WorldToScreenPoint (cam, background.position);
            Vector2 radius = background.sizeDelta / 2;
            input = (eventData.position - position) / (radius * canvas.scaleFactor);
            FormatInput ();
            HandleInput (input.magnitude, input.normalized, radius, cam);
            handle.anchoredPosition = input * radius * handleRange;
            SendValueToControl (input);
        }

        protected virtual void HandleInput (float magnitude, Vector2 normalised, Vector2 radius, Camera cam) {
            if (magnitude > moveThreshold) {
                Vector2 difference = normalised * (magnitude - moveThreshold) * radius;
                background.anchoredPosition += difference;
            }
            if (magnitude > deadZone) {
                if (magnitude > 1) input = normalised;
            } else
                input = Vector2.zero;
        }

        void FormatInput () {
            if (axisOptions == AxisOptions.Horizontal)
                input = new Vector2 (input.x, 0f);
            else if (axisOptions == AxisOptions.Vertical) input = new Vector2 (0f, input.y);
        }

        float SnapFloat (float value, AxisOptions snapAxis) {
            if (value == 0) return value;
            if (axisOptions == AxisOptions.Both) {
                float angle = Vector2.Angle (input, Vector2.up);
                if (snapAxis == AxisOptions.Horizontal) {
                    if (angle < 22.5f || angle > 157.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                } else if (snapAxis == AxisOptions.Vertical) {
                    if (angle > 67.5f && angle < 112.5f)
                        return 0;
                    else
                        return (value > 0) ? 1 : -1;
                }
                return value;
            } else {
                if (value > 0) return 1;
                if (value < 0) return -1;
            }
            return 0;
        }

        public virtual void OnPointerUp (PointerEventData eventData) {
            background.gameObject.SetActive (false);
            input = Vector2.zero;
            handle.anchoredPosition = Vector2.zero;
            SendValueToControl (input);
        }

        protected Vector2 ScreenPointToAnchoredPosition (Vector2 screenPosition) {
            Vector2 localPoint = Vector2.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle (baseRect, screenPosition, cam, out localPoint)) {
                Vector2 pivotOffset = baseRect.pivot * baseRect.sizeDelta;
                return localPoint - (background.anchorMax * baseRect.sizeDelta) + pivotOffset;
            }
            return Vector2.zero;
        }

        protected override string controlPathInternal {
            get => m_ControlPath;
            set => m_ControlPath = value;
        }
    }
}