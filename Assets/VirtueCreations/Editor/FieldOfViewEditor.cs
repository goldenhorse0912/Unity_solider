using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView) target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.fovData.viewRadius);
        Vector3 viewAngleA = fow.DirFromAngle(-fow.fovData.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.fovData.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.fovData.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.fovData.viewRadius);

        Handles.color = Color.red;
        foreach (Transform visibleTarget in fow.target)
        {
            Handles.DrawLine(fow.transform.position, visibleTarget.position);
        }
    }
}