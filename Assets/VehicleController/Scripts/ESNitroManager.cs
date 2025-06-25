using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESNitroManager : MonoBehaviour
{

    [Tooltip("must be a prefab")]
    public Transform nitroprefab;
    public float Starttime, DelayTime;
    public bool ShowGizmos = true;
    public Color _GizmosColor = new Vector4(1, 1, 1, 1);
    [HideInInspector]
    public Transform nitorobj;

    // Use this for initialization
    void Start()
    {
        InvokeRepeating("spawnme", Starttime, DelayTime);
    }



    void spawnme()
    {
        if (nitorobj == null)
        {
            nitorobj = Instantiate(nitroprefab, transform.position, Quaternion.identity, this.transform);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = _GizmosColor;
        if (ShowGizmos)
            Gizmos.DrawWireSphere(transform.position, 0.8f);
    }

}
