using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
[AddComponentMenu("EasyVehicleSystem/ESVehicleAI")]
//[RequireComponent(typeof(AudioSource))]
public class ESVehicleAI : MonoBehaviour
{
    public enum DriveType
    {
        FourWheel,
        RearWheel,
        FrontWheel
    }
    [Serializable]
    public class WheelSettings
    {
        [Serializable]
        public class FrontWheel
        {
            public WheelCollider[] frontwheelcol;
            public Transform[] frontwheelmesh;
        }
        [Serializable]
        public class RearWheel
        {
            public WheelCollider[] rearwheelcol;
            public Transform[] rearwheelmesh;
        }
        public FrontWheel frontwheel;
        public RearWheel rearwheel;
    }
    [Serializable]
    public class Sensor
    {
        [Serializable]
        public class FrontSensor
        {
            public float raylength = 20f;
            public Vector3 frontsensorpos = new Vector3(0f, 0f, 0.5f);
            public float angle = 30f;
            public float skinwidth = 0.2f;
        }
        [Serializable]
        public class SideSensor
        {
            public ESSideSensors _Leftsidesensors;
            public ESSideSensors _Rightsidesensors;
            [Range(0f, 1f)]
            public float steersensitivity = 0.58f;
        }
        public FrontSensor frontsensor;
        public SideSensor sidesensor;
    }
    [Serializable]
    public class GearShift
    {
        public float enginerpm;
        [Range(0, 10)]
        public int maxgear;
        public bool geartransmissioneffect;
        public float effecttime;
        public int currentgear;
        public GameObject[] exhaustflame;
        // hideinspector
        [HideInInspector]
        public bool neutral;
        [HideInInspector]
        public int oldgear;
        [HideInInspector]
        public float inversegearratio;
        [HideInInspector]
        public float oldTopSpeed;
        [HideInInspector]
        public float oldgearratio;
        [HideInInspector]
        public float gearratio;
        [HideInInspector]
        public float tempforce;
        [HideInInspector]
        public float currentengineforce;
        [HideInInspector]
        public float inversecurrentgear;

    }
    [Serializable]
    public class Drift
    {
        public ESDrift[] esdriftAI;
        public float killbrakespeed = 100;
        public float driftangle;
        [HideInInspector]
        public bool drifting;
        public float killriftspeed;
    }
    [Serializable]
    public class AudioSettings
    {
        public AudioSource audiosource;
        public float pitchmultiplier;
        public float pitchmodifier;

    }
    public enum AIBehavoiur
    {
        Aggressive,
        Gentle
    }
    public enum AiWrapMode
    {
        Once,
        Continous
    }
    public enum EngineMode
    {
        Nitro,
        Normal
    }
    public enum NitroUsage
    {
        Decisive,
        Immediate
    }
    #region public
    public Sensor sensor;
    public GearShift gearshift;
    public WheelSettings wheelsettings;
    public Drift drift;
    public AudioSettings audiosettings;
    public Vector3 carridigbodycenterofmass = new Vector3(0f, -0.5f, 0f);
    //
    [Header("PathSettings")]
    [Tooltip("Enable if node is not static")]
    public bool IsMovingTarget;
    [HideInInspector]
    public bool StopTracking;
    [Tooltip("if 'IsMovingTarget' is true drag drop moving node here. note that node most be a child gameobject:)")]
    public GameObject path;
    [Tooltip("Distance away from the current node and Ai")]
    public float distanceapart;
    public AiWrapMode _wrapmode = AiWrapMode.Continous;
    [Tooltip("Obstacle Must have a rigibody attached to it")]
    public GameObject[] Obstacles;
    public string ObstacleTagName;
    public string PathName = "";
    [Header("End")]
    //
    [Header("EngineSettings")]
    public EngineMode enginemode = EngineMode.Normal;
    public DriveType drivetype = DriveType.FourWheel;
    public float maxsteerangle = 20f;
    public float SteerBalanceFactor = 0.5f;
    public float reversetime = 4f;
    public float turnspeed = 5f;
    public float maxtorque;
    public float maxbraketorque;
    public float currentspeed;
    public bool isbraking;
    public float maxspeed = 100f;
    public float topspeed;
    public float sliplimit = 0.9f;
    public float forceAppliedToWheels;
    [Range(0f, 1f)]
    public float tractionfactor = 1f;
    [Header("End")]
    [Header("AIBehaviour")]
    public AIBehavoiur aibehaviour = AIBehavoiur.Aggressive;
    [Tooltip("distance ai detects obstacles")]
    public float detectiondistance = 100f;
    public float smoothtargetangle = 10f;
    public float smoothtargetspeed = 25f;
    public float smoothTargetDistance = 100f;
    [HideInInspector]
    public bool juststartednitro;
    [Header("End")]
    [Header("Light System")]
    public bool UseHeadLamp;
    public bool High;
    [Header("End")]
    [Header("Nitro Settings")]
    [Header("to use this change engine mode to nitro")]
    public NitroUsage nitro_usage = NitroUsage.Immediate;
    [HideInInspector]
    public bool do_nitro;
    [HideInInspector]
    public float NitroValue;
    public float NitroTorque;
    [Tooltip("change nitro usage to decicisive inorder to use this")]
    public float DecisionDelay;
    [HideInInspector]
    public int decisionindex;
    [HideInInspector]
    public float ReturnTorque;
    public AudioSource PickUpSource;
    [Range(0, 100)]
    public float MaxNitroValue;
    [Range(0, 10)]
    public float NitroExpense;
    [HideInInspector]
    public bool once;
    public List<Transform> nodes;
    public int currentnode = 0;
    [Header("miscellaneous")]
    public bool showdebuglog = true;
    public float mul = 1;
    [Header("Balance")]
    public float drag = 0.5f;
    public float AirDrag = 5f;
    #endregion
    //
    #region private
    private bool isavoiding;
    private bool isreverse;
    private bool isdrivingforward;
    private float targetangle;
    private float startdrag;
    private float startangdrag;
    private bool isgrounded;
    //private float Rpm;
    private Rigidbody CarRb;
    private float forwardslip;
    private float fdist;
    private float accel;
    private float angle;
    private GameObject currentobstacle;
    private ESFuelManager fuelmanager;
    private ESLightSystem lightAi;
    private bool usefuel;
    private bool uselight;
    private float fuelmul;
    private bool capespeed;
    private float OldRot;

