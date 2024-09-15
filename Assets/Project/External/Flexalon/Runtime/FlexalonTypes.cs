using UnityEngine;

namespace Flexalon
{
    /// <summary> Represents an axis and direction. </summary>
    public enum Direction
    {
        PositiveX = 0,
        NegativeX = 1,
        PositiveY = 2,
        NegativeY = 3,
        PositiveZ = 4,
        NegativeZ = 5
    };

    /// <summary> Represents an axis. </summary>
    public enum Axis
    {
        X = 0,
        Y = 1,
        Z = 2
    };

    /// <summary> Represents a direction to align. </summary>
    public enum Align
    {
        Start = 0,
        Center = 1,
        End = 2
    };

    /// <summary> Represents a plane along two axes. </summary>
    public enum Plane
    {
        XY = 0,
        XZ = 1,
        YZ = 2
    }

    /// <summary> Determines how a FlexalonObject should be sized. </summary>
    public enum SizeType
    {
        /// <summary> Specify a fixed size value. </summary>
        Fixed = 0,

        /// <summary> Specify a factor of the space allocated by the parent layout.
        /// For example, 0.5 will fill half of the space. </summary>
        Fill = 1,

        /// <summary> The size is determined by the Adapter and attached Unity
        /// components such as MeshRenderer, SpriteRenderer, TMP_Text, RectTransform, and Colliders.
        /// An empty GameObject gets a size of 1. </summary>
        Component = 2,

        /// <summary> The size determined by the layout's algorithm. </summary>
        Layout = 3
    };

    /// <summary> Six floats representing right, left, top, bottom, back, front.</summary>
    [System.Serializable]
    public struct Directions
    {
        public static Directions zero => new Directions(new float[]{ 0, 0, 0, 0, 0, 0 });

        public float[] Values;

        public Directions(float[] values)
        {
            Values = values;
        }

        public float this[int key]
        {
            get => Values[key];
            set => Values[key] = value;
        }

        public float this[Direction key]
        {
            get => Values[(int)key];
            set => Values[(int)key] = value;
        }

        public Vector3 Size => new Vector3(
            Values[0] + Values[1], Values[2] + Values[3], Values[4] + Values[5]);

        public Vector3 Center => new Vector3(
            (Values[0] - Values[1]) * 0.5f, (Values[2] - Values[3]) * 0.5f, (Values[4] - Values[5]) * 0.5f);
    }
}