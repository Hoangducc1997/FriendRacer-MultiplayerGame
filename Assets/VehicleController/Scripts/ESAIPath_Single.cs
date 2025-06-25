using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class ESAIPath_Single : MonoBehaviour
{
    [HideInInspector]
    public GameObject spawnnode;
    [HideInInspector]
    public bool done;
    public Color linecolor;
    public bool Merg;
    public float Raduis = 30f;
    public float Arrowsize = 1;
    public bool DebugMode;
    public const float SpawnPointIdentity = 100;
    private List<Transform> nodes = new List<Transform>();

    private void Update()
    {
        spawnnode = Resources.Load("Path/node") as GameObject;
        //
        MeshRenderer[] pathmeshrenderer = GetComponentsInChildren<MeshRenderer>();
        if (DebugMode == false)
        {
            for (int i = 0; i < pathmeshrenderer.Length; i++)
            {
                pathmeshrenderer[i].enabled = false;
            }
        }
        else
        {
            for (int i = 0; i < pathmeshrenderer.Length; i++)
            {
                pathmeshrenderer[i].enabled = true;
            }
        }
        for (int i = 0; i < pathmeshrenderer.Length; i++)
        {
            if (!pathmeshrenderer[i].GetComponent<ESShowForwardDirection>().ResizeCollider)
                pathmeshrenderer[i].GetComponent<SphereCollider>().radius = Raduis;
        }
        if (nodes.Count > 0)
        {
            for (int i = 0; i < nodes.Count; ++i)
            {
                if (nodes[i] != null)
                {
                    nodes[i].GetComponent<ESShowForwardDirection>().PointValue = 0.0f;
                    nodes[i].GetComponent<ESShowForwardDirection>().IDNumber = 0;
                    nodes[i].GetComponent<ESShowForwardDirection>().IDNumber = i + 1;
                    nodes[i].GetComponent<ESShowForwardDirection>().Arrowsize = Arrowsize;
                    if (i == 0)
                    {
                        nodes[i].GetComponent<ESShowForwardDirection>().PointValue = SpawnPointIdentity * 0.5f;
                    }
                    else
                    {
                        nodes[i].GetComponent<ESShowForwardDirection>().PointValue = SpawnPointIdentity * i;
                    }
                }
            }
        }
    }
    private void OnDrawGizmosSelected()
    {

        Gizmos.color = linecolor;
        Transform[] Tchild = GetComponentsInChildren<Transform>();
        if (Tchild.Length > 1)
        {
            for (int i = 0; i < Tchild.Length; i++)
            {
                if (i < Tchild.Length - 1 && Tchild[i] != transform)
                    Tchild[i].GetComponent<ESShowForwardDirection>().NextNode = Tchild[i + 1].transform;
            }
            if (Merg)
            {
                Tchild[Tchild.Length - 1].GetComponent<ESShowForwardDirection>().NextNode = Tchild[1].transform;
            }
            else
            {
                Tchild[Tchild.Length - 1].GetComponent<ESShowForwardDirection>().NextNode = null;
            }
        }
        Transform[] pathtrans = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < pathtrans.Length; i++)
        {
            if (pathtrans[i] != transform)
            {
                nodes.Add(pathtrans[i]);
            }
        }
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 curnode = nodes[i].position;
            Vector3 prevnode = Vector3.zero;
            if (i > 0)
            {
                prevnode = nodes[i - 1].position;
            }
            else if (i == 0 && nodes.Count > 1 && Merg)
            {
                prevnode = nodes[nodes.Count - 1].position;
            }
            else if (i == 0)
            {
                prevnode = nodes[0].position;
            }
            Gizmos.DrawLine(prevnode, curnode);
            Gizmos.DrawWireSphere(curnode, 0.5f);


        }
    }
}
