using System;
using UnityEngine;

namespace CaseStudy.WheelSpin
{
    [Serializable]
    public struct ItemViewSettings
    {
        public Vector2 Size;
        public float Rotation;
        public Vector2 Offset;

        public static readonly Vector2 DefaultSize = new Vector2(100f, 100f);

        public static ItemViewSettings Default => new ItemViewSettings
        {
            Size = DefaultSize,
            Rotation = 0f,
            Offset = Vector2.zero
        };
    }
}