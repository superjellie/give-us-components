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
            if (this != null) this.Done();
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

        public static Task Together(MonoBehaviour master, params Task[] tasks) 
        => ByCoroutine(master, () => TogetherCoroutine(tasks));

        public static Task Chain(
            MonoBehaviour master, params System.Func<Task>[] tasks
        ) => ByCoroutine(master, () => ChainCoroutine(tasks));

        public static IEnumerator TogetherCoroutine(Task[] tasks) {
            foreach (var task in tasks)
                yield return task;
        }
        public static IEnumerator ChainCoroutine(
            System.Func<Task>[] tasks
        ) {
            foreach (var task in tasks) {
                var startedTask = task();
                yield return startedTask;
            }
        }
    }

    public delegate bool CancelationToken();
    public class CancelableTask : CustomYieldInstruction {
        
        public /* readonly */ bool canceled = false; 
        public /* readonly */ UnityEvent onCancel = new UnityEvent(); 

        public bool done { get => this.task.done; } 
        public UnityEvent onDone { get => this.task.onDone; } 
        private Task task = new Task();

        public void Cancel() {
            this.canceled = true;
            this.onCancel.Invoke();
            this.Done();
        }

        public override bool keepWaiting => !this.done;
        public void Done() => this.task.Done();
        
        public static implicit operator Task(CancelableTask ct) => ct.task;
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
            if (!this.done) this.Done();
        }

        public static CancelableTask ByEvent(
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

        public static CancelableTask Together(
            MonoBehaviour master, params CancelableTask[] tasks
        ) => ByCoroutine(master, 
            (isCanceled) => CancelableTask.TogetherCoroutine(tasks, isCanceled)
        );

        public static CancelableTask Chain(
            MonoBehaviour master, params System.Func<CancelableTask>[] tasks
        ) => ByCoroutine(master, 
            (isCanceled) => CancelableTask.ChainCoroutine(tasks, isCanceled)
        );

        public static IEnumerator TogetherCoroutine(
            CancelableTask[] tasks, CancelationToken isCanceled
        ) {
            while (!isCanceled()) {
                var done = true;
                foreach (var task in tasks) done &= task.done; 
                if (done) break;
                yield return new WaitForSeconds(.01f);
            }
            if (isCanceled())
                foreach (var task in tasks)
                    if (!task.done) task.Cancel();
        }
        public static IEnumerator ChainCoroutine(
            System.Func<CancelableTask>[] tasks, CancelationToken isCanceled
        ) {
            foreach (var task in tasks) {
                var startedTask = task();

                while (!startedTask.done) {
                    if (isCanceled()) {
                        startedTask.Cancel();
                        yield break;
                    }
                    yield return new WaitForSeconds(.01f);
                }
            }
        }
    }

    public class AutoInteruptedTask {
        private CancelableTask task = null;

        public void Play(CancelableTask newTask) {
            if (this.task != null && !this.task.done) this.task.Cancel();
            this.task = newTask;
        }
    }
}