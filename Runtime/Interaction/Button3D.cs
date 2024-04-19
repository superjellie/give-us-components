using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GiveUsComponents {

    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("GiveUsComponents/Interaction/Button3D", 100)]
    public class Button3D : MonoBehaviour {
        
        [SerializeField] public bool useDefaultUniformScale = true;
        [SerializeField] public UnityEvent onClick = new UnityEvent();

        [HideInInspector] public UnityEvent onSelect = new UnityEvent();
        [HideInInspector] public UnityEvent onDeselect = new UnityEvent();
        [HideInInspector] public /* readonly */ bool selected = false;

        private AutoInteruptedTask task = new AutoInteruptedTask();
        /* message */ void OnMouseEnter() {
            if (useDefaultUniformScale)
                task.Play(Tween.ScaleUniform(this, 1.1f));
            this.selected = true;
            this.onSelect.Invoke();
        }

        /* message */ void OnMouseExit() {
            if (useDefaultUniformScale)
                task.Play(Tween.ScaleUniform(this, 1f));
            this.selected = false;
            this.onDeselect.Invoke();
        }

        /* message */ void OnMouseUpAsButton() {
            if (useDefaultUniformScale)
                task.Play(CancelableTask.Chain(this, 
                    () => Tween.ScaleUniform(this, 1.2f, .1f),
                    () => Tween.ScaleUniform(this, 1.1f, .1f)
                ));
            this.onClick.Invoke();
        }
    }
}
