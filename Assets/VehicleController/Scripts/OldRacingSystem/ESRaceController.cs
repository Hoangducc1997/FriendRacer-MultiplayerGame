using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESRaceController : MonoBehaviour
{
    public List <Transform> AI_vehicles = new List<Transform>(1);
    public int Num_Of_Laps = 3;
    public ESAIRaceManager PlayerCar;
    public int currentlap;
    //
    private void Update()
    {
        RaceManager();
    }
    //
    private void RaceManager()
    {
        resetdamagevehicle();
        currentlap =  PlayerCar.currentlap;
    }
    //
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("AICar") || other.gameObject.CompareTag("Player"))
        {
            if (other.GetComponent<ESAIRaceManager>().currentlap < Num_Of_Laps)
            {

                if (other.GetComponent<ESAIRaceManager>().StartCount)
                {
                    other.GetComponent<ESAIRaceManager>().add_current_lap = true;
                }
            }
        }

    }
    //
    private void resetdamagevehicle()
    {
        if (AI_vehicles.Count > 0)
        {
            for (int i = 0; i < AI_vehicles.Count; i++)
            {
                if (AI_vehicles[i].GetComponent<ESAIRaceManager>() != null)
                {
                    ESAIRaceManager AImanager = AI_vehicles[i].GetComponent<ESAIRaceManager>();
                    if (AImanager.Reset)
                    {
                        //
                        AImanager.transform.position = AImanager.LastCheckpiont.position;
                        AImanager.transform.rotation = AImanager.LastCheckpiont.rotation;
                        AImanager.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
                        AImanager.triggered = false;
                        AImanager.Reset = false;
                        AImanager.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
                    }
                }
            }
        }
    }
  //
}
