using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;

public class ESVehicleMenuItem : MonoBehaviour
{
    [MenuItem("Easy Vehicle System/AI/NewPath(Advance)", false, 10)]
    static void CreateAiPathGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("AI Path(Advance)");
        go.AddComponent<ESAIPath_Single>();

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

    [MenuItem("Easy Vehicle System/NitroPlacer", false, 10)]
    static void CreateNitroPlacerGameObject(MenuCommand menuCommand)
    {
        // Create a custom game object
        GameObject go = new GameObject("NitroPlacer");
        go.AddComponent<ESNitroPlacer>();

        // Ensure it gets reparented if this was a context click (otherwise does nothing)
        GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
        // Register the creation in the undo system
        Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
        Selection.activeObject = go;
    }

}
#endif
