using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESAdvanceVehicleAI : MonoBehaviour
{
    [Header("WheelSettings")]
    public WheelCollider[] frontwheel;
    public WheelCollider[] rearwheel;
    public Transform[] FrontWheelMesh;
    public Transform[] RearWheelMesh;
    [Header("EngineSettings")]
    public float EngineTorque = 1000f;
    public float BrakeTorque = 100000000f;
    public float TopSpeed = 120f;
    public float NitroTopSpeed = 150f;
    [Range(0, 100)] public float MaxNitroValue = 100f;
    [Range(0, 100)] public float NitroExpense = 0.5f;
    [Header("AI Settings")]
    [Range(0, 100)]
    public float RoadWidth = 5f;
    public float OvertakeDistance = 10f;
    public float SmoothTargetSpeed = 75f;
    public bool Navigate = true;
    public float SideSensor = 3;
    public float SteerBalanceFactor = 1;
    public enum UpdateTargetType
    {
        ByDistance,
        ByTriggerEnter
    }
    [Tooltip("ByTrigger Is Not accurrate but lighter ")]
    public UpdateTargetType targetType = UpdateTargetType.ByTriggerEnter;
    public float UpdateTargetDistance = 10;
    public Transform TargetNode;
    public float CurrentSpeed;
    //private 
    private Rigidbody _rigidbody;
    [Tooltip("ReadOnly")]
    [SerializeField] private float random;
    private Transform ObjectInFront;
    private bool LeftSide, RightSide, do_nitro;
    private float DistanceApartTarget, newsteer, nitroval;
    private int DoNitroIndex = 0;
    private float OldRot, bactopspeed;
    [HideInInspector] public Vector3 relativevec, Tar;
    // Start is called before the first frame update
    void Start()
    {
        bactopspeed = TopSpeed;
        _rigidbody = this.GetComponent<Rigidbody>();
        float _random = Random.Range(0, RoadWidth);
        Vector3 right = transform.TransformDirection(Vector3.right);
        Vector3 playerpositon = TargetNode.position - transform.position;
        float sidedot = Vector3.Dot(right, playerpositon);
        //print(sidedot);
        random = -Mathf.Sign(sidedot) * _random;
        InvokeRepeating("NitroProb", 0.1f, 0.5f);
    }
    void NitroProb()
    {
        if (do_nitro == false)
            DoNitroIndex = Random.Range(0, 2);
    }
    // Update is called once per frame
    void Update()
    {
        SteerBalance();
        AiBehavior();
        ApplySteer();
        ArrangeWheelMesh();
        Motor();
        CarSpeed();
        DoNitro();
    }
    //
    private void Motor()
    {
        if (Navigate)
        {
            for (int i = 0; i < frontwheel.Length; i++)
            {
                frontwheel[i].motorTorque = EngineTorque;
            }
            //
            for (int i = 0; i < rearwheel.Length; i++)
            {
                rearwheel[i].motorTorque = EngineTorque;
            }
        }
        else
        {
            for (int i = 0; i < frontwheel.Length; i++)
            {
                frontwheel[i].brakeTorque = BrakeTorque;
            }
            //
            for (int i = 0; i < rearwheel.Length; i++)
            {
                rearwheel[i].brakeTorque = BrakeTorque;
            }
        }
    }
    //
    private void ArrangeWheelMesh()
    {
        Vector3 frontwheelposition;
        Quaternion frontwheelrotation;
        for (int i = 0; i < frontwheel.Length; i++)
        {
            if (FrontWheelMesh[i] == null)
            {
                return;
            }
            frontwheel[i].GetWorldPose(out frontwheelposition, out frontwheelrotation);
            FrontWheelMesh[i].transform.position = frontwheelposition;
            FrontWheelMesh[i].transform.rotation = frontwheelrotation;
        }

        // align rear wheel meshes
        Vector3 rearwheelposition;
        Quaternion rearwheelrotation;
        for (int i = 0; i < rearwheel.Length; i++)
        {
            if (RearWheelMesh[i] == null)
            {
                return;
            }
            rearwheel[i].GetWorldPose(out rearwheelposition, out rearwheelrotation);
            RearWheelMesh[i].transform.position = rearwheelposition;
            RearWheelMesh[i].transform.rotation = rearwheelrotation;
            // Rpm = m_wheelsettings.frontwheels.frontwheelcols[i].rpm;
        }

    }
    //
    private void ApplySteer()
    {
        LerpToSteerAngle();
        Vector3 initailTar = TargetNode.position;
        initailTar += TargetNode.right * random;
        Tar = initailTar;


        //Tar += TargetNode.right * 4f;
        relativevec = transform.InverseTransformPoint(Tar);
        relativevec = relativevec / relativevec.magnitude;
        //newsteer = (relativevec.x / relativevec.magnitude) * 50;

        if (!RightSide && !LeftSide)
        {
            newsteer = (relativevec.x / relativevec.magnitude) * 50;
        }
        else
        {
            Vector3 right = transform.TransformDirection(Vector3.right);
            Vector3 playerpositon = ObjectInFront.transform.position - transform.position;
            float sidedot = Vector3.Dot(right, playerpositon);
            if (sidedot < SideSensor)
            {
                if (RightSide)
                {
                    newsteer = CurrentSpeed < 30f ? -50f : -.25f;
                }
                if (LeftSide)
                {
                    newsteer = CurrentSpeed < 30f ? 50f : .25f;
                }
            }
            else
            {
                newsteer = (relativevec.x / relativevec.magnitude) * 50;
            }
        }

    }
    //
    private void LerpToSteerAngle()
    {
        for (int i = 0; i < frontwheel.Length; i++)
        {
            frontwheel[i].steerAngle = newsteer;
        }
    }
    //
    public void SteerBalance()
    {
        //if (Mathf.Abs(newsteer) < 15) return;
        for (int i = 0; i < frontwheel.Length; i++)
        {
            WheelHit wheelhit;
            frontwheel[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
        }
        for (int i = 0; i < rearwheel.Length; i++)
        {
            WheelHit wheelhit;
            rearwheel[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
        }
        if (Mathf.Abs(OldRot - transform.eulerAngles.y) < 10f)
        {
            var alignturn = (transform.eulerAngles.y - OldRot) * SteerBalanceFactor;
            Quaternion angvelocity = Quaternion.AngleAxis(alignturn, Vector3.up);
            _rigidbody.linearVelocity = angvelocity * _rigidbody.linearVelocity;
        }
        OldRot = transform.eulerAngles.y;
    }
    //

    //
    public void CarSpeed()
    {
        //km/h
        float Pi = Mathf.PI * 1.15f;
        CurrentSpeed = _rigidbody.linearVelocity.magnitude * Pi;


        if (do_nitro == false)
        {
            if (CurrentSpeed > TopSpeed)
                _rigidbody.linearVelocity = (TopSpeed / Pi) * _rigidbody.linearVelocity.normalized;
        }
        else
        {
            if (CurrentSpeed > (NitroTopSpeed))
                _rigidbody.linearVelocity = ((NitroTopSpeed) / Pi) * _rigidbody.linearVelocity.normalized;
        }
    }
    //

    private void AiBehavior()
    {
        if (ObjectInFront != null)
        {
            //
            float overtakedistance = Vector3.Distance(ObjectInFront.position, transform.position);
            if (overtakedistance > OvertakeDistance)
            {
                RightSide = false;
                LeftSide = false;
                ObjectInFront = null;
            }
        }
        else
        {
            RightSide = false;
            LeftSide = false;
        }
        if (TargetNode != null)
        {
            if (TargetNode.GetComponent<ESShowForwardDirection>().NextNode == null)
            {
                Navigate = false;
            }
        }
        //
        if (targetType == UpdateTargetType.ByDistance)
        {
            Vector3 targetdir = TargetNode.position - transform.position;
            float ang = Vector3.Angle(targetdir, transform.forward);
            if (ang > 20)
            {
                SteerBalanceFactor = 1f;
                TopSpeed = SmoothTargetSpeed;
            }
            else
            {
                SteerBalanceFactor = .5f;
                TopSpeed = bactopspeed;
            }
            float dist = Vector3.Distance(this.transform.position, Tar);

            if (dist < UpdateTargetDistance)
            {
                if (TargetNode.GetComponent<ESShowForwardDirection>().NextNode != null)
                    TargetNode = TargetNode.GetComponent<ESShowForwardDirection>().NextNode;
            }
        }
    }
    //
    void DoNitro()
    {
        if (nitroval >= 1)
        {
            if (DoNitroIndex == 1)
            {
                nitroval -= NitroExpense * Time.deltaTime;
            }
        }
        else
        {
            if (do_nitro)
            {
                DoNitroIndex = 0;
                do_nitro = false;
            }
        }
    }
    //
    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody != null)
            ObjectInFront = other.transform;
        //
        if (other.GetComponent<ESNitroSetup>() != null)
        {
            if (nitroval < MaxNitroValue)
            {
                nitroval = other.GetComponent<ESNitroSetup>().NitroValue;
                other.GetComponent<ESNitroSetup>().Reset = true;
                do_nitro = true;
            }
        }
        if (targetType == UpdateTargetType.ByTriggerEnter)
        {
            if (other.CompareTag("Nodes"))
            {
                if (other.GetComponent<ESShowForwardDirection>().NextNode != null)
                    TargetNode = other.GetComponent<ESShowForwardDirection>().NextNode;
            }
        }
        if (ObjectInFront != null)
        {
            Vector3 right = transform.TransformDirection(Vector3.right);
            Vector3 playerpositon = ObjectInFront.transform.position - transform.position;
            float sidedot = Vector3.Dot(right, playerpositon);
            if (sidedot > 0)
            {
                RightSide = true;
                LeftSide = false;
            }
            else
            {
                LeftSide = true;
                RightSide = false;
            }
        }
    }
    //

}
