using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using VIRTUE;

public class FieldOfView : MonoBehaviour {
    public FieldOfView_SO fovData;

    /*public float viewRadius;

    [Range (0, 360)]
    public float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;*/
    public List<Transform> target = new();

    public bool isFound;
    public float delay;

    void Start () {
        StartCoroutine (FindTargetsWithDelay ());
    }

    IEnumerator FindTargetsWithDelay () {
        while (true) {
            yield return new WaitForSeconds (delay);
            FindVisibleTargets ();
        }
    }

    void FindVisibleTargets () {
        target.Clear ();
        var targetsInViewRadius = Physics.OverlapSphere (transform.position, fovData.viewRadius, fovData.targetMask);
        for (var i = 0; i < targetsInViewRadius.Length; i++) {
            var target = targetsInViewRadius[i].transform;
            var dirToTarget = (target.position - transform.position).normalized;
            var dstToTarget = Vector3.Distance (transform.position, target.position);
            if (!Physics.Raycast (transform.position, dirToTarget, dstToTarget, fovData.obstacleMask)) {
                this.target.Add (target);
                isFound = true;
            } else {
                isFound = false;
            }
        }
        if (targetsInViewRadius.Length == 0) {
            isFound = false;
        }
    }

    public Vector3 DirFromAngle (float angleInDegrees, bool angleIsGlobal) {
        if (!angleIsGlobal) {
            angleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3 (Mathf.Sin (angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos (angleInDegrees * Mathf.Deg2Rad));
    }
}