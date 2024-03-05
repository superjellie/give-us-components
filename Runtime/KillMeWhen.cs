using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace GiveUsComponents {

    [AddComponentMenu("GiveUsComponents/Misc/KillMeWhen", 100)]
    public class KillMeWhen : MonoBehaviour {
        
        [Header("General")]
        [SerializeField] public float destroyTimeout = 0f;

        [Header("Time")]
        [SerializeField] public bool whenTimeIsOut = false;
        [SerializeField] public float timeout = 1f;

        [Header("Collision")]
        [SerializeField] public bool whenEnterCollision = false;
        [SerializeField] public bool whenEnterTrigger = false;
        [SerializeField] public LayerMask mask;

        [Header("Gameplay")]
        [SerializeField] public bool whenFallenOutOfWorld = false;
        [SerializeField] public float minY = -200f;

        IEnumerator Start() {
            var time = Time.time;
            yield return new WaitUntil(() => 
                this.whenTimeIsOut && Time.time - time > this.timeout
            ||  this.whenFallenOutOfWorld 
                && this.transform.position.y < this.minY
            );
            GameObject.Destroy(this.gameObject, this.destroyTimeout);
        }


    // collisions
        void OnTriggerEnter(Collider other) {
            var otherMask = 1 << other.gameObject.layer;
            if (this.whenEnterTrigger && (otherMask & this.mask.value) > 0)
                GameObject.Destroy(this.gameObject, this.destroyTimeout);
        }

        void OnTriggerEnter2D(Collider2D other) {
            var otherMask = 1 << other.gameObject.layer;
            if (this.whenEnterTrigger && (otherMask & this.mask.value) > 0)
                GameObject.Destroy(this.gameObject, this.destroyTimeout);
        }

        void OnCollisionEnter(Collision col) {
            var otherMask = 1 << col.gameObject.layer;
            if (this.whenEnterCollision && (otherMask & this.mask.value) > 0)
                GameObject.Destroy(this.gameObject, this.destroyTimeout);
        }

        void OnCollisionEnter2D(Collision2D col) {
            var otherMask = 1 << col.gameObject.layer;
            if (this.whenEnterCollision && (otherMask & this.mask.value) > 0)
                GameObject.Destroy(this.gameObject, this.destroyTimeout);
        }

    }

}