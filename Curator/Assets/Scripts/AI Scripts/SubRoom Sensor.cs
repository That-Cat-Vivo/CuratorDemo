using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubRoomSensor : MonoBehaviour
{
    public bool PlayerPresent;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPresent = false;
    }


    //Detect when player enters the sensor (Set in-engine to only detect player)
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            PlayerPresent = true;
        }
    }
    //Detect when player exits the sensor
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9)
        {
            PlayerPresent = false;
        }
    }
}
