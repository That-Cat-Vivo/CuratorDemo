using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DownAttack : MonoBehaviour
{
    public Rigidbody2D player;
    
    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            player.velocity = new Vector2(player.velocity.x, 15f);
            Destroy(collision.gameObject);
        }
    }
}
