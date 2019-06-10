using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(MGFPolygon))]
public class Collider : Editor
{
    private MGFPolygon _target;
    private bool _editMode = false;


    private void OnEnable()
    {
        if (!_target) _target = target as MGFPolygon;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var vertex = _target.vertex;
        if (vertex != null)
        {
            for(int i=0;i<vertex.Length;i++)
            {
                EditorGUILayout.TextField("顶点" + i, vertex[i].ToString());
            }
        }
        EditorGUILayout.BeginHorizontal();
        if(GUILayout.Button("开启编辑"))
        {
            _editMode = true;
        }
        if (GUILayout.Button("关闭编辑"))
        {
            _editMode = false;
        }
        EditorGUILayout.EndHorizontal();

    }

    private void OnSceneGUI()
    {
        Handles.DrawCube(1, new Vector3(0, 0, 0), Quaternion.identity, 3);
    }

}
