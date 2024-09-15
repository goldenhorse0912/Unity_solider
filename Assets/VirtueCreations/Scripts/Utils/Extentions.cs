using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Object = UnityEngine.Object;

namespace VIRTUE {
    public static class Extentions {
#if UNITY_EDITOR
        public static void RemoveNullEvents (this UnityEventBase arg) {
            for (int i = 0; i < arg.GetPersistentEventCount (); i++) {
                var target = arg.GetPersistentTarget (i);
                if (target == null) {
                    UnityEditor.Events.UnityEventTools.RemovePersistentListener (arg, i);
                }
            }
        }

        public static void RemoveAllEvents (this UnityEventBase arg) {
            for (int i = 0; i < arg.GetPersistentEventCount (); i++) {
                UnityEditor.Events.UnityEventTools.RemovePersistentListener (arg, i);
            }
        }

        public static void AddPersistentListener (this UnityEvent unityEvent, UnityAction call) {
            UnityEditor.Events.UnityEventTools.AddPersistentListener (unityEvent, call);
        }

        public static void AddIntPersistentListener (this UnityEventBase unityEvent, UnityAction<int> call, int argument) {
            UnityEditor.Events.UnityEventTools.AddIntPersistentListener (unityEvent, call, argument);
        }

        public static void SetDirty (this Object obj) {
            UnityEditor.EditorUtility.SetDirty (obj);
        }
#endif

        public static Bounds GetBounds (this GameObject original) {
            var referenceTransform = original.transform;
            var b = new Bounds (Vector3.zero, Vector3.zero);
            RecurseEncapsulate (referenceTransform, ref b);
            return b;

            void RecurseEncapsulate (Transform child, ref Bounds bounds) {
                var mesh = child.GetComponent<MeshFilter> ();
                if (mesh) {
                    var lsBounds = mesh.sharedMesh.bounds;
                    var wsMin = child.TransformPoint (lsBounds.center - lsBounds.extents);
                    var wsMax = child.TransformPoint (lsBounds.center + lsBounds.extents);
                    bounds.Encapsulate (referenceTransform.InverseTransformPoint (wsMin));
                    bounds.Encapsulate (referenceTransform.InverseTransformPoint (wsMax));
                }
                foreach (Transform grandChild in child.transform) {
                    RecurseEncapsulate (grandChild, ref bounds);
                }
            }
        }

        public static Vector2 With (this Vector2 original, float? x = null, float? y = null) => new(x ?? original.x, y ?? original.y);
        public static Vector3 With (this Vector3 original, float? x = null, float? y = null, float? z = null) => new(x ?? original.x, y ?? original.y, z ?? original.z);
        public static Color32 With (this Color32 original, byte? r = null, byte? g = null, byte? b = null, byte? a = null) => new(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);
        public static Color With (this Color original, float? r = null, float? g = null, float? b = null, float? a = null) => new(r ?? original.r, g ?? original.g, b ?? original.b, a ?? original.a);

        public static string ToTimerString (this float original) {
            var seconds = (int)(original % 60);
            var minutes = (int)(original / 60) % 60;
            return $"{minutes:00} : {seconds:00}";
        }

        public static int ToMilliseconds (this int original) => original * 1000;
        public static int ToMilliseconds (this float original) => (int)(original * 1000);
        public static int ToInt<TEnum> (this TEnum original) where TEnum : Enum => Convert.ToInt32 (original);
        public static byte ToByte<TEnum> (this TEnum original) where TEnum : Enum => Convert.ToByte (original);
        public static TEnum ToEnum<TEnum> (this int original) where TEnum : Enum => (TEnum)Enum.ToObject (typeof (TEnum), original);

        public static void PlayIfNotPlaying (this AudioSource original) {
            if (!original.isPlaying) original.Play ();
        }

