// using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GiveUsComponents {
    
    [AddComponentMenu("GiveUsComponents/Misc/Spawner", 100)]
    public class Spawner : MonoBehaviour {

        [SerializeField] public List<GameObject> prefabs 
            = new List<GameObject>();

        [Header("Setup")]
        [Tooltip("If provided will be used as pivot for spawning prefabs")]
        [SerializeField] public Transform pivot;

        [Tooltip("If provided will parent all spawned objects to it")]
        [SerializeField] public Transform parent;

        [Header("Random")]
        [SerializeField] public bool spawnRandomly = true;
        // [SerializeField] public GiveUsComponents.RNG.State rngState;

        // [Tooptip("Use this as weights for random generation")]
        // [SerializeField] private float[] weights;
        
        [Header("Automation")]
        [SerializeField] public bool spawnPeriodically = false;
        [SerializeField] public float spawnTimeout = 1f;
        [SerializeField] public bool loop;


        /* message */ void OnValidate() {

        }

    // public interface

        public GameObject SpawnNext() { return null; } 
        public T SpawnNext<T>() where T : Component { return null; } 

    }

}