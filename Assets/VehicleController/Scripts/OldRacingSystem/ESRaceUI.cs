using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ESRaceUI : MonoBehaviour
{
    public Text MaxLap;
    public Text Current_Lap;
    public Text currpostxt;
    public ESRaceController rc;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MaxLap.text = "MaxLap :" + " " + rc.Num_Of_Laps;
        Current_Lap.text = "Lap :" + " " + rc.currentlap;
    }
}
