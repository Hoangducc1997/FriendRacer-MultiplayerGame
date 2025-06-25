using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ESNitroPlacer))]
public class ESNitroPlacerEditor : Editor
{
    public ESNitroPlacer scripts;


    public void OnSceneGUI()
    {
        scripts = target as ESNitroPlacer;
        Event e = Event.current;

        if (Event.current.type == EventType.KeyDown)
        {
            if (e.keyCode == KeyCode.A && !scripts.done)
            {
                PlaceNitro(scripts);
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

    public void PlaceNitro(ESNitroPlacer ENP)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray.origin, ray.direction, out hit, Mathf.Infinity))
        {
            GameObject g = Instantiate(ENP.Nitromanager, hit.point + ENP.OffsetVector, Quaternion.identity);
            g.transform.parent = scripts.transform;
        }
    }
}
