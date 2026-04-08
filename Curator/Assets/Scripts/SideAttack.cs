using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideAttack : MonoBehaviour
{
    public Rigidbody2D player;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 12)
        {
            player.velocity = new Vector2(20f, player.velocity.y + 5f);
            Destroy(collision.gameObject);
        }
    }
}
