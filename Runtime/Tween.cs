using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace GiveUsComponents {

    public delegate void Tweener(float t);

    public static class Tween {

        public static CancelableTask AlongCurve(
            MonoBehaviour master, Tweener tweener, 
            float duration = .2f, AnimationCurve curve = null, 
            CancelationToken isMasterCanceled = null
        ) {
            if (isMasterCanceled == null) isMasterCanceled = () => false; 
            if (curve == null) curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
            return CancelableTask.ByCoroutine(master,
                (isMeCanceled) => AlongCurveCoroutine(
                    duration, curve, tweener, 
                    () => isMeCanceled.Invoke() || isMasterCanceled.Invoke()
                )
            );
        }

        public static CancelableTask AlongLine01(
            MonoBehaviour master, Tweener tweener, 
            float duration = .2f, CancelationToken isMasterCanceled = null
        ) {
            if (isMasterCanceled == null) isMasterCanceled = () => false; 
            var curve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            return CancelableTask.ByCoroutine(master,
                (isMeCanceled) => AlongCurveCoroutine(
                    duration, curve, tweener, 
                    () => isMeCanceled.Invoke() || isMasterCanceled.Invoke()
                )
            );
        }

        private static IEnumerator AlongCurveCoroutine(
            float duration, AnimationCurve curve, Tweener tweener,
            CancelationToken isCanceled
        ) {
            float startTime  = Time.time;
            float finishTime = startTime + duration;
            for (float t = startTime; t < finishTime; t = Time.time) {
                tweener.Invoke(curve.Evaluate((t - startTime) / duration));
                if (isCanceled()) yield break;
                yield return new WaitForSeconds(.01f);
            }
            tweener.Invoke(curve.Evaluate(1f));
        }

        public static CancelableTask ScaleUniform(
            MonoBehaviour master, float to, float duration = .2f, 
            Transform target = null, AnimationCurve curve = null
        ) {
            if (target == null) target = master.transform;
            float from = target.localScale.x;
            return AlongCurve(
                master, 
                (t) => target.localScale = Mathx.LerpUniform(from, to, t), 
                duration, curve
            );
        }
    }

}