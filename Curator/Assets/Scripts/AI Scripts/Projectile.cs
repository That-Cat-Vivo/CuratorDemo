using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public GameObject player;
    private Vector2 playerPosition;
    private Vector2 projectilePosition;
    private Vector2 direction;
    public int speed;
    // Start is called before the first frame update
    void Awake()
    {
        player = GameObject.Find("Player Up To Date");
        playerPosition = player.transform.position;
        projectilePosition = transform.position;
        direction = (playerPosition - projectilePosition);
        direction = direction.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        projectilePosition = transform.position;
        projectilePosition += direction * speed * Time.deltaTime;
        transform.position = projectilePosition;    
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        Destroy(gameObject);
    }
}
