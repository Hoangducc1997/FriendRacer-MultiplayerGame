using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[AddComponentMenu("EasyVehicleSystem/ESNeutralWheelController")]

public class ESNeutralWheelController : MonoBehaviour
{
    [Header("if this is a truck container pls make sure its child to the main truck head,you can multiple containers as child at same time")]
    public WheelCollider[] wheelcollider = new WheelCollider[2];
    public Transform[] wheelmeshes = new Transform[2];
    [Tooltip("set value above 0 to applydrag")]
    public float MaxAngularDrag = 100f;
    public bool Isgrounded = false;
    [Tooltip("set value above 0 to applydrag")]
    public float MaxDrag = 500f;
    private float ReturnDrag;
    private float RetuenAngulardrag;
    private Rigidbody m_ridigbody;
    public ESVehicleController evs;
    private WheelHit wheelhit;
    private void Start()
    {
        m_ridigbody = GetComponent<Rigidbody>();
        ReturnDrag = m_ridigbody.linearDamping;
        RetuenAngulardrag = m_ridigbody.angularDamping;
    }

    private void FixedUpdate()
    {
        WheelAlignment();
        ApplyDrag();
    }

    private void WheelAlignment()
    {
        // align front wheel meshes
        Vector3 wheelposition;
        Quaternion wheelrotation;

        for (int i = 0; i < wheelcollider.Length; i++)
        {
            if (wheelmeshes[i] == null)
            {
                return;
            }
            wheelcollider[i].GetWorldPose(out wheelposition, out wheelrotation);
            wheelmeshes[i].transform.position = wheelposition;
            wheelmeshes[i].transform.rotation = wheelrotation;
            wheelcollider[i].GetGroundHit(out wheelhit);
        }
        //
        if (evs != null)
        {
            if (evs.CurrentSpeed < 0.25f && evs.Accel == 0 && evs.Shoebrake == 0)
            {
                for (int i = 0; i < wheelcollider.Length; ++i)
                {
                    wheelcollider[i].brakeTorque = float.MaxValue;
                }
                //print("forrze");
            }
            else
            {
                //print("frre");
                for (int i = 0; i < wheelcollider.Length; ++i)
                {
                    wheelcollider[i].brakeTorque = 0.0f;
                }
            }
        }
    }

    private void ApplyDrag()
    {
        //WheelHit hit;

        for (int i = 0; i < wheelcollider.Length; i++)
        {
            Isgrounded = wheelcollider[i].GetGroundHit(out wheelhit);
        }
        if (MaxDrag > 0)
        {
            if (evs.CurrentSpeed < 0.9f)
            {
                if (Input.GetAxis("Vertical") == 0)
                    m_ridigbody.linearDamping = MaxDrag;
                else
                    m_ridigbody.linearDamping = ReturnDrag;
            }
            else
            {
                m_ridigbody.linearDamping = ReturnDrag;
            }
        }
        if (MaxAngularDrag > 0)
        {
            if (evs.CurrentSpeed < 2f)
            {
                if (Input.GetAxis("Vertical") == 0)
                {
                    m_ridigbody.angularDamping = MaxAngularDrag;
                    // m_ridigbody.Sleep();
                    m_ridigbody.solverIterations = 500;   //Physics.defaultSolverIterations;
                }
                else
                {
                    m_ridigbody.solverIterations = Physics.defaultSolverIterations;
                    // m_ridigbody.WakeUp();
                    if (this.GetComponent<HingeJoint>().connectedBody != null)
                    {
                        evs.CarRb.AddForce(evs.transform.forward * 50000f * Input.GetAxis("Vertical"));
                    }
                    //m_ridigbody.angularDrag = RetuenAngulardrag;
                }
            }
            else
            {
                // m_ridigbody.WakeUp();
                m_ridigbody.solverIterations = Physics.defaultSolverIterations;
                m_ridigbody.mass = 500f;
                if (Input.GetAxis("Vertical") > 0)
                {
                    m_ridigbody.angularDamping = RetuenAngulardrag;
                }
            }
        }
    }
}
