using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GiveUsComponents;

public class CustomTweenTest : MonoBehaviour {
    
    [SerializeField] Tween.Custom tweener = new Tween.Custom();
    [SerializeField] float time = 1f;


    /* message */ void Update() {
        tweener.Evaluate(Mathf.PingPong(Time.time, time));
    }

}
