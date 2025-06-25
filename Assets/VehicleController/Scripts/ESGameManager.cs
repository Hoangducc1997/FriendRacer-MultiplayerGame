using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESGameManager : MonoBehaviour
{
    public int targetframerate = 30;
    private int i;
    private float deltatime = 0.0f;

    private void Awake()
    {
#if  UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        i = Application.targetFrameRate = targetframerate;

#endif
    }
    //
    private void Update()
    {
        //deltatime += (Time.unscaledDeltaTime - deltatime) * 0.1f;
    }
    //
    private void OnGUI()
    {
        //float msec = deltatime * 1000.0f;
        //float fps = 1.0f / deltatime;
        // string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
        //GUI.Box(new Rect(150, 0, 200, 32), text);
        //
        string[] names = QualitySettings.names;
        GUILayout.BeginVertical();

        for (int i = 0; i < names.Length; i++)
        {
            if (GUILayout.Button(names[i]))
            {
                QualitySettings.SetQualityLevel(i, true);
            }
        }
        GUILayout.EndVertical();
    }
}