    #endregion
    // Use this for initialization

    private void Start()
    {
        path = GameObject.Find(PathName);
        if (ObstacleTagName.Length > 0)
        {
            Obstacles = GameObject.FindGameObjectsWithTag(ObstacleTagName);
        }
        maxbraketorque = float.MaxValue;
        ReturnTorque = maxtorque;
        InvokeRepeating("DoRandom", 0f, DecisionDelay);
        NitroValue = MaxNitroValue;
        accel = 1;

        CarRb = GetComponent<Rigidbody>();
        audiosettings.audiosource = GetComponent<AudioSource>();
        startangdrag = CarRb.linearDamping;
        startangdrag = CarRb.angularDamping;
        if (this.GetComponent<ESLightSystem>() != null)
        {
            lightAi = GetComponent<ESLightSystem>();
            uselight = true;
        }
        else
        {
            uselight = false;
        }

        CarRb.centerOfMass += carridigbodycenterofmass;
        if (this.GetComponent<ESFuelManager>() != null)
        {
            fuelmanager = GetComponent<ESFuelManager>();
            usefuel = true;

        }
        else
        {
            usefuel = false;
        }
        Transform[] pathtrans = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < pathtrans.Length; i++)
        {
            if (pathtrans[i] != path.transform)
            {
                nodes.Add(pathtrans[i]);
            }
        }

        //

        //calculate gearratio on start
        gearshift.currentgear = 1;
        gearshift.gearratio = (float)gearshift.currentgear / (float)gearshift.maxgear;
        gearshift.inversecurrentgear = gearshift.maxgear;
        gearshift.oldgear = gearshift.currentgear;
        gearshift.inversegearratio = (float)gearshift.inversecurrentgear / (float)gearshift.maxgear;
        topspeed = maxspeed * gearshift.gearratio;
        //

