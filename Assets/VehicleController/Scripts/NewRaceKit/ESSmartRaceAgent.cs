using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ESSmartRaceAgent : MonoBehaviour
{
    public float ResetTime = 5;
    public float ReSpawnYoffset = 0.5f;
    public float RequiredIndexAmount = 10f;
    [Tooltip("Increase if your road has sharp curves")]
    [Range(0, 180)] public float ResetAngle = 90f;
    [HideInInspector]
    public Vector3 LastCheckPointPosition, LastCheckPointFwd;
    [HideInInspector] public Transform CheckPointTransform;
    [HideInInspector] public Quaternion LastCheckPointRotation;
    [HideInInspector] public float p;
    [HideInInspector] public float DistanceFromLastWayPoint;
    [HideInInspector] public float LapModifier;
    [HideInInspector] public int CurrentLap = 1;
    [HideInInspector] public int numberofwaypointsattained;
    private bool countedlap = true;
    private bool Reset, triggered, ReadAng;
    [HideInInspector]
    public float ang, counter, Rang;
    private int _requiredIndexAmount;

    // Update is called once per frame
    void Update()
    {
        CallReset();
        if (Reset)
        {
            if (counter < ResetTime)
            {
                counter += Time.deltaTime;
            }
            else
            {
                ResetVehiclePos_Rot();
                Reset = false;
            }
        }
        else
        {
            counter = 0;
        }
        //
        DistanceFromLastWayPoint = GetDistanceFromLastWayPoint();
        p = GetWayPointIndentity();
        if (this.GetComponent<ESVehicleController>() != null)
        {
            Vector3 targetdir = CheckPointTransform.position - transform.position;
            Rang = Vector3.Angle(targetdir, transform.forward);
        }
    }
    private void OnCollisionStay(Collision other)
    {
        if (other.contacts.Length == 0) return;
        if (this.GetComponent<Rigidbody>().linearVelocity.magnitude < 0.15f)
        {
            Reset = true;
        }
    }
    //
    private void OnTriggerExit(Collider other)
    {
        Reset = false;
    }
    //
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Nodes"))
        {
            _requiredIndexAmount++;
            CheckPointTransform = other.GetComponent<ESShowForwardDirection>().NextNode;
            if (this.GetComponent<ESAdvanceVehicleAI>() != null)
            {
                LastCheckPointPosition = this.GetComponent<ESAdvanceVehicleAI>().TargetNode.position;
                LastCheckPointRotation = this.GetComponent<ESAdvanceVehicleAI>().TargetNode.rotation;
                LastCheckPointFwd = this.GetComponent<ESAdvanceVehicleAI>().TargetNode.forward;
            }
            else
            {
                LastCheckPointPosition = other.transform.position;
                LastCheckPointRotation = other.transform.rotation;
                LastCheckPointFwd = other.transform.forward;
            }
            if (_requiredIndexAmount > RequiredIndexAmount)
            {
                if (other.GetComponent<ESShowForwardDirection>().LapCounter)
                {
                    if (numberofwaypointsattained == other.GetComponent<ESShowForwardDirection>().IDNumber)
                    {
                        countedlap = false;
                    }
                    if (!countedlap)
                    {
                        CurrentLap++;
                        LapModifier = LapModifier * CurrentLap;
                        countedlap = true;
                    }
                }
            }
            if (other.GetComponent<ESShowForwardDirection>().LapCounter)
            {
                _requiredIndexAmount = 0;
            }
            numberofwaypointsattained = other.GetComponent<ESShowForwardDirection>()
            .NextNode.GetComponent<ESShowForwardDirection>().IDNumber;
        }
    }
    //
    void ResetVehiclePos_Rot()
    {
        this.GetComponent<Rigidbody>().isKinematic = true;
        this.transform.position = LastCheckPointPosition + new Vector3(0, ReSpawnYoffset, 0);
        this.transform.rotation = LastCheckPointRotation;
        this.GetComponent<Rigidbody>().isKinematic = false;
    }
    //
    private void CallReset()
    {
        CheckForFlip();
    }
    //
    public float GetWayPointIndentity()
    {
        float pointvalue = new float();
        return pointvalue = CurrentLap > 0 ? (CheckPointTransform.GetComponent<ESShowForwardDirection>().PointValue + LapModifier) * CurrentLap :
      (CheckPointTransform.GetComponent<ESShowForwardDirection>().PointValue + LapModifier) * 1;
    }
    //
    private float GetDistanceFromLastWayPoint()
    {
        Vector3 fwd = transform.TransformDirection(Vector3.forward);
        Vector3 Carpositon = CheckPointTransform.position - transform.position;
        float Fwddot = Vector3.Dot(fwd, Carpositon);
        return Mathf.Abs(Fwddot);
    }
    //

    void CheckForFlip()
    {
        Vector3 fwd = LastCheckPointFwd;
        ang = Vector3.Dot(transform.forward, fwd);
        if (Vector3.Dot(transform.up, Vector3.down) > 0)
        {
            Reset = true;
        }
        //check for side ways
        else if (Mathf.Abs(Vector3.Dot(transform.up, Vector3.down)) < 0.125f)
        {
            Reset = true;

        }
        else if (Mathf.Abs(Vector3.Dot(transform.right, Vector3.down)) > 0.825f)
        {
            Reset = true;
        }
        //
        else if (ang < 0 && Rang > ResetAngle)
        {
            Reset = true;
        }
        //when it collides with a wall nd gets stuck 
        else if (triggered)
        {
            Reset = true;
        }
    }
}
