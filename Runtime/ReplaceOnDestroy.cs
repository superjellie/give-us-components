// using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

namespace GiveUsComponents {

    [AddComponentMenu("GiveUsComponents/Misc/ReplaceOnDestroy", 100)]
    public class ReplaceOnDestroy : MonoBehaviour {
        
        [Header("General")]
        [SerializeField] public GameObject replaceWith;


    // collisions
        void OnDestroy() {
            var go = GameObject.Instantiate(this.replaceWith);
            go.transform.SetParent(this.transform.parent);
            go.transform.position = this.transform.position;
            go.transform.rotation = this.transform.rotation;
            go.transform.localScale = this.transform.localScale;
        } 

    }

}