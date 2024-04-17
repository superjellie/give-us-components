using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GiveUsComponents;

// [ExecuteInEditMode]
public class PathTest : MonoBehaviour {

    // [SerializeField] int segmentsCount = 3;
    // [SerializeField] List<int> segmentStates = new List<int>();
    // [SerializeField] List<Vector3> keys = new List<Vector3>();

    // void OnDrawGizmos() {
    //     var list = new List<Curve<Vector3>>();
    //     for (int i = 0; i < this.segmentsCount; ++i) 
    //         list.Add(segmentStates[i] == 0 ?
    //             Curves.MakeLine3D(keys[i], keys[i + 1])
    //             : Curves.MakeQuintic(
    //                 keys[i], keys[i + 1], 
    //                 Vector3.up, Vector3.up, 
    //                 Vector3.zero, Vector3.zero
    //             )
    //         );
    //     var test = Curves.MakeSpline(list);

    //     for (float t = 0f; t < 1f - .01f; t += .01f) {
    //         var p0 = this.transform.TransformPoint(test(t));    
    //         var p1 = this.transform.TransformPoint(test(t + .01f));
    //         Gizmos.DrawLine(p0, p1);    
    //     }
    // }

    // void OnValidate() {
    //     if (segmentsCount != segmentStates.Count) {
    //         var list = new List<int>();
    //         for (int i = 0; i < segmentsCount; ++i) list.Add(0);
    //         for (int i = 0; i < segmentsCount && i < segmentStates.Count; ++i)
    //             list[i] = segmentStates[i];
    //         segmentStates = list;
    //     }
    //     if (segmentsCount != keys.Count - 1) {
    //         var list = new List<Vector3>(segmentsCount + 1);
    //         for (int i = 0; i < segmentsCount + 1; ++i) list.Add(Vector3.zero);
    //         for (int i = 0; i < segmentsCount + 1 && i < keys.Count; ++i)
    //             list[i] = keys[i];
    //         keys = list;
    //     }
    // }

}
