using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeSensor : MonoBehaviour
{
    public bool inRange;

    private void Awake()
    {
        inRange = false;
    }
    //Detect when player enters the sensor
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9) inRange = true;

    }
    //Detect when player exits the sensor

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 9) inRange = false;
    }
}
