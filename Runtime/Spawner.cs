using System.Collections;
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
        [SerializeField] public uint rngState = 0xAABB;

        // [Tooptip("Use this as weights for random generation")]
        // [SerializeField] private float[] weights;
        
        [Header("Automation")]
        [SerializeField] public bool spawnPeriodically = false;
        [SerializeField] public float spawnTimeout = 1f;
        [SerializeField] public bool loop;


        /* message */ IEnumerator Start() {
            if (!this.spawnPeriodically || this.prefabs.Count == 0) yield break;

            do {
                for (int i = 0; i < this.prefabs.Count; ++i) this.Spawn();
                yield return new WaitForSeconds(this.spawnTimeout);
            } while (this.loop);
        }

    // public interface

        private GameObject Spawn(GameObject prefab) {
            var go = GameObject.Instantiate(prefab); 
            if (this.parent != null) go.transform.SetParent(this.parent, false);
            if (this.pivot != null) {
                go.transform.position = this.pivot.position;
                go.transform.rotation = this.pivot.rotation;
                go.transform.localScale = this.pivot.localScale;
            }
            return go;
        } 

        private int nextSpawnIndex = 0;
        public GameObject Spawn() {
            var len = this.prefabs.Count;
            if (len == 0) return null;

            if (!this.spawnRandomly) {
                this.nextSpawnIndex = this.nextSpawnIndex % len; 
                return this.Spawn(this.prefabs[this.nextSpawnIndex]);
            } 

            var index = RNG.RandInt(ref this.rngState, 0, len);
            return this.Spawn(this.prefabs[index]);

        } 

        public T Spawn<T>() where T : Component { 
            var go = this.Spawn(); 
            return go != null ? go.GetComponent<T>() : null;
        } 

    }
}