        public static void StopAndClear (this ParticleSystem original, bool withChildren = true) => original.Stop (withChildren, ParticleSystemStopBehavior.StopEmittingAndClear);
        public static void StopEmission (this ParticleSystem original, bool withChildren = true) => original.Stop (withChildren, ParticleSystemStopBehavior.StopEmitting);

        public static void Show (this GameObject original) => original.SetActive (true);
        public static void Hide (this GameObject original) => original.SetActive (false);

        public static Vector3 DirectionTo (this Vector3 source, Vector3 destination) => Vector3.Normalize (destination - source);
        public static Vector3 DirectionTo (this Vector3 source, Transform destination) => Vector3.Normalize (destination.position - source);
        public static Vector3 DirectionTo (this Transform source, Vector3 destination) => Vector3.Normalize (destination - source.position);
        public static Vector3 DirectionTo (this Transform source, Transform destination) => Vector3.Normalize (destination.position - source.position);

        public static float DistanceTo (this Vector3 source, Vector3 destination) => Vector3.Distance (source, destination);
        public static float DistanceTo (this Vector3 source, Transform destination) => Vector3.Distance (source, destination.position);
        public static float DistanceTo (this Transform source, Vector3 destination) => Vector3.Distance (source.position, destination);
        public static float DistanceTo (this Transform source, Transform destination) => Vector3.Distance (source.position, destination.position);

        public static Vector3 RandomPoint (this Bounds bounds) {
            return Vector3.zero.With (
                                      UnityEngine.Random.Range (bounds.min.x, bounds.max.x),
                                      UnityEngine.Random.Range (bounds.min.y, bounds.max.y),
                                      UnityEngine.Random.Range (bounds.min.z, bounds.max.z));
        }

        static IList<T> Shuffle<T> (this IList<T> list, int size) {
            var rnd = new System.Random ();
            var res = new T[size];
            res[0] = list[0];
            for (var i = 1; i < size; i++) {
                var j = rnd.Next (i);
                res[i] = res[j];
                res[j] = list[i];
            }
            return res;
        }

        public static IList<T> Shuffle<T> (this IList<T> list) => list.Shuffle (list.Count);

        public static T Random<T> (this IList<T> original) {
            if (original.Count == 0) {
                // Debug.LogError ("Trying to get random element but there is no element at all");
                return default;
            }
            return original[UnityEngine.Random.Range (0, original.Count)];
        }

        public static int Random<T> (this IList<T> original, int previous) {
            if (original.Count == 0) {
                return default;
            }
            return (previous + UnityEngine.Random.Range (1, original.Count)) % original.Count;
        }

        public static T FindNearest<T> (this IEnumerable<T> list, Transform original) where T : Component {
            T nearestObj = null;
            var distance = Mathf.Infinity;
            foreach (var t in list) {
                var difference = Vector3.Distance (original.position, t.transform.position);
                if (difference < distance) {
                    distance = difference;
                    nearestObj = t;
                }
            }
            return nearestObj;
        }

        public static T FindNearest<T> (this IEnumerable<T> collection, Vector3 position) where T : Component {
            T nearestObject = null;
            float nearestDistance = Mathf.Infinity;
            foreach (var obj in collection) {
                if (obj == null) continue;
                float distance = Vector3.Distance (obj.transform.position, position);
                if (distance < nearestDistance) {
                    nearestObject = obj;
                    nearestDistance = distance;
                }
            }
            return nearestObject;
        }

        public static bool IsPointerOverUIObject (this EventSystem original) {
            var eventDataCurrentPosition = new PointerEventData (original) {
                position = new Vector2 (Input.mousePosition.x, Input.mousePosition.y)
            };
            var results = new List<RaycastResult> ();
            original.RaycastAll (eventDataCurrentPosition, results);
            bool flag = false;
            foreach (var raycastResult in results) {
                if (raycastResult.gameObject.layer != 13) {
                    flag = true;
                    break;
                }
            }
            return results.Count > 0 && flag;
        }
    }
}