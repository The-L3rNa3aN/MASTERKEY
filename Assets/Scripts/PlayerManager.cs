using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    CharacterController characterController;
    Vector3 velocity, move;
    public float gravity = -20f;
    public GameObject playerCamera;
    public GameObject terminal;

    Vector3 lastPos;
    [SerializeField] float horDist, verDist;

    [Header("Health and Attack related Vars")]
    public int health = 3;

    [Header("Normal Movement-Related Vars")]
    public string directLR, directUD;
    public float unitsLR, unitsUD;
    
    [Header("Dash Related Vars")]
    public Vector3 dest;
    public string dashDir;
    public Vector3 dashOldPos;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        lastPos = transform.position;

        if (!isLocalPlayer) { playerCamera.SetActive(false); }
    }

    private void Update()
    {
        if (!isLocalPlayer)                                                  //If this script doesn't belong to the main client, don't run it.
        {
            terminal.SetActive(false);                                       //Prevents the terminal UI of other players to overlap with the local players.
            
            return;
        }

        if (characterController.isGrounded == true && velocity.y < 0f)       //Gravity.
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        if(dashDir == null) { dashOldPos = transform.position; }

        //Dashing.
        switch(dashDir)
        {
            case "left":
                move = Vector3.left * 7.5f;
                dest = dashOldPos + new Vector3(-7.5f, 0f, 0f);

                if (transform.position.x <= dest.x)
                {
                    transform.position = new Vector3(dest.x, transform.position.y, transform.position.z);
                    dashDir = null;
                    move.x = 0f;
                }
                break;

            case "right":
                move = Vector3.right * 7.5f;
                dest = dashOldPos + new Vector3(7.5f, 0f, 0f);

                if (transform.position.x >= dest.x)
                {
                    transform.position = new Vector3(dest.x, transform.position.y, transform.position.z);
                    dashDir = null;
                    move.x = 0f;
                }
                break;

            case "up":
                move = Vector3.forward * 7.5f;
                dest = dashOldPos + new Vector3(0f, 0f, 7.5f);

                if (transform.position.z >= dest.z)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, dest.z);
                    dashDir = null;
                    move.z = 0f;
                }
                break;

            case "down":
                move = Vector3.back * 7.5f;
                dest = dashOldPos + new Vector3(0f, 0f, -7.5f);

                if (transform.position.z <= dest.z)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, dest.z);
                    dashDir = null;
                    move.z = 0f;
                }
                break;
        }

        //Horizontal Movement.
        switch(directLR)
        {
            case "left":
                move = new Vector3(-1f, 0f, move.z);
                horDist += Vector3.Distance(lastPos, transform.position);
                lastPos = transform.position;

                if (horDist >= unitsLR)
                {
                    move.x = 0f;
                    horDist = 0f;
                    directLR = null;
                }
                break;


            case "right":
                move = new Vector3(1f, 0f, move.z);
                horDist += Vector3.Distance(lastPos, transform.position);
                lastPos = transform.position;

                if (horDist >= unitsLR)
                {
                    move.x = 0f;
                    horDist = 0f;
                    directLR = null;
                }
                break;
        }

        //Vertical Movement.
        switch(directUD)
        {
            case "up":
                move = new Vector3(move.x, 0f, 1f);
                verDist += Vector3.Distance(lastPos, transform.position);
                lastPos = transform.position;

                if (verDist >= unitsUD)
                {
                    move.z = 0f;
                    verDist = 0f;
                    directUD = null;
                }
                break;

            case "down":
                move = new Vector3(move.x, 0f, -1f);
                verDist += Vector3.Distance(lastPos, transform.position);
                lastPos = transform.position;

                if (verDist >= unitsUD)
                {
                    move.z = 0f;
                    verDist = 0f;
                    directUD = null;
                }
                break;
        }

        characterController.Move(move * 3f * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
    }

    public void StopMovement()
    {
        directLR = null;
        directUD = null;
        unitsLR = 0f;
        unitsUD = 0f;
        move = Vector3.zero;
    }
}
