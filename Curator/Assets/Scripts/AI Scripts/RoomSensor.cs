using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSensor : MonoBehaviour
{
    public bool playerPresent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    //Detect when player enters the sensor (Set in-engine to only detect player)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        playerPresent = true;
    }
    //Detect when player exits the sensor
    private void OnTriggerExit2D(Collider2D collision)
    {
        playerPresent = false;
    }
}
