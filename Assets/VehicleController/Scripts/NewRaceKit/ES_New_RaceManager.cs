using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class ES_New_RaceManager : MonoBehaviour
{
    public Transform Player;
    public Transform PathParent;
    public string SceneName = "NewRace2021";
    public Slider PlayerNitroSlider;
    public Text PositionText;
    [Range(1, 5)] public int MaxLap = 5;
    public GameObject ScorePanel;
    public Text ScoreText;
    public Text SaluteText;
    public Text LapText;
    public GameObject ReportText;
    public List<Transform> AIvehicle = new List<Transform>(1);
    //
    [HideInInspector] public List<Transform> VehiclesAbove = new List<Transform>();
    [HideInInspector] public List<Transform> VehiclesBehind = new List<Transform>();
    private List<Transform> TargetNodes;
    private bool OneCheck;

    // Start is called before the first frame update
    void Awake()
    {
        TargetNodes = new List<Transform>();
        if (PathParent != null)
        {
            Transform[] nodes = PathParent.GetComponentsInChildren<Transform>();
            for (int i = 0; i < nodes.Length; ++i)
            {
                if (nodes[i].transform != PathParent.transform)
                {
                    TargetNodes.Add(nodes[i]);
                }
            }
        }
        //
        TargetNodes[TargetNodes.Count - 1].GetComponent<ESShowForwardDirection>().LapCounter = true;
        Player.GetComponent<ESSmartRaceAgent>().LapModifier = TargetNodes[TargetNodes.Count - 1].GetComponent<ESShowForwardDirection>().PointValue;
        VehiclesAbove = new List<Transform>();
        if (AIvehicle.Count > 0)
        {
            for (int i = 0; i < AIvehicle.Count; ++i)
            {
                VehiclesAbove.Add(AIvehicle[i]);
                AIvehicle[i].GetComponent<ESAdvanceVehicleAI>().TargetNode = TargetNodes[0];
                AIvehicle[i].GetComponent<ESSmartRaceAgent>().LapModifier = TargetNodes[TargetNodes.Count - 1].GetComponent<ESShowForwardDirection>().PointValue;
            }
        }


    }
    //
    private void StopRace()
    {
        if (Player.GetComponent<ESSmartRaceAgent>().CurrentLap > MaxLap)
        {

            Player.GetComponent<ESGearShift>().isparking = true;
            ScorePanel.gameObject.SetActive(true);
            if (Player.GetComponent<ESSmartRaceAgent>().CurrentLap <= 3)
            {
                SaluteText.text = "Welldone";
                OneCheck = true;
            }
            else
            {
                OneCheck = true;
                SaluteText.text = "TryAgain";
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                SceneManager.LoadScene(SceneName);
            }
        }
    }
    // Update is called once per frame
    void LateUpdate()
    {
        RaceManagement();
        ShowText();
        StopRace();
        if (PlayerNitroSlider != null)
        {
            PlayerNitroSlider.interactable = false;
            PlayerNitroSlider.value = Player.GetComponent<ESGearShift>().NitroValue;
        }
    }
    //
    void RaceManagement()
    {
        AssignPlayerPos();
        if (Player.GetComponent<ESSmartRaceAgent>().ang < 0 &&
         Player.GetComponent<ESSmartRaceAgent>().Rang > Player.GetComponent<ESSmartRaceAgent>().ResetAngle)
        {
            ReportText.SetActive(true);
        }
        else
        {
            ReportText.SetActive(false);
        }
    }
    //
    void ShowText()
    {
        if (LapText == null && PositionText == null) return;

        if (!OneCheck)
        {
            PositionText.text = GetPlayerPosition().ToString();
            ScoreText.text = PositionText.text;
        }
        LapText.text = Player.GetComponent<ESSmartRaceAgent>().CurrentLap.ToString();
    }
    //
    public float GetPlayerPosition()
    {
        int m = new int();
        return m = (VehiclesAbove.Count + 1);
    }
    void AssignPlayerPos()
    {
        VehiclesAbove = new List<Transform>();
        VehiclesBehind = new List<Transform>();
        foreach (Transform Ai in AIvehicle)
        {
            ESSmartRaceAgent AIsmartRaceAgent = Ai.GetComponent<ESSmartRaceAgent>();
            ESSmartRaceAgent PlayersmartRaceAgent = Player.GetComponent<ESSmartRaceAgent>();
            if (AIsmartRaceAgent.GetWayPointIndentity() > PlayersmartRaceAgent.GetWayPointIndentity())
            {
                VehiclesAbove.Add(AIsmartRaceAgent.transform);
            }
            if (AIsmartRaceAgent.GetWayPointIndentity() < PlayersmartRaceAgent.GetWayPointIndentity())
            {
                VehiclesBehind.Add(AIsmartRaceAgent.transform);
            }
            if (AIsmartRaceAgent.GetWayPointIndentity() == PlayersmartRaceAgent.GetWayPointIndentity())
            {
                if (AIsmartRaceAgent.DistanceFromLastWayPoint < PlayersmartRaceAgent.DistanceFromLastWayPoint)
                {
                    VehiclesAbove.Add(AIsmartRaceAgent.transform);
                }
                if (AIsmartRaceAgent.DistanceFromLastWayPoint > PlayersmartRaceAgent.DistanceFromLastWayPoint)
                {
                    VehiclesBehind.Add(AIsmartRaceAgent.transform);
                }
            }

        }
        //
    }
    //
}
