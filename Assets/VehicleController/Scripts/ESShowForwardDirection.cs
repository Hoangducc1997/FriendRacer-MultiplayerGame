using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESShowForwardDirection : MonoBehaviour
{
    [SerializeField]
    private float raylength = 6;
    [SerializeField]
    private Color LineColor = new Vector4(1, 1, 1, 1);
    public bool ResizeCollider;
    public GameObject mesh;
    public Transform NextNode;
    public float Arrowsize = 1;
    public float PointValue = 100f;
    public bool LapCounter = false;
    public int IDNumber = 0;
    private void OnDrawGizmos()
    {
        Gizmos.color = LineColor;
        Vector3 dir = transform.forward * raylength;
        Gizmos.DrawRay(transform.position, dir);

        if (mesh != null && NextNode != null)
        {
            transform.LookAt(NextNode);
            Gizmos.color = Color.red;
            Gizmos.DrawMesh(mesh.GetComponent<MeshFilter>().sharedMesh, -1, (this.transform.position + NextNode.position) * 0.5f, this.transform.rotation, new Vector3(1, 1, 1) * Arrowsize);
        }
    }
}