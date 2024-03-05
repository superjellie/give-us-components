// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace GiveUsComponents {

    [AddComponentMenu("GiveUsComponents/Misc/ReplaceOnDestroy", 100)]
    public class ReplaceOnDestroy : MonoBehaviour {
        
        [Header("General")]
        [SerializeField] public GameObject replaceWith;


    // collisions
        private bool quitting = false; 
        void OnApplicationQuit() => this.quitting = true; 
        void OnDestroy() {
            if (this.quitting) return;
            var go = GameObject.Instantiate(this.replaceWith);
            go.transform.SetParent(this.transform.parent);
            go.transform.position = this.transform.position;
            go.transform.rotation = this.transform.rotation;
            go.transform.localScale = this.transform.localScale;
        } 

    }

}