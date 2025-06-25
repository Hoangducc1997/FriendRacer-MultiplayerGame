using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESNitroSetup : MonoBehaviour
{
    public float NitroValue;
    public float RotationSpeed;
    public AudioClip PickUpSound;
    public bool Reset;
    public float ResetTime = 1.5f;

    public void FixedUpdate()
    {
        Vector3 RotVec = new Vector3(1, 1, 1) * RotationSpeed;
        transform.Rotate(RotVec);
        if (Reset)
        {
            StartCoroutine(ResetNitro());
        }
    }
    IEnumerator ResetNitro()
    {
        this.GetComponent<Collider>().enabled = false;
        this.GetComponent<MeshRenderer>().enabled = false;
        Reset = false;
        yield return new WaitForSeconds(ResetTime);
        this.GetComponent<Collider>().enabled = true;
        this.GetComponent<MeshRenderer>().enabled = true;
    }
}
