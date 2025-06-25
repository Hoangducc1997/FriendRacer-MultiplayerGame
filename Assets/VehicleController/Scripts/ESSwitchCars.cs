using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESSwitchCars : MonoBehaviour
{
    public ESThirdPersonCamera360 cam;
    public Transform car0, car1, car2, car3, car4;
    public int defualtint = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            defualtint = 0;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            defualtint = 1;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            defualtint =2;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            defualtint = 3;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            defualtint = 4;
        }
      
        //


        if (defualtint == 0)
        {
            cam.Target = car0.gameObject;
            shufflecompnents(true, car0);
            shufflecompnents(false, car1);
            shufflecompnents(false, car2);
            shufflecompnents(false, car3);
            shufflecompnents(false, car4);
           
        }
        else if (defualtint == 1)
        {
            cam.Target = car1.gameObject;
            shufflecompnents(false, car0);
            shufflecompnents(true, car1);
            shufflecompnents(false, car2);
            shufflecompnents(false, car3);
            shufflecompnents(false, car4);
          
        }
        else if (defualtint == 2)
        {
            cam.Target = car2.gameObject;
            shufflecompnents(false, car0);
            shufflecompnents(false , car1);
            shufflecompnents(true, car2);
            shufflecompnents(false, car3);
            shufflecompnents(false, car4);
          
        }
        else if (defualtint == 3)
        {
            cam.Target = car3.gameObject;
            shufflecompnents(false, car0);
            shufflecompnents(false, car1);
            shufflecompnents(false, car2);
            shufflecompnents(true , car3);
            shufflecompnents(false, car4);
           
        }
        else if (defualtint == 4)
        {
            cam.Target = car4.gameObject;
            shufflecompnents(false, car0);
            shufflecompnents(false, car1);
            shufflecompnents(false, car2);
            shufflecompnents(false, car3);
            shufflecompnents(true , car4);
        
        }
       
    }
    //
    void shufflecompnents(bool _bool, Transform car)
    {
        car.root.GetComponent<AudioSource>().enabled = _bool;
        car.root.GetComponent<ESGearShift>().enabled = _bool;
        car.root.GetComponent<ESAudioSystem>().enabled = _bool;
        car.root.GetComponent<ESVehicleController>().enabled = _bool;
         car.root.GetComponent<ESVehicleController>().CarRb.isKinematic = _bool == true ? false : true;
    }
}
