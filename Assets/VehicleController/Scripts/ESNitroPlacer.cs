using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESNitroPlacer : MonoBehaviour
{
    [Header("Use A key to add nitro  spawn point")]
    [Tooltip("Increase Y value to place higher above the ground")]
    public Vector3 OffsetVector;
    [Tooltip("Must Be a Prefab")]
    public GameObject Nitromanager;
    [HideInInspector]
    public bool done;

}
