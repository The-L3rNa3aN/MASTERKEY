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
    public Transform GFX;

    [Header("Visualizers")]
    [Range(0f, 1f)] public float interp;
    public float interpSpeed;

    [Header("Health and Attack related Vars")]
    [SyncVar] public int health = 3;
    [SyncVar] public Vector3 knockBackVector;
    public bool isAttacked;
    public Vector3 attackerPos;
    public Vector3 knockBackFriction;

    [Header("Normal Movement-Related Vars")]
    public string directLR, directUD;
    public float unitsLR, unitsUD;
    Vector3 lastPos;
    [SerializeField] float horDist, verDist;

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
        DirectionRotation();                                                 //Player rotates based on which direction they are going.
        if (!isLocalPlayer)                                                  //If this script doesn't belong to the main client, don't run it.
        {
            terminal.SetActive(false);                                       //Prevents the terminal UI of other players to overlap with the local players.
            
            return;
        }

        if (isAttacked == true)                                               //Knockback.
        {
            //knockBackVector = (attackerPos - transform.position);
            //Debug.Log(knockBackVector);
            isAttacked = false;
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
        characterController.Move(knockBackVector * Time.deltaTime);             //FIX IT ASAP!!!
    }

    public void StopMovement()
    {
        directLR = null;
        directUD = null;
        unitsLR = 0f;
        unitsUD = 0f;
        move = Vector3.zero;
    }

    [Command(requiresAuthority = false)] public void Damage(int dmg) => health -= dmg;

    public void DirectionRotation()
    {
        switch(move.x, move.z)
        {
            case (0f, 1f):
                GFX.localRotation = Quaternion.Lerp(GFX.rotation, Quaternion.Euler(0f, 0f, 0f), (interp + interpSpeed) * Time.deltaTime);
                break;

            case (1f, 1f):
                GFX.localRotation = Quaternion.Lerp(GFX.rotation, Quaternion.Euler(0f, 45f, 0f), (interp + interpSpeed) * Time.deltaTime);
                break;

            case (1f, 0f):
                GFX.localRotation = Quaternion.Lerp(GFX.rotation, Quaternion.Euler(0f, 90f, 0f), (interp + interpSpeed) * Time.deltaTime);
                break;

            case (1f, -1f):
                GFX.localRotation = Quaternion.Lerp(GFX.rotation, Quaternion.Euler(0f, 135f, 0f), (interp + interpSpeed) * Time.deltaTime);
                break;

            case (0f, -1f):
                GFX.localRotation = Quaternion.Lerp(GFX.rotation, Quaternion.Euler(0f, 180f, 0f), (interp + interpSpeed) * Time.deltaTime);
                break;

            case (-1f, -1f):
                GFX.localRotation = Quaternion.Lerp(GFX.rotation, Quaternion.Euler(0f, 225f, 0f), (interp + interpSpeed) * Time.deltaTime);
                break;

            case (-1f, 0f):
                GFX.localRotation = Quaternion.Lerp(GFX.rotation, Quaternion.Euler(0f, 270f, 0f), (interp + interpSpeed) * Time.deltaTime);
                break;

            case (-1f, 1f):
                GFX.localRotation = Quaternion.Lerp(GFX.rotation, Quaternion.Euler(0f, 315f, 0f), (interp + interpSpeed) * Time.deltaTime);
                break;
        }
    }

    public float CosAngle()
    {
        float look = transform.eulerAngles.y;
        float move = Mathf.Atan2(velocity.x, velocity.z) * Mathf.Deg2Rad;

        float u = Mathf.DeltaAngle(look, move) * Mathf.Deg2Rad;
        float v = (90 - u) * Mathf.Deg2Rad;

        float angle = 2f * Mathf.Cos((u + v) / 2f) * Mathf.Cos((u - v) / 2f);
        return angle;
    }
}
