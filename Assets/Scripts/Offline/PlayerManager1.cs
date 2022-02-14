using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager1 : MonoBehaviour
{
    CharacterController characterController;
    Vector3 velocity, move;
    public float gravity = -20f;
    public GameObject playerCamera;

    [Header("Normal Movement-Related Vars")]
    public bool changeDir;
    public string directLR, directUD;
    public float unitsLR, unitsUD;
    public Vector3 dest;
    public Vector3 oldPos;

    [Header("Dash Related Vars")]
    public string dashDir;
    public Vector3 dashOldPos;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (characterController.isGrounded == true && velocity.y < 0f)
        {
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;

        if(move == Vector3.zero || changeDir == true)            //Stores the player's last known stationary position as long as they aren't moving.
        {
            oldPos = transform.position;
            changeDir = false;
        }

        if(dashDir == null) { dashOldPos = transform.position; }

        switch(dashDir)
        {
            case "left":
                changeDir = true;
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
                changeDir = true;
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
                changeDir = true;
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
                changeDir = true;
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

        switch(directLR)
        {
            case "left":
                Debug.Log("Test");
                move = new Vector3(-1f, 0f, move.z);
                dest = oldPos + new Vector3(-unitsLR, 0f, 0f);

                if (transform.position.x <= dest.x)
                {
                    transform.position = new Vector3(dest.x, transform.position.y, transform.position.z);
                    directLR = null;
                    move.x = 0f;
                }
                break;


            case "right":
                Debug.Log("Test");
                move = new Vector3(1f, 0f, move.z);
                dest = oldPos + new Vector3(unitsLR, 0f, 0f);

                if (transform.position.x >= dest.x)
                {
                    transform.position = new Vector3(dest.x, transform.position.y, transform.position.z);
                    directLR = null;
                    move.x = 0f;
                }
                break;
        }

        switch(directUD)
        {
            case "up":
                move = new Vector3(move.x, 0f, 1f);
                dest = oldPos + new Vector3(0f, 0f, unitsUD);

                if (transform.position.z >= dest.z)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, dest.z);
                    directUD = null;
                    move.z = 0f;
                }
                break;

            case "down":
                move = new Vector3(move.x, 0f, -1f);
                dest = oldPos + new Vector3(0f, 0f, -unitsUD);

                if (transform.position.z <= dest.z)
                {
                    transform.position = new Vector3(transform.position.x, transform.position.y, dest.z);
                    directUD = null;
                    move.z = 0f;
                }
                break;
        }
        characterController.Move(move * 3f * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
    }
}
