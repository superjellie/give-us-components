using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GiveUsComponents {
    [AddComponentMenu("GiveUsComponents/Misc/CameraMouseWobble", 100)]
    public class CameraMouseWobble : MonoBehaviour {   
        [SerializeField] private float maxMove = .2f;
        [SerializeField] private float moveSpeed = .2f;
        private Vector3 startPos;
        private Vector3 targetPos;

        void Awake() {
            startPos = this.transform.position;
        }
        
        void Update() {
            Vector3 pos = Input.mousePosition;
            float dx = 2f * pos.x / Screen.width - 1f;
            float dy = 2f * pos.y / Screen.height - 1f;
            targetPos = startPos + maxMove * new Vector3(dx, dy, 0f);
            this.transform.position += moveSpeed * Time.deltaTime * (
                targetPos - this.transform.position
            );
        }
    }
}
