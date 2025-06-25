using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ESAIPath_Single))]
public class ESAIPath_SingleEditor : Editor
{
    public ESAIPath_Single scripts;

    public override void OnInspectorGUI()
    {
        scripts = target as ESAIPath_Single;
        EditorGUILayout.HelpBox("Note 1 : click A key to add node :)", MessageType.Info);
        EditorGUILayout.HelpBox("Note 2 : please ensure to add at least three(3) nodes before using", MessageType.Info);
        GUILayout.Space(10);
        //
        EditorGUI.BeginChangeCheck();
        Color _linecolor = new Color();
        bool _Merg = new bool();
        float raduis = new float();
        bool _DebugMode = new bool();
        float Arrowsize = new float();

        //
        _Merg = EditorGUILayout.Toggle("Merg", scripts.Merg);
        _DebugMode = EditorGUILayout.Toggle("Debug", scripts.DebugMode);
        _linecolor = EditorGUILayout.ColorField("LineColor", scripts.linecolor);
        raduis = EditorGUILayout.FloatField("Raduis", scripts.Raduis);
        Arrowsize = EditorGUILayout.FloatField("ArrowSize", scripts.Arrowsize);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RegisterCompleteObjectUndo(scripts, "fiii");
            scripts.Merg = _Merg;
            scripts.DebugMode = _DebugMode;
            scripts.linecolor = _linecolor;
            scripts.Raduis = raduis;
            scripts.Arrowsize = Arrowsize;
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(scripts);

        }
    }

    public void OnSceneGUI()
    {
        scripts = target as ESAIPath_Single;
        Event e = Event.current;

        if (Event.current.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.A && !scripts.done)
            {
                CallNodes(scripts);
                scripts.done = true;
            }
        }


        if (Event.current.type == EventType.KeyUp)
        {
            scripts.done = false;
        }
        if (GUI.changed)
            EditorUtility.SetDirty(scripts);
    }

    private void CallNodes(ESAIPath_Single es)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            Transform t = Instantiate(es.spawnnode.transform, hit.point, Quaternion.identity);
            t.parent = scripts.transform;
        }
    }
}
