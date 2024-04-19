using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GiveUsComponents {

    public struct TransformSnapshot {
        public Vector3 localPosition;
        public Vector3 localScale;
        public Quaternion localRotation;

        public static implicit operator TransformSnapshot(Transform t) 
        => new TransformSnapshot() {
            localPosition = t.localPosition,
            localScale = t.localScale,
            localRotation = t.localRotation
        };

        public void Set(Transform t) {
            t.localPosition = this.localPosition;
            t.localScale = this.localScale;
            t.localRotation = this.localRotation;
        }
    }


    public static class Mathx {

    // Interpolation
        public static float Lerp(float from, float to, float t)
            => (1f - t) * from + t * to;
        public static Vector3 LerpUniform(float from, float to, float t)
            => Vector3.one * ((1f - t) * from + t * to);
        public static Vector3 Lerp(Vector3 from, Vector3 to, float t)
            => (1f - t) * from + t * to;
        public static Color Lerp(Color from, Color to, float t)
            => (1f - t) * from + t * to;
        public static Vector2 Lerp(Vector2 from, Vector2 to, float t)
            => (1f - t) * from + t * to;
        public static Quaternion Slerp(Quaternion from, Quaternion to, float t)
            => Quaternion.Slerp(from, to, t);
        public static TransformSnapshot Lerp(
            TransformSnapshot from, TransformSnapshot to, float t
        ) => new TransformSnapshot() {
            localPosition = Lerp(from.localPosition, to.localPosition, t),
            localScale = Lerp(from.localScale, to.localScale, t),
            localRotation = Slerp(from.localRotation, to.localRotation, t)
        };
    //
    }
}
