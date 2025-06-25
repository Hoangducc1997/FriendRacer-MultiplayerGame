using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(ESVehicleAI))]
public class ESVehicleAIEditor :Editor
{
    public ESVehicleAI myscript;
    public int tab;

    public override void OnInspectorGUI()
    {

        myscript = target as ESVehicleAI;
        ESVehicleAICustomInpspector(myscript);
        
        if (GUI.changed)
        {
            EditorUtility.SetDirty(myscript);
        }
    }

    public void ESVehicleAICustomInpspector(ESVehicleAI ESAI)
    {
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("EngineSettings", MessageType.None);
        ESVehicleAI.DriveType _drivtype = new ESVehicleAI.DriveType();
        ESVehicleAI.EngineMode enginemode = new ESVehicleAI.EngineMode();

        float MaxTorque = new float();
        float MaxBrakeForce = new float();
        float Maxspeed = new float();
        float MaxSteerAngle = new float();
        float TurnSpeed = new float();
        float ReverseTime = new float();
        float SlipLimit = new float();
        float TractionFactor = new float();
        float SteerBalanceFactor = new float();
        Vector3 CenterOfMass = new Vector3();

        _drivtype = (ESVehicleAI.DriveType)EditorGUILayout.EnumPopup("DriveType", myscript.drivetype);
        enginemode = (ESVehicleAI.EngineMode)EditorGUILayout.EnumPopup("EngineMode", myscript.enginemode);
        MaxTorque = EditorGUILayout.FloatField("MaxTorque", myscript.maxtorque);
        Maxspeed = EditorGUILayout.FloatField("MaxSpeed", myscript.maxspeed);
        MaxBrakeForce = EditorGUILayout.FloatField("MaxBrakeTorque", myscript.maxbraketorque);
        MaxSteerAngle = EditorGUILayout.FloatField("MaxSteerAngle", myscript.maxsteerangle);
        SteerBalanceFactor = EditorGUILayout.FloatField("SteerBalanceFactor", myscript.SteerBalanceFactor);
        TurnSpeed = EditorGUILayout.FloatField("TurnSpeed", myscript.turnspeed);
        ReverseTime = EditorGUILayout.FloatField("ReverseTime", myscript.reversetime);
        SlipLimit = EditorGUILayout.FloatField("SlipLimit", myscript.sliplimit);
        TractionFactor = EditorGUILayout.FloatField("TractionFactor", myscript.tractionfactor);
        CenterOfMass = EditorGUILayout.Vector3Field("CenterOfMass", myscript.carridigbodycenterofmass);

        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("Sensor", MessageType.None);
        ArrayEditor("sensor");
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("GearSettings", MessageType.None);
        ArrayEditor("gearshift");
        EditorGUILayout.EndVertical();


        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("WheelSettings", MessageType.None);
        ArrayEditor("wheelsettings");
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("DriftSettings", MessageType.None);
        ArrayEditor("drift");
        EditorGUILayout.EndVertical();


        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("AudioSettings", MessageType.None);
        ArrayEditor("audiosettings");
        myscript.currentnode = EditorGUILayout.IntField("currnode", myscript.currentnode);
        EditorGUILayout.EndVertical();

        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("PathSettings", MessageType.None);
        string PathName = "";
        float DistanceApart = new float();
        bool Ismovingtar = new bool();
        string Obstaclename = "";
        ESVehicleAI.AiWrapMode _aiwrapmode = new ESVehicleAI.AiWrapMode();
        _aiwrapmode = (ESVehicleAI.AiWrapMode)EditorGUILayout.EnumPopup("AiWrapMode", myscript._wrapmode);
        PathName = EditorGUILayout.TextField("PathName", myscript.PathName);
        DistanceApart = EditorGUILayout.FloatField("DistanceApart", myscript.distanceapart);
        Obstaclename = EditorGUILayout.TextField("ObstacleTagName", myscript.ObstacleTagName);
        ArrayEditor("Obstacles");
        Ismovingtar = EditorGUILayout.Toggle("IsMovingTarget", myscript.IsMovingTarget);
        EditorGUILayout.EndVertical();

        ESVehicleAI.NitroUsage NitroUsage = new ESVehicleAI.NitroUsage();
        AudioSource PickUPAudioSource = new AudioSource();

        float nitrotorque = new float();
        float DecisionDelay = new float();
        float MaxNitroValue = new float();
        float NitroExpense = new float();
        if (myscript.enginemode == ESVehicleAI.EngineMode.Nitro)
        {
            GUILayout.Space(10);
            EditorGUILayout.BeginVertical("Box");
            EditorGUILayout.HelpBox("NitroSettings", MessageType.None);
         


            NitroUsage = (ESVehicleAI.NitroUsage)EditorGUILayout.EnumPopup("NitroUsage", myscript.nitro_usage);
            nitrotorque = EditorGUILayout.FloatField("NitroTorque", myscript.NitroTorque);
            if (NitroUsage == ESVehicleAI.NitroUsage.Decisive)
            {
                DecisionDelay = EditorGUILayout.FloatField("DecisionDelay", myscript.DecisionDelay);
            }
            MaxNitroValue = EditorGUILayout.Slider("MaxNitroValue", myscript.MaxNitroValue, 0, 100);
            NitroExpense = EditorGUILayout.Slider("NitroExpense", myscript.NitroExpense, 0, 10);
            PickUPAudioSource = EditorGUILayout.ObjectField("PickUpAudioSource", myscript.PickUpSource, typeof(AudioSource), true) as AudioSource;

            EditorGUILayout.EndVertical();
        }

        GUILayout.Space(10);
        EditorGUILayout.BeginVertical("Box"); 
        EditorGUILayout.HelpBox("AIBehaviour", MessageType.None);
        ESVehicleAI.AIBehavoiur Aibehaviour = new ESVehicleAI.AIBehavoiur();
        Aibehaviour = (ESVehicleAI.AIBehavoiur)EditorGUILayout.EnumPopup("Aibehvoiur", myscript.aibehaviour);

        float detectiondistance = new float();
        float smoothtargetangle = new float();
        float smoothtargetspeed = new float();
        float smoothTargetDistance = new float();

        detectiondistance =     EditorGUILayout.FloatField("DetectionDistance",myscript.detectiondistance);
        smoothtargetangle =     EditorGUILayout.FloatField("SmoothTargetAngle",myscript.smoothtargetangle);
        smoothtargetspeed =     EditorGUILayout.FloatField("SmoothTargetSpeed",myscript.smoothtargetspeed);
        smoothTargetDistance = EditorGUILayout.FloatField("SmoothTargetDistance", myscript.smoothTargetDistance);

        EditorGUILayout.EndVertical();
        //

        EditorGUILayout.BeginVertical("Box");
        EditorGUILayout.HelpBox("Balance", MessageType.None);
        float drag = new float();
        float airdrag = new float();


        drag = EditorGUILayout.FloatField("Drag", myscript.drag);
        airdrag = EditorGUILayout.FloatField("AirDrag", myscript.AirDrag);
        EditorGUILayout.EndVertical();


        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(myscript, "Changes");
            myscript.drivetype = _drivtype;
            myscript.maxtorque = MaxTorque;
            myscript.maxspeed = Maxspeed;
            myscript.maxbraketorque = MaxBrakeForce;
            myscript.PathName = PathName;
            myscript.distanceapart = DistanceApart;
            myscript.IsMovingTarget = Ismovingtar;
            myscript.SteerBalanceFactor = SteerBalanceFactor;
            myscript._wrapmode = _aiwrapmode;
            myscript.nitro_usage = NitroUsage;
            myscript.NitroTorque = nitrotorque;
            myscript.DecisionDelay = DecisionDelay;
            myscript.NitroExpense = NitroExpense;
            myscript.MaxNitroValue = MaxNitroValue;
            myscript.PickUpSource = PickUPAudioSource;
            myscript.turnspeed = TurnSpeed;
            myscript.maxsteerangle = MaxSteerAngle;
            myscript.reversetime = ReverseTime;
            myscript.sliplimit = SlipLimit;
            myscript.tractionfactor = TractionFactor;
            myscript.aibehaviour = Aibehaviour;
            myscript.smoothtargetangle = smoothtargetangle;
            myscript.smoothTargetDistance = smoothTargetDistance;
            myscript.smoothtargetspeed = smoothtargetspeed;
            myscript.detectiondistance = detectiondistance;
            myscript.enginemode = enginemode;
            myscript.ObstacleTagName = Obstaclename;
            myscript.carridigbodycenterofmass = CenterOfMass;
            myscript.AirDrag = airdrag;
            myscript.drag = drag;
        }

    }

    private void ArrayEditor(string word)
    {
        SerializedProperty MyArrayprop = serializedObject.FindProperty(word);
        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(MyArrayprop, true);
        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            //EditorGUIUtility.LookLikeControls();
        }
        serializedObject.ApplyModifiedProperties();
    }	
}
