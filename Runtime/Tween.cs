using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GiveUsComponents {

    public delegate void Tweener(float t);

    public static class Tween {

        public static CancelableTask AlongCurve(
            MonoBehaviour master, Tweener tweener, 
            float time = .2f, AnimationCurve curve = null, 
            CancelationToken isMasterCanceled = null
        ) {
            if (isMasterCanceled == null) isMasterCanceled = () => false; 
            if (curve == null) curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            return CancelableTask.ByCoroutine(master,
                (isMeCanceled) => AlongCurveCoroutine(
                    time, curve, tweener, 
                    () => isMeCanceled.Invoke() || isMasterCanceled.Invoke()
                )
            );
        }

        public static CancelableTask AlongLine01(
            MonoBehaviour master, Tweener tweener, 
            float time = .2f, CancelationToken isMasterCanceled = null
        ) {
            if (isMasterCanceled == null) isMasterCanceled = () => false; 
            var curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            return CancelableTask.ByCoroutine(master,
                (isMeCanceled) => AlongCurveCoroutine(
                    time, curve, tweener, 
                    () => isMeCanceled.Invoke() || isMasterCanceled.Invoke()
                )
            );
        }

        private static IEnumerator AlongCurveCoroutine(
            float time, AnimationCurve curve, Tweener tweener,
            CancelationToken isCanceled
        ) {
            float startTime  = Time.time;
            float finishTime = startTime + time;
            for (float t = startTime; t < finishTime; t = Time.time) {
                tweener.Invoke(curve.Evaluate((t - startTime) / time));
                if (isCanceled()) yield break;
                yield return new WaitForSeconds(.01f);
            }
            tweener.Invoke(curve.Evaluate(1f));
        }

        public static Tweener Together(params Tweener[] tweeners) 
        => (t) => {
            foreach (var tw in tweeners) tw(t);
        };

        public static Tweener UniformScale(
            Transform target, float from, float to
        ) => (t) => target.localScale 
            = Vector3.one * ((1f - t) * from + t * to);

        public static Tweener Scale(Transform target, Vector3 from, Vector3 to) 
        => (t) => target.localScale = (1f - t) * from + t * to;

        public static Tweener LocalPosition(
            Transform target, Vector3 from, Vector3 to
        ) => (t) => target.localPosition = (1f - t) * from + t * to;

        public static Tweener Position(
            Transform target, Vector3 from, Vector3 to
        ) => (t) => target.position = (1f - t) * from + t * to;

        public static Tweener Rotation(
            Transform target, Quaternion from, Quaternion to
        ) => (t) => target.rotation = Quaternion.Slerp(from, to, t);

        public static Tweener LocalRotation(
            Transform target, Quaternion from, Quaternion to
        ) => (t) => target.localRotation = Quaternion.Slerp(from, to, t);

        public static Tweener WholeTransform(
            Transform target, Transform from, Transform to
        ) => Together(
            Position(target, from.position, to.position),
            Rotation(target, from.rotation, to.rotation),
            Scale(target, from.localScale, to.localScale)
        );


        [System.Flags]
        public enum TweenType {
            UniformScale   = 0b000000000001,
            LocalPosition  = 0b000000000010,
            WorldRotation  = 0b000000000100,
            Scale          = 0b000000001000,
            WorldPosition  = 0b000000010000,
            LocalRotation  = 0b000000100000,
            WholeTransform = 0b000001000000,
            // Together       = 0b000010000000,
            // Chain          = 0b000100000000,
            Nothing        = 0b000000000000,
            Everything     = 0b000001111111
        }

        [System.Serializable]
        public class Custom {
            public TweenType type = TweenType.UniformScale;
            public Transform target;
            public AnimationCurve curve 
                = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

            [ShowWhen("type", (ulong)TweenType.UniformScale, true)]
            public float uniformScaleFrom = 0f;
            [ShowWhen("type", (ulong)TweenType.UniformScale, true)]
            public float uniformScaleTo = 1f;

            [ShowWhen("type", (ulong)TweenType.LocalPosition, true)]
            public Vector3 localPositionFrom = Vector3.zero;
            [ShowWhen("type", (ulong)TweenType.LocalPosition, true)]
            public Vector3 localPositionTo = Vector3.right;

            [ShowWhen("type", (ulong)TweenType.WorldRotation, true)]
            public Quaternion worldRotationFrom = Quaternion.identity;
            [ShowWhen("type", (ulong)TweenType.WorldRotation, true)]
            public Quaternion worldRotationTo = Quaternion.identity;

            [ShowWhen("type", (ulong)TweenType.Scale, true)]
            public Vector3 scaleFrom = Vector3.zero;
            [ShowWhen("type", (ulong)TweenType.Scale, true)]
            public Vector3 scaleTo = Vector3.one;

            [ShowWhen("type", (ulong)TweenType.WorldPosition, true)]
            public Transform worldPositionFrom;
            [ShowWhen("type", (ulong)TweenType.WorldPosition, true)]
            public Transform worldPositionTo;

            [ShowWhen("type", (ulong)TweenType.LocalRotation, true)]
            public Quaternion localRotationFrom = Quaternion.identity;
            [ShowWhen("type", (ulong)TweenType.LocalRotation, true)]
            public Quaternion localRotationTo = Quaternion.identity;

            [ShowWhen("type", (ulong)TweenType.WholeTransform, true)]
            public Transform wholeTransformFrom;
            [ShowWhen("type", (ulong)TweenType.WholeTransform, true)]
            public Transform wholeTransformTo;

            // [ShowWhen("type", (ulong)TweenType.Together)]
            // public List<Custom> together;

            // [ShowWhen(typeof(TweenType), TweenType.Chain)]
            // public List<Custom> chain;

            public void Evaluate(float t) {
                if (this.target == null) return;
                if ((this.type & TweenType.UniformScale) > 0)
                    Tween.UniformScale(this.target, 
                        this.uniformScaleFrom, this.uniformScaleTo
                    )(t);
                if ((this.type & TweenType.LocalPosition) > 0)
                    Tween.LocalPosition(this.target, 
                        this.localPositionFrom, this.localPositionTo
                    )(t);
                if ((this.type & TweenType.WorldRotation) > 0)
                    Tween.Rotation(this.target, 
                        this.worldRotationFrom, this.worldRotationTo
                    )(t);
                if ((this.type & TweenType.Scale) > 0)
                    Tween.Scale(this.target, 
                        this.scaleFrom, this.scaleTo
                    )(t);
                if ((this.type & TweenType.WorldPosition) > 0
                    && this.worldPositionFrom != null
                    && this.worldPositionTo != null) 
                    Tween.Position(this.target, 
                        this.worldPositionFrom.position, 
                        this.worldPositionTo.position
                    )(t);
                if ((this.type & TweenType.LocalRotation) > 0)
                    Tween.LocalRotation(this.target, 
                        this.localRotationFrom, 
                        this.localRotationTo
                    )(t);
                if ((this.type & TweenType.WholeTransform) > 0)
                    Tween.WholeTransform(this.target, 
                        this.wholeTransformFrom, 
                        this.wholeTransformTo
                    )(t);
                // if ((this.type & TweenType.Together) > 0)
                //     foreach (var cstm in this.together)
                //         cstm.Evaluate(t);
                // if ((this.type & TweenType.Chain) > 0)
                //     Chain(this.chain)(t);
            }
        }
    }

}