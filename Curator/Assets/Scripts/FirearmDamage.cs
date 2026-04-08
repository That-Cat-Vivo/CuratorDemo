using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmDamage : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            Destroy(collision.gameObject);
        }
    }
}
