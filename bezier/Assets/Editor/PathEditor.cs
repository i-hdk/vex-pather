using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PathCreator))]
public class PathEditor : Editor
{
    PathCreator creator;
    Path path;
    void Draw()
    {
        Handles.color = Color.red;
        for(int i = 0; i < path.NumPoints; i++)
        {
            var fmh_16_62_638545072014472439 = Quaternion.identity; Vector2 newPos = Handles.FreeMoveHandle(path[i], 100, Vector2.zero, Handles.CylinderHandleCap);
            if (path[i] != newPos)
            {
                Undo.RecordObject(creator, "Move Point");
                path.MovePoint(i, newPos);
            }
        }
    }
}
