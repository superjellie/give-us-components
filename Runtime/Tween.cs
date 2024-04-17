using System.Collections;
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

        public static Tweener Scale(
            Transform target, Vector3 from, Vector3 to
        ) => (t) => target.localScale = (1f - t) * from + t * to;

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

    }
}