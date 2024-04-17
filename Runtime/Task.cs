using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GiveUsComponents {

    public class Task : CustomYieldInstruction {
        
        public /* readonly */ bool done = false; 
        public /* listen-only */ UnityEvent onDone = new UnityEvent();

        public override bool keepWaiting => !this.done;

        public void Done() {
            this.done = true;
            this.onDone.Invoke();
        }

        public static Task ByCoroutine(
            MonoBehaviour master, System.Func<IEnumerator> crtn
        ) {
            var task = new Task();
            master.StartCoroutine(task.WaitForCoroutine(master, crtn));
            return task;
        }

        private IEnumerator WaitForCoroutine(
            MonoBehaviour master, System.Func<IEnumerator> crtn
        ) {
            yield return master.StartCoroutine(crtn());
            this.Done();
        }

        public static Task ByEvent(
            MonoBehaviour master, UnityEvent evt
        ) {
            var task = new Task();
            UnityAction listener = null;
            listener = () => {
                evt.RemoveListener(listener);
                task.Done();
            };
            evt.AddListener(listener);
            return task;
        }
    }

    public delegate bool CancelationToken();
    public class CancelableTask : Task {
        
        public /* readonly */ bool canceled = false; 
        public /* readonly */ UnityEvent onCancel = new UnityEvent(); 

        public void Cancel() {
            this.canceled = true;
            this.onCancel.Invoke();
        }

        public static CancelableTask ByCoroutine(
            MonoBehaviour master, 
            System.Func<CancelationToken, IEnumerator> crtn
        ) {
            var task = new CancelableTask();
            master.StartCoroutine(
                task.WaitForCoroutine(master, () => crtn(() => task.canceled))
            );
            return task;
        }

        private IEnumerator WaitForCoroutine(
            MonoBehaviour master, System.Func<IEnumerator> crtn
        ) {
            yield return master.StartCoroutine(crtn());
            this.Done();
        }

        public static new CancelableTask ByEvent(
            MonoBehaviour master, UnityEvent evt
        ) {
            var task = new CancelableTask();
            UnityAction listener = null;
            listener = () => {
                evt.RemoveListener(listener);
                task.Done();
            };
            task.onCancel.AddListener(() => evt.RemoveListener(listener));
            evt.AddListener(listener);
            return task;
        }
    }
}