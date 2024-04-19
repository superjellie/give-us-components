using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GiveUsComponents {

    [AddComponentMenu("GiveUsComponents/Interaction/Draggable3D", 110)]
    public class Draggable3D : MonoBehaviour {

        public enum State { Idle, Selected, Dragged, DropPreview };

        [SerializeField] public bool useDefaultDrag = true;
        [SerializeField, ShowWhen("useDefaultDrag", true)] 
        public LayerMask defaultDragPlaneMask;
        [SerializeField, ShowWhen("useDefaultDrag", true)] 
        public float defaultDragHeight = 1f;
        [SerializeField, ShowWhen("useDefaultDrag", true)] 
        public float defaultDragNoPlaneDistance = 4f;
        [SerializeField] public LayerMask dropTargetMask;

        [HideInInspector] public UnityEvent onSelect = new UnityEvent();
        [HideInInspector] public UnityEvent onDeselect = new UnityEvent();
        [HideInInspector] public UnityEvent onDragStart = new UnityEvent();
        [HideInInspector] public UnityEvent onDropPreviewStart 
            = new UnityEvent();
        [HideInInspector] public UnityEvent onDropPreviewEnd = new UnityEvent();
        [HideInInspector] public UnityEvent onDrop = new UnityEvent();
        [HideInInspector] public /* readonly */ State state = State.Idle;
        [HideInInspector] public /* readonly */ bool selected = false;
        [HideInInspector] public /* readonly */ bool dragged = false;
        [HideInInspector] public /* readonly */ Ray pointerRay;
        [HideInInspector] public /* readonly */ Transform currentDropTarget;


        private AutoInteruptedTask task = new AutoInteruptedTask();

        /* message */ void OnMouseEnter() => this.selected = true;
        /* message */ void OnMouseExit() => this.selected = false;
        /* message */ void OnMouseDown() => this.dragged = true;
        /* message */ void OnMouseUp() => this.dragged = false;

        
        private Vector3 moveTarget;
        private Vector3 oldPosition;
        private bool shouldMoveToTarget = false;
        /* message */ void Update() {
            if (this.useDefaultDrag && (
                    this.state == State.DropPreview
                    || this.state == State.Dragged || this.shouldMoveToTarget
                )
            ) {
                var offset = this.moveTarget - this.transform.position;
                this.transform.position 
                    += offset * Time.deltaTime * 10f;
                if (offset.magnitude < .01f) shouldMoveToTarget = false;
            } 

            if (Camera.main == null) return;
            this.pointerRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        /* message */ void FixedUpdate() {
            var overDropTarget = Physics.Raycast(
                this.pointerRay, out var hit, maxDistance: Mathf.Infinity, 
                layerMask: this.dropTargetMask, 
                queryTriggerInteraction: QueryTriggerInteraction.Collide
            );
            this.currentDropTarget = hit.transform;

            if (this.state == State.Idle && this.selected) {
                this.state = State.Selected; this.onSelect.Invoke(); 
                if (this.useDefaultDrag)
                    this.task.Play(Tween.ScaleUniform(this, 1.1f));
            } else if (this.state == State.Selected && !this.selected) {
                this.state = State.Idle; this.onDeselect.Invoke();
                if (this.useDefaultDrag)
                    this.task.Play(Tween.ScaleUniform(this, 1f));
            } else if (this.state == State.Idle && this.dragged
                || this.state == State.Selected && this.dragged) {
                this.state = State.Dragged; this.onDragStart.Invoke();
                this.oldPosition = this.transform.position;
                if (this.useDefaultDrag)
                    this.task.Play(Tween.ScaleUniform(this, 1f));
            } else if (this.state == State.Dragged && !this.dragged
                || this.state == State.DropPreview && !this.dragged) {
                this.state = State.Idle; this.onDrop.Invoke();
                this.moveTarget = overDropTarget ? hit.transform.position
                    : this.oldPosition;
                this.shouldMoveToTarget = true;
            } else if (this.state == State.Dragged && overDropTarget) {
                this.state = State.DropPreview; 
                this.onDropPreviewStart.Invoke();
            } else if (this.state == State.DropPreview && !overDropTarget) {
                this.state = State.Dragged; 
                this.onDropPreviewEnd.Invoke();
            }

            if (this.useDefaultDrag && this.state == State.Dragged) {
                var overPlane = Physics.Raycast(
                    this.pointerRay, out var plane, maxDistance: Mathf.Infinity, 
                    layerMask: this.defaultDragPlaneMask, 
                    queryTriggerInteraction: QueryTriggerInteraction.Collide
                );

                this.moveTarget = overPlane ? plane.point 
                        + plane.normal * this.defaultDragHeight
                    : this.pointerRay.origin + this.pointerRay.direction 
                        * this.defaultDragNoPlaneDistance;
            } 

            if (this.useDefaultDrag && this.state == State.DropPreview) {
                this.moveTarget 
                    = hit.transform.position 
                        + Vector3.up * this.defaultDragHeight;
            }
        }
    }
}
