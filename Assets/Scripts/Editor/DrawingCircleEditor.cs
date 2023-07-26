using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CamCollision))]
public class DrawingCircleEditor : Editor
{

    private void OnSceneGUI()
    {
        CamCollision camCollision = (CamCollision)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(camCollision.transform.position, Vector3.up, Vector3.forward, 360, camCollision.radius);
    }

}
