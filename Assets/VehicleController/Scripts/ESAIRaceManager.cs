using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(SphereCollider))]
public class ESAIRaceManager : MonoBehaviour
{
    [HideInInspector]
    public bool Reset = false;
    [HideInInspector]
    public bool triggered;
    [HideInInspector]
    public float mytime = 0;
    public int currentlap = 1;
    public string RaceControllerTagName = "racecontroller";
    //public float _DestroyTime;
    public Transform LastCheckpiont;
    [HideInInspector]
    public Vector3 pos;
    [HideInInspector]
    public Quaternion rot;
    [HideInInspector]
    public float ang;
    [HideInInspector]
    public bool StartCount = false;
    [HideInInspector]
    public bool add_current_lap;
    [HideInInspector]
    public bool passed = false;
    //
    [Header("Manually fill the player current before play")]
    public int currentpos = 1;
    public bool isplayer =  false;
   
    void Update()
    {
        
        CheckForFlipState();
        if(LastCheckpiont != null)
        ang = Vector3.Dot(transform.forward,LastCheckpiont.forward);

        //
        if (add_current_lap)
        {
            currentlap++;
            add_current_lap = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "DeathZone")
        {
            triggered = true;
        }
        if (other.gameObject.tag == "DeathImmidiate")
        {
            Reset = true;
        }
        if (other.gameObject.tag == "Nodes")
        {
            pos = other.gameObject.transform.position;
            rot = other.gameObject.transform.rotation;
            LastCheckpiont = other.transform;
        }
     
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "DeathZone")
        {
            triggered = false;
        }
        //
        if (other.gameObject.CompareTag(RaceControllerTagName))
        {
            StartCount = true;
        }
    }

    void CheckForFlipState()
    {
        //check for upsidedown

        if (Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            mytime += Time.deltaTime;
            if (mytime > 5)
            {
                Reset = true;
            }
        }
        //check for side ways
        else if (Mathf.Abs(Vector3.Dot(transform.up, Vector3.down)) < 0.125f)
        {
            mytime += Time.deltaTime;
            if (mytime > 5)
            {
                Reset = true;
            }

        }
        else if (Mathf.Abs(Vector3.Dot(transform.right, Vector3.down)) > 0.825f)
        {
            mytime += Time.deltaTime;
            if (mytime > 5)
            {
                Reset = true;
            }
        }
        //
        else if (ang < 0)
        {
            mytime += Time.deltaTime;
            if (mytime > 5)
            {
                Reset = true;
            }
        }
        //when it collides with a wall nd gets stuck 
        else if (triggered)
        {
            mytime += Time.deltaTime;
            if (mytime > 5)
            {
                Reset = true;
            }
        }
        else
        {
            mytime = 0;
        }

    }
}