        for (int i = 0; i < gearshift.exhaustflame.Length; i++)
        {
            gearshift.exhaustflame[i].GetComponent<ParticleSystem>().Stop();
            gearshift.exhaustflame[i].GetComponent<ParticleSystem>().Clear();

        }

    }

    private void FixedUpdate()
    {
        ApplyDrag();
        SteerBalance();
        ApplySteer();
        Drive();
        Gear();
        WheelAlignment();
        Braking();
        if (IsMovingTarget)
        {
            followMovingTarget();
        }
        //

        DetectObstacles();

        LerpToSteerAngle();
        AI_Behavoiur();
        TorqueControl();
        if (enginemode == EngineMode.Nitro)
        {
            Nitro();
        }
        AudioAI();
    }
    private void Update()
    {
       
        if (uselight)
        {
            lightAi.LightSystemAI(UseHeadLamp, isbraking, accel, isreverse, High);
        }
        if (enginemode == EngineMode.Nitro)
            if (do_nitro)
            {
                if (NitroValue > 1)
                {
                    NitroValue -= NitroExpense;
                }
            }
    }
    #region SteerBalance
    public void SteerBalance()
    {
        for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
        {
            WheelHit wheelhit;
            wheelsettings.frontwheel.frontwheelcol[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
        }
        for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
        {
            WheelHit wheelhit;
            wheelsettings.rearwheel.rearwheelcol[i].GetGroundHit(out wheelhit);
            if (wheelhit.normal == Vector3.zero)
                return;
        }
        if (Mathf.Abs(OldRot - transform.eulerAngles.y) < 10f)
        {
            var alignturn = (transform.eulerAngles.y - OldRot) * SteerBalanceFactor;
            Quaternion angvelocity = Quaternion.AngleAxis(alignturn, Vector3.up);
            CarRb.linearVelocity = angvelocity * CarRb.linearVelocity;
        }
        OldRot = transform.eulerAngles.y;
    }
    #endregion
    #region Drag
    private void ApplyDrag()
    {
        for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
        {
            isgrounded = wheelsettings.frontwheel.frontwheelcol[i].isGrounded;
        }
        //
        for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
        {
            isgrounded = wheelsettings.rearwheel.rearwheelcol[i].isGrounded;
        }
        //
        CarRb.linearDamping = Mathf.Abs(accel) == 0 ? drag : startdrag;
        CarRb.angularDamping = isgrounded == false ? AirDrag : startangdrag;
    }
    #endregion
    void OnDrawGizmosSelected()
    {
        Vector3 startpoint = transform.position;
        startpoint += transform.forward * sensor.frontsensor.frontsensorpos.z;
        startpoint += transform.up * sensor.frontsensor.frontsensorpos.y;
        //
        Gizmos.DrawLine(startpoint, startpoint + transform.forward * sensor.frontsensor.raylength);
        //
        startpoint += transform.right * sensor.frontsensor.skinwidth;
        Gizmos.DrawLine(startpoint, startpoint + transform.forward * sensor.frontsensor.raylength);
        //
        Gizmos.DrawLine(startpoint, startpoint + (Quaternion.AngleAxis(sensor.frontsensor.angle, transform.up) * transform.forward)
       * sensor.frontsensor.raylength);
        //
        startpoint -= transform.right * sensor.frontsensor.skinwidth *2;
        Gizmos.DrawLine(startpoint, startpoint + transform.forward * sensor.frontsensor.raylength);
        //
      
        //
        Gizmos.DrawLine(startpoint, startpoint + (Quaternion.AngleAxis(-sensor.frontsensor.angle, transform.up )* transform.forward)
        * sensor.frontsensor.raylength);
      

    }
    #region DetectObstacles
    private void DetectObstacles()
    {
        RaycastHit hit = new RaycastHit();
        Vector3 startpoint = transform.position;
        startpoint += transform.forward * sensor.frontsensor.frontsensorpos.z;
        startpoint += transform.up * sensor.frontsensor.frontsensorpos.y;
        float avoidmul = 0f;
        isavoiding = false;
        //center
        //front right sensor
        if (Physics.Raycast(startpoint, transform.forward, out hit, sensor.frontsensor.raylength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                if (hit.collider.CompareTag("Obstacles"))
                {
                    Debug.DrawLine(startpoint, hit.point);
                    isavoiding = true;
                    avoidmul -= 1f;
                }

            }
        }
        //front right sensor
        startpoint += transform.right * sensor.frontsensor.skinwidth;
        if (Physics.Raycast(startpoint, transform.forward, out hit, sensor.frontsensor.raylength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                if (hit.collider.CompareTag("Obstacles"))
                {
                    Debug.DrawLine(startpoint, hit.point);
                    isavoiding = true;
                    avoidmul -= 1f;
                }

            }
        }
        //  right angled sensor
        else if (Physics.Raycast(startpoint, Quaternion.AngleAxis(sensor.frontsensor.angle, transform.up) * transform.forward, out hit, sensor.frontsensor.raylength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                if (hit.collider.CompareTag("Obstacles"))
                {
                    Debug.DrawLine(startpoint, hit.point);
                    isavoiding = true;
                    avoidmul -= 1f;

                }

            }
        }
        if (!IsMovingTarget)
            CheckRangePointDist(hit);
        // front left sensor
        startpoint -= transform.right * sensor.frontsensor.skinwidth * 2f;
        if (Physics.Raycast(startpoint, transform.forward, out hit, sensor.frontsensor.raylength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                if (hit.collider.CompareTag("Obstacles"))
                {
                    Debug.DrawLine(startpoint, hit.point);
                    isavoiding = true;
                    avoidmul += 1f;
                }

            }
        }

        //  left angled sensor
        else if (Physics.Raycast(startpoint, Quaternion.AngleAxis(-sensor.frontsensor.angle, transform.up) * transform.forward, out hit, sensor.frontsensor.raylength))
        {
            if (!hit.collider.CompareTag("Terrain"))
            {
                if (hit.collider.CompareTag("Obstacles"))
                {
                    Debug.DrawLine(startpoint, hit.point);
                    isavoiding = true;
                    avoidmul += 1f;
                }

            }
        }
        if (!IsMovingTarget)
            CheckRangePointDist(hit);
        if (avoidmul == 0)
        {
            //front sensor
            if (Physics.Raycast(startpoint, transform.forward, out hit, sensor.frontsensor.raylength))
            {
                if (!hit.collider.CompareTag("Terrain"))
                {
                    if (hit.collider.CompareTag("Obstacles"))
                    {
                        Debug.DrawLine(startpoint, hit.point);
                        isavoiding = true;
                        if (hit.normal.x < 0)
                        {
                            avoidmul = -1f;
                        }
                        else
                        {
                            avoidmul = 1f;
                        }
                    }

                }
            }
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Obstacles")
                {
                    if (hit.distance < 0.1f && !isreverse)
                    {
                        StartCoroutine(ReverseTime(reversetime));
                    }
                }
            }
            if (!IsMovingTarget)
                CheckRangePointDist(hit);
        }
        // loop below generates side sensors
        RaycastHit hit2 = new RaycastHit();
        #region sidesensor
        if (sensor.sidesensor._Leftsidesensors != null && sensor.sidesensor._Rightsidesensors != null)
        {
            if (sensor.sidesensor._Leftsidesensors.avoid)
            {
                isavoiding = true;
                avoidmul = sensor.sidesensor.steersensitivity;

            }
            else if (sensor.sidesensor._Rightsidesensors.avoid)
            {
                isavoiding = true;
                avoidmul = -sensor.sidesensor.steersensitivity;

            }
        }
       
        if (!IsMovingTarget)
            CheckRangePointDist(hit2);
        #endregion
        if (isavoiding)
        {
            targetangle = maxsteerangle * avoidmul;
        }
    }
    #endregion
    #region LerpToSteerAngle
    private void LerpToSteerAngle()
    {
        // gently steers wheel to angle of destination
        for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
        {
            wheelsettings.frontwheel.frontwheelcol[i].steerAngle = Mathf.Lerp(wheelsettings.frontwheel.frontwheelcol[i].steerAngle, targetangle, Time.deltaTime * turnspeed);
        }
    }
    #endregion
    #region ApplySteer
    private void ApplySteer()
    {
        if (isavoiding) return;
        Vector3 relativevec = transform.InverseTransformPoint(nodes[currentnode].position);
        relativevec = relativevec / relativevec.magnitude;
        float newsteer = (relativevec.x / relativevec.magnitude) * maxsteerangle;
        targetangle = newsteer;
    }
    #endregion
    #region Braking
    private void Braking()
    {
        if (isbraking)
        {
            for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
            {
                wheelsettings.frontwheel.frontwheelcol[i].brakeTorque = maxbraketorque;
            }
            for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
            {
                wheelsettings.rearwheel.rearwheelcol[i].brakeTorque = maxbraketorque;
            }
        }
        else
        {
            for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
            {
                wheelsettings.frontwheel.frontwheelcol[i].brakeTorque = 0f;
            }
            for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
            {
                wheelsettings.rearwheel.rearwheelcol[i].brakeTorque = 0f;
            }
        }
    }
    #endregion
    #region WheelAlignment
    private void WheelAlignment()
    {
        // make tyre meshes follow wheels;

        // align front wheel meshes
        Vector3 frontwheelposition;
        Quaternion frontwheelrotation;
        for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
        {
            if (wheelsettings.frontwheel.frontwheelmesh[i] == null)
            {
                // Debug.Log("frontwheelmesh missing");
                return;
            }
            wheelsettings.frontwheel.frontwheelcol[i].GetWorldPose(out frontwheelposition, out frontwheelrotation);
            wheelsettings.frontwheel.frontwheelmesh[i].transform.position = frontwheelposition;
            wheelsettings.frontwheel.frontwheelmesh[i].transform.rotation = frontwheelrotation;
            //Rpm = wheelsettings.frontwheel.frontwheelcol[i].rpm;
        }

        // align rear wheel meshes
        Vector3 rearwheelposition;
        Quaternion rearwheelrotation;
        for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
        {
            if (wheelsettings.rearwheel.rearwheelmesh[i] == null)
            {
                //Debug.Log("rearwheelmesh missing");
                return;
            }
            wheelsettings.rearwheel.rearwheelcol[i].GetWorldPose(out rearwheelposition, out rearwheelrotation);
            wheelsettings.rearwheel.rearwheelmesh[i].transform.position = rearwheelposition;
            wheelsettings.rearwheel.rearwheelmesh[i].transform.rotation = rearwheelrotation;
        }
    }
    #endregion
    #region Drive
    private void Drive()
    {
        switch (drivetype)
        {
            case DriveType.FourWheel:
                {
                    if (!isbraking)
                    {
                        for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
                        {
                            wheelsettings.frontwheel.frontwheelcol[i].motorTorque = mul * accel * (forceAppliedToWheels) / 4f;
                        }
                        for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
                        {
                            wheelsettings.rearwheel.rearwheelcol[i].motorTorque = mul * accel * (forceAppliedToWheels) / 4f;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
                        {
                            wheelsettings.frontwheel.frontwheelcol[i].motorTorque = 0;
                        }
                        for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
                        {
                            wheelsettings.rearwheel.rearwheelcol[i].motorTorque = 0;
                        }
                    }
                }
                break;
            case DriveType.FrontWheel:
                {
                    if (!isbraking)
                    {
                        for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
                        {
                            wheelsettings.frontwheel.frontwheelcol[i].motorTorque = mul * accel * (forceAppliedToWheels) / 2f;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
                        {
                            wheelsettings.frontwheel.frontwheelcol[i].motorTorque = 0;
                        }
                    }

                }
                break;
            case DriveType.RearWheel:
                {
                    if (!isbraking)
                    {
                        for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
                        {
                            wheelsettings.rearwheel.rearwheelcol[i].motorTorque = mul * accel * (forceAppliedToWheels) / 2f;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
                        {
                            wheelsettings.rearwheel.rearwheelcol[i].motorTorque = 0;
                        }
                    }
                }
                break;
        }


    }
    #endregion
    #region Gear
    private void Gear()
    {
        if (gearshift.neutral)
        {
            //empty haha.
        }
        gearshift.enginerpm = currentspeed * gearshift.inversegearratio;
        if (currentspeed >= topspeed)
        {
            GearTransmissionUp();
        }
        if (currentspeed <= gearshift.oldTopSpeed)
        {
            GearTransmissionDown();
        }
    }
    #endregion
    #region GearTransmissionUp
    private void GearTransmissionUp()
    {
        if (gearshift.currentgear < gearshift.maxgear)
        {
            gearshift.currentgear++;
            if (gearshift.geartransmissioneffect)
            {
                StartCoroutine(flametime(gearshift.effecttime));
            }
            if (gearshift.currentgear > 1)
            {
                gearshift.oldgear = gearshift.currentgear - 1;
            }
            gearshift.inversecurrentgear--;
            gearshift.inversegearratio = (float)gearshift.inversecurrentgear / (float)gearshift.maxgear;
            gearshift.gearratio = (float)gearshift.currentgear / (float)gearshift.maxgear;
            topspeed = maxspeed * gearshift.gearratio;
            gearshift.oldgearratio = (float)gearshift.oldgear / (float)gearshift.maxgear;
            gearshift.oldTopSpeed = maxspeed * gearshift.oldgearratio;
        }
    }
    #endregion
    #region GearTransmissionDown
    private void GearTransmissionDown()
    {
        if (gearshift.currentgear > 1)
        {
            gearshift.currentgear--;
            gearshift.inversecurrentgear++;
            gearshift.inversegearratio = (float)gearshift.inversecurrentgear / (float)gearshift.maxgear;
            //
            if (gearshift.currentgear > 1)
            {
                gearshift.oldgear = gearshift.currentgear - 1;
            }
            gearshift.gearratio = (float)gearshift.currentgear / (float)gearshift.maxgear;
            topspeed = maxspeed * gearshift.gearratio;
            gearshift.oldgearratio = (float)gearshift.oldgear / (float)gearshift.maxgear;
            gearshift.oldTopSpeed = maxspeed * gearshift.oldgearratio;
        }
    }
    #endregion
    #region CarSpeed
    //CarSpeed
    public void CarSpeed()
    {
        //km/h
        float Pi = Mathf.PI * 1.15f;
        currentspeed = CarRb.linearVelocity.magnitude * Pi;
        if (!capespeed)
        {
            if (currentspeed > topspeed)
                CarRb.linearVelocity = (topspeed / Pi) * CarRb.linearVelocity.normalized;
        }
        else
        {
            if (currentspeed > smoothtargetspeed)
                CarRb.linearVelocity = (smoothtargetspeed / Pi) * CarRb.linearVelocity.normalized;
        }
    }
    #endregion
    #region TorqueControl()
    public void TorqueControl()
    {

        if (fuelmanager != null)
        {
            if (fuelmanager.Empty)
            {
                usefuel = false;
            }
            else
            {
                usefuel = true;
            }
        }

        WheelHit wheelhit = new WheelHit();
        switch (drivetype)
        {
            case DriveType.FourWheel:
                {
                    for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
                    {
                        wheelsettings.frontwheel.frontwheelcol[i].GetGroundHit(out wheelhit);
                    }
                    for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
                    {
                        wheelsettings.rearwheel.rearwheelcol[i].GetGroundHit(out wheelhit);
                    }
                }
                break;
            case DriveType.RearWheel:
                {

                    for (int i = 0; i < wheelsettings.rearwheel.rearwheelcol.Length; i++)
                    {
                        wheelsettings.rearwheel.rearwheelcol[i].GetGroundHit(out wheelhit);
                    }
                }
                break;
            case DriveType.FrontWheel:
                {
                    for (int i = 0; i < wheelsettings.frontwheel.frontwheelcol.Length; i++)
                    {
                        wheelsettings.frontwheel.frontwheelcol[i].GetGroundHit(out wheelhit);
                    }
                }
                break;
        }

        forwardslip = wheelhit.forwardSlip;

        if (forwardslip >= sliplimit && forceAppliedToWheels >= 0)
        {
            forceAppliedToWheels -= 10 * tractionfactor;
        }
        else
        {
            if (forceAppliedToWheels < maxtorque)
            {
                forceAppliedToWheels += 10 * tractionfactor;
            }
        }

        CarSpeed();
    }
    #endregion
    #region CheckPointDistance
    private void CheckRangePointDist(RaycastHit hit)
    {
        if (hit.collider == null)
        {
            //
            if (Vector3.Distance(transform.position, nodes[currentnode].position) < distanceapart)
            {
                if (currentnode == nodes.Count - 1)
                {
                    currentnode = 0;
                }
                else
                {
                    currentnode++;
                }
            }
        }
        else
        {
            // skips current node  and moves to next if obstacle is close to currnetnode
            if (hit.collider.tag == "Obstacles")
            {
                if (Vector3.Distance(hit.point, nodes[currentnode].position) < 30f)
                {
                    if (showdebuglog)
                    {
                        Debug.Log("Node is close to obstacle so i'm gonna skip to the next :)");
                    }
                    if (currentnode == nodes.Count - 1)
                    {
                        currentnode = 0;
                    }
                    else
                    {
                        currentnode++;
                    }
                }
            }
        }
    }
    #endregion
    #region FollowMovingTarget
    private void followMovingTarget()
    {
        if (Vector3.Distance(transform.position, nodes[0].transform.position) > distanceapart || Vector3.Distance(transform.position, nodes[0].transform.position) < 10)
        {
            //print("Too far from target Ai will stop tracking");
            StopTracking = true;

        }
        else
        {
            StopTracking = false;
        }
        //
        if (StopTracking)
        {
            isbraking = true;
        }
        //

    }
    #endregion
    //
    void AiTakePrecution()
    {

        if (currentspeed > smoothtargetspeed)
        {
            if (drift.drifting == false)
            {
                isbraking = true;
            }
        }

    }
    #region fucntions
    public float Vector3distance(Vector3 from, Vector3 to)
    {
        float distfloat = from.z - to.z;
        return distfloat;
    }
    // 
    #endregion
    #region AI behavoiur
    private void AI_Behavoiur()
    {
        // behaviour of your ai car
        //
        //gets direction and angle
        Vector3 dir = nodes[currentnode].position - transform.position;
        float tempangle = Vector3.Angle(transform.forward, dir);
        angle = tempangle;
        if (!StopTracking)
        {
            if (IsMovingTarget)
            {
                if (angle > smoothtargetangle && currentspeed > smoothtargetspeed)
                {
                    capespeed = true;
                    AiTakePrecution();
                }
                else
                {
                    capespeed = false;
                    if (drift.drifting == false)
                        isbraking = false;
                }
            }
            //

            if (angle > drift.driftangle && currentspeed > drift.killriftspeed)
            {
                DoDriftAI(true);
                drift.drifting = true;
                if (currentspeed > drift.killbrakespeed)
                {
                    isbraking = true;
                }
            }
            else
            {
                drift.drifting = false;
                DoDriftAI(false);
            }
            //
            if (!IsMovingTarget && !StopTracking)
            {
                //
                Vector3 nextnodedir = new Vector3();
                if (Vector3.Distance(transform.position, nodes[currentnode].position) < 100f && currentspeed > smoothtargetspeed)
                {

                    //
                    if (currentnode != nodes.Count - 1)
                    {
                        nextnodedir = nodes[currentnode + 1].position - transform.position;
                    }
                    if (currentnode == nodes.Count)
                    {
                        nextnodedir = nodes[nodes.Count - 1].position - transform.position;
                    }
                    float nextnodeang = Vector3.Angle(transform.forward, nextnodedir);
                    angle = nextnodeang;
                    if (nextnodeang > smoothtargetangle && currentspeed > smoothtargetspeed)
                    {
                        capespeed = true;
                        AiTakePrecution();
                    }
                    else
                    {
                        capespeed = false;
                        if (drift.drifting == false)
                            isbraking = false;
                    }
                }
                else
                {

                    if (angle > smoothtargetangle && currentspeed > smoothtargetspeed)
                    {
                        capespeed = true;
                        AiTakePrecution();
                    }
                    else
                    {
                        capespeed = false;
                        if (drift.drifting == false)
                            isbraking = false;
                    }
                }
            }
        }
        //reverse
        if (usefuel)
        {
            if (fuelmanager.Empty)
            {
                fuelmul = 0f;
            }
            else
            {
                fuelmul = 1f;
            }
        }
        else
        {
            fuelmul = 1f;
        }
        if (isreverse)
        {
            accel = -1 * fuelmul;
        }
        else
        {
            accel = 1 * fuelmul;
        }

        #region foreach

        foreach (GameObject obstacles in Obstacles)
        {

            if (Vector3distance(obstacles.transform.position, transform.position) > 0)
            {
                currentobstacle = obstacles;
            }
            if (currentobstacle != null)
            {
                if (Vector3.Distance(currentobstacle.transform.position, transform.position) < detectiondistance)
                {
                    // gets distance apart
                    Vector3 dist = currentobstacle.transform.position - transform.position;
                    fdist = dist.z;
                    if (!isreverse)
                    {
                        if (currentspeed <= smoothtargetspeed && currentspeed > smoothtargetspeed - 2f && fdist > 5f)
                        {
                            accel = -1 * fuelmul;
                        }
                        else
                        {
                            accel = 1 * fuelmul;
                        }
                    }
                    switch (aibehaviour)
                    {
                        // ai drives a little bit responsible
                        case AIBehavoiur.Gentle:
                            {
                                if (currentobstacle.GetComponent<Rigidbody>().linearVelocity.magnitude <= 0)
                                {
                                    if (currentspeed > smoothtargetspeed && fdist > 5f)
                                    {
                                        capespeed = true;
                                        AiTakePrecution();
                                    }
                                    else
                                    {
                                        if (!StopTracking)
                                        {
                                            capespeed = false;
                                            if (drift.drifting == false)
                                                isbraking = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (currentspeed > smoothtargetspeed)
                                    {
                                        capespeed = true;
                                        AiTakePrecution();
                                    }
                                    else
                                    {
                                        if (!StopTracking)
                                        {
                                            capespeed = false;
                                            if (drift.drifting == false)
                                                isbraking = false;
                                        }
                                    }
                                }

                            }
                            break;
                        case AIBehavoiur.Aggressive:
                            {
                                // ai drives aggressive
                                if (currentobstacle.GetComponent<Rigidbody>().linearVelocity.magnitude <= 0)
                                {
                                    if (currentspeed > smoothtargetspeed && fdist > 20f)
                                    {
                                        capespeed = true;
                                        AiTakePrecution();
                                    }
                                    else
                                    {
                                        if (!StopTracking)
                                        {
                                            capespeed = false;
                                            if (drift.drifting == false)
                                                isbraking = false;
                                        }
                                    }
                                }
                                else
                                {
                                    if (currentspeed > smoothtargetspeed + (smoothtargetspeed * 0.5f))
                                    {
                                        capespeed = true;
                                        AiTakePrecution();
                                    }
                                    else
                                    {
                                        if (!StopTracking)
                                        {
                                            capespeed = false;
                                            if (drift.drifting == false)
                                                isbraking = false;
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
                else
                {
                    if (!isreverse)
                    {
                        accel = 1 * fuelmul;
                    }
                }
            }
        }
        #endregion
        #region Wrapmode
        if (_wrapmode == AiWrapMode.Once)
        {

            if (currentnode == nodes.Count - 1)
            {
                once = true;

            }
        }
        if (once)
        {
            isbraking = true;
        }

        if (_wrapmode == AiWrapMode.Continous)
        {
            once = false;
        }
        #endregion
    }
    #endregion
    #region drift
    private void DoDriftAI(bool isdfriting)
    {
        for (int i = 0; i < drift.esdriftAI.Length; i++)
        {
            drift.esdriftAI[i].m_dodrift = isdfriting;
        }
    }
    #endregion
    #region audio
    public void AudioAI()
    {
        audiosettings.audiosource.pitch = (gearshift.enginerpm  / audiosettings.pitchmultiplier) + audiosettings.pitchmodifier;
        // generate realistic engine sound
        if (usefuel)
        {
            if (fuelmanager.Empty && audiosettings.audiosource.isPlaying)
            {
                audiosettings.audiosource.Stop();
            }
            else if (!fuelmanager.Empty && !audiosettings.audiosource.isPlaying)
            {
                audiosettings.audiosource.Play();
            }
        }
        else
        {
            if (audiosettings.audiosource.isPlaying == false)
            {
                audiosettings.audiosource.Play();
            }
        }
       
    }
    #endregion
    #region Nitro
    private void Nitro()
    {
        if (do_nitro)
        {

            if (NitroValue > 1)
            {
                //AddNitroForce.
                // NitroValue -= NitroExpense;
                maxtorque = NitroTorque;

                for (int i = 0; i < gearshift.exhaustflame.Length; i++)
                {
                    gearshift.exhaustflame[i].GetComponent<ParticleSystem>().Emit(1);
                }


            }
            if (NitroValue < 1)
            {
                maxtorque = ReturnTorque;

                for (int i = 0; i < gearshift.exhaustflame.Length; i++)
                {
                    gearshift.exhaustflame[i].GetComponent<ParticleSystem>().Stop();
                }

                do_nitro = false;
                //juststartednitro = false;

            }
        }

        if (isbraking)
        {
            if (do_nitro)
            {
                maxtorque = ReturnTorque;
                do_nitro = false;
                //juststartednitro = false;
            }
        }
        if (CarRb.linearVelocity.magnitude < 10f)
        {
            if (do_nitro)
            {
                do_nitro = false;
                //juststartednitro = false;
            }
        }
        if (!do_nitro)
        {
            maxtorque = ReturnTorque;
            if (forceAppliedToWheels != maxtorque)
                forceAppliedToWheels = maxtorque;
        }

        if (nitro_usage == NitroUsage.Immediate)
        {
            if (NitroValue > 1 && CarRb.linearVelocity.magnitude > 10f)
            {
                do_nitro = true;
            }
        }
        if (nitro_usage == NitroUsage.Decisive)
        {
            if (decisionindex == 2)
            {
                Agressive_Gentle_NitroAI();
            }


            if (decisionindex == 4)
            {
                Agressive_Gentle_NitroAI();
            }
            if (aibehaviour == AIBehavoiur.Gentle)
            {
                if (decisionindex == 0 || decisionindex == 1 || decisionindex == 3)
                {
                    do_nitro = false;
                }
            }
        }
    }
    #endregion
    private void Agressive_Gentle_NitroAI()
    {
        // behavoiur of ai in nitro mode
        if (aibehaviour == AIBehavoiur.Gentle)
        {
            if (CarRb.linearVelocity.magnitude > 10f && NitroValue > 1)
            {
                if (!isbraking)
                {
                    do_nitro = true;
                }
                else
                {
                    do_nitro = false;
                }
            }
        }
        if (aibehaviour == AIBehavoiur.Aggressive)
        {
            if (CarRb.linearVelocity.magnitude > 10f && NitroValue > 1)
            {
                do_nitro = true;
            }
        }
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<ESNitroSetup>() != null)
        {
            if (NitroValue < MaxNitroValue)
            {
                PickUpSource.clip = other.gameObject.GetComponent<ESNitroSetup>().PickUpSound;
                PickUpSource.Play();
                //print("Picked Up" + other.gameObject.name);
                float addtonitrovalue = MaxNitroValue - NitroValue;
                float initialnitroval = NitroValue;
                if (addtonitrovalue > other.gameObject.GetComponent<ESNitroSetup>().NitroValue)
                {
                    NitroValue = other.gameObject.GetComponent<ESNitroSetup>().NitroValue + initialnitroval;
                }
                if (addtonitrovalue < other.gameObject.GetComponent<ESNitroSetup>().NitroValue)
                {
                    NitroValue = MaxNitroValue;
                }

                Destroy(other.gameObject);
            }
            else
            {
                // print("Did Not Pick Up" + other.gameObject.name);
            }
        }
    }

    private void DoRandom()
    {
        decisionindex = UnityEngine.Random.Range(0, 5);
    }

    IEnumerator ReverseTime(float time)
    {
        // how long the ai vehicle can reverse
        isreverse = true;
        yield return new WaitForSeconds(time);
        isreverse = false;
    }

    IEnumerator DriftTime(float time)
    {
        // how long the ai vehicle can drift
        DoDriftAI(true);
        drift.drifting = true;
        yield return new WaitForSeconds(time);
        drift.drifting = false;
        DoDriftAI(false);
    }
    IEnumerator flametime(float t)
    {
        for (int i = 0; i < gearshift.exhaustflame.Length; i++)
        {
            gearshift.exhaustflame[i].GetComponent<ParticleSystem>().Emit(1);
        }


        yield return new WaitForSeconds(t);
        for (int i = 0; i < gearshift.exhaustflame.Length; i++)
        {
            gearshift.exhaustflame[i].GetComponent<ParticleSystem>().Stop();
        }

    }
}

