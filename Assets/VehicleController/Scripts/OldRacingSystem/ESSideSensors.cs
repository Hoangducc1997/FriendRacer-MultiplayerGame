using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESSideSensors : MonoBehaviour
{

    public bool avoid;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Obstacles")
        {
            avoid = true;
        }
    }
    //
    void OnTriggerExit(Collider other)
    {
        avoid = false;
    }
}
