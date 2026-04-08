using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class RangerFSM : MonoBehaviour
{
    //Node that dictates the idle position for the NPC
    public NavNode nodes;
    //Room and subroom sensors, detect the presence of the player
    public RoomSensor room;
    public SubRoomSensor subRoom;
    //Reference the player
    public GameObject player;
    //Melee sensor checks if player is within range to attack
    public MeleeSensor meleeRadius;
    //Left and right boundaries say how far the NPC can travel
    public GameObject boundaryL;
    public GameObject boundaryR;
    //reference to projectile prefab
    public GameObject projectile;
    //attack hurtbox reference
    public Collider2D attack;
    public enum State { idle, maneuver, rangedAttack, meleeAttack, reloading, dead}
    State state;

    private int rangeTimer;
    private int reloadTimer;
    private int meleeTimer;
    private int healthPoints;
    public float speed;
    private Vector2 NPCLocation;
    private Vector2 playerLocation;
    private Vector2 idleSpot;

    private void Awake()
    {
        state = State.idle;
        rangeTimer = 70;
        reloadTimer = 100;
        meleeTimer = 100;
        healthPoints = 5;
        idleSpot = new Vector2(nodes.transform.position.x, NPCLocation.y);
    }

    public void FixedUpdate()
    {
        //Update information on the position of the NPC, player, and idle nod.
        NPCLocation = transform.position;
        playerLocation = new Vector2(player.transform.position.x, NPCLocation.y);
        //Idle node y is consistantly updated to prevent NPC attempting to traverse the y axis in game.
        idleSpot = new Vector2(nodes.transform.position.x, NPCLocation.y);

        //States
        switch (state)
        {
            case State.idle:
                Idle();
                break;

            case State.maneuver:
                Maneuver();
                break;

            case State.rangedAttack:
                RangedAttack();
                break;

            case State.meleeAttack:
                MeleeAttack();
                break;

            case State.reloading:
                Reloading();
                break;

            case State.dead:
                Dead();
                break;

        }
        /*
        if (healthPoints <= 0)
        {
            state = State.dead;
        }
        */
    }

    private void Idle()
    {
        //NPC stay still.
        //If the NPC is outside of its idle position, it moves to it.
        if (NPCLocation.x <= idleSpot.x - 0.1f || NPCLocation.x >= idleSpot.x + 0.1f) transform.position = Vector2.MoveTowards(NPCLocation, idleSpot, speed);
        //When the subroom sensor detects the player, enter maneuver state.
        if (subRoom.PlayerPresent == true)
        {
            state = State.maneuver;
        }
    }
    private void Maneuver()
    {
        //Reset state timers
        reloadTimer = 150;
        meleeTimer = 50;
        //Disable attack hurtbox
        attack.enabled = false;

        //If the NPC is within its set boundaries, it moves towards the player.
        if (NPCLocation.x <= boundaryR.transform.position.x && NPCLocation.x >= boundaryL.transform.position.x)
        {
            transform.position = Vector2.MoveTowards(NPCLocation, playerLocation, speed);
        }
        //If the player is within the NPCs set boundaries, it moves towards the player.
        else if (playerLocation.x <= boundaryR.transform.position.x && playerLocation.x >= boundaryL.transform.position.x)
        {
            transform.position = Vector2.MoveTowards(NPCLocation, playerLocation, speed);
        }
        //If the player exited the room sensor, return to idle
        if (room.playerPresent == false)
        {
            state = State.idle;
        }
        //When timer reaches 0, enter ranged attack state
        rangeTimer -= 1;
        if (rangeTimer <= 0)
        {
            state = State.rangedAttack;
        }
        //When player is within melee range, enter melee attack state
        if (meleeRadius.inRange == true)
        {
            state = State.meleeAttack;
        }
    }
    private void RangedAttack()
    {
        //Reset range timer
        rangeTimer = 100;
        //Fire projectile
        GameObject.Instantiate(projectile, transform.position, transform.rotation);
        //Enter releoading state
        state = State.reloading;
    }
    private void MeleeAttack()
    {
        //Enable attack hurtbox
        attack.enabled = true;
        //When melee timer reaches 0, return to maneuver state
        meleeTimer -= 1;
        if (meleeTimer <= 0)
        {
            state = State.maneuver;
        }
    }
    private void Reloading()
    {
        //Stay still. When reload timer reaches 0, return to maneuver state
        reloadTimer -= 1;
        if (reloadTimer <= 0)
        {
            state = State.maneuver;
        }
    }
    private void Dead()
    {
        Invoke("Destruction", 300);
    }

    private void Destruction()
    {
        Destroy(gameObject);
    }
}
