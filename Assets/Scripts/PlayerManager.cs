using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    CharacterController characterController;
    Vector3 velocity, move;
    GameObject networkManager;
    public float gravity = -20f;
    public float mass = 3f;
    public GameObject playerCamera;
    public GameObject terminal;
    public Transform GFX;

    [Header("Visualizers")]
        [Range(0f, 1f)] public float interp;
        public float interpSpeed;

    [Header("Health and Attack related Vars")]
        [SyncVar] public int health = 3;
        [HideInInspector] public bool doAttack;
        [HideInInspector] public bool toRespawn;
        [SyncVar] public bool doubleTake;
        public Vector3 impact = Vector3.zero;
        public Vector3 attackerPos;
        private SphereCollider attackSphere;

    [Header("Powerup Effects")]
        public bool corruptus;
        public bool escren;

    [Header("Normal Movement-Related Vars")]
        public string directLR, directUD;
        [SerializeField] float horDist, verDist;

    [Header("Dash Related Vars")]
        public Vector3 dest;
        public string dashDir;
        public Vector3 dashOldPos;

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        attackSphere = GetComponent<SphereCollider>();
        networkManager = GameObject.Find("NetworkManager");
        SpawnOnStart(networkManager);                                   //The player spawns at one of the spawn points on start.
    }

    private void SpawnOnStart(GameObject manager)                       //Ditto. Uses the same code of respawning on spawnpoints upon death and running the "respawn" command.
    {
        var spawn = manager.GetComponent<GameManager>().spawns;
        int randomElement = Random.Range(0, spawn.Count);
        transform.position = spawn[randomElement].transform.position;
    }
    private void Update()
    {
        DirectionRotation();                                                                                            //Player rotates based on which direction they are going.
        ClientHealthDecay();                                                                                            //Health decays every 10 seconds if beyond 3.
        if (isLocalPlayer && doAttack == true)
        {
            doAttack = false;
            StartCoroutine(AttackDelay(0.3f));                                                                          //The player character takes 0.3s to attack after typing "attack".
            return;
        }

        if (!isLocalPlayer)
        {
            playerCamera.GetComponent<Camera>().enabled = false;
            playerCamera.GetComponent<AudioListener>().enabled = false;
            terminal.SetActive(false);
        }

        if(health <= 0 && toRespawn == true)
        {
            CmdRespawn();                                                                                                  //Adding the "Command" attribute to Update() wasn't a good idea.
            toRespawn = false;
        }

        if (impact.magnitude > 0.2f)
        {
            characterController.Move(impact * Time.deltaTime);
        }
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        #region Death Camera Close-Up
        if (health <= 0)                                                      //Zooms in the place where the player died.
        {
            playerCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCamera.GetComponent<Camera>().fieldOfView, 60f - 30f, 0.5f * Time.deltaTime);
        }
        else { playerCamera.GetComponent<Camera>().fieldOfView = 60f; }      //Zooms back to its original value.
        #endregion

        if (characterController.isGrounded == true && velocity.y < 0f) { velocity.y = -2f; }                                //Gravity.
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
                break;

            case "right":
                move = new Vector3(1f, 0f, move.z);
                break;
        }

        //Vertical Movement.
        switch(directUD)
        {
            case "up":
                move = new Vector3(move.x, 0f, 1f);
                break;

            case "down":
                move = new Vector3(move.x, 0f, -1f);
                break;
        }
        
        characterController.Move(move * 3f * Time.deltaTime);
        characterController.Move(velocity * Time.deltaTime);
    }

    [ClientRpc] public void RpcKnockBack(Vector3 dir, float force)
    {
        dir.Normalize();
        impact += dir.normalized * force / mass;
    }

    public void StopMovement()
    {
        directLR = null;
        directUD = null;
        move = Vector3.zero;
    }

    #region Attacking and Damage
    private IEnumerator AttackDelay(float t)
    {
        yield return new WaitForSeconds(t);
        attackSphere.enabled = true;
        yield return new WaitForSeconds(0.05f);
        attackSphere.enabled = false;
    }

    private void OnTriggerEnter(Collider other)                                                                             //This is used by the attackSphere collider by default.
    {
        if (other.GetComponent<NetworkIdentity>().isLocalPlayer == false)
        {
            CmdDoDamage(other.GetComponent<NetworkIdentity>().gameObject);
        }
    }

    [Command] public void CmdDoDamage(GameObject enemyGameObject)
    {
        enemyGameObject.GetComponent<PlayerManager>().RpcTakeDamage(1);                                                     //Victim takes damage.
        enemyGameObject.GetComponent<PlayerManager>().RpcKnockBack(enemyGameObject.transform.position - transform.position, 50f);
    }

    [Command] public void CmdSeppuku() => RpcTakeDamage(health);                                                            //Player committing suicide by running the "kill" command.
    
    [ClientRpc] public void RpcTakeDamage(int dmg)                                                                          //This RPC method handles taking damage and death.
    {
        health -= dmg;

        if (health <= 0)                             //DIE!
        {
            GFX.gameObject.SetActive(false);
            characterController.enabled = false;
        }
    }
    #endregion

    #region Respawning the player
    [Command] public void CmdRespawn() => RpcRespawn();

    [ClientRpc] public void RpcRespawn()            //And you're ALIVE!
    {
        health = 3;
        var spawn = networkManager.GetComponent<GameManager>().spawns;
        int randomElement = Random.Range(0, spawn.Count);                           //A random index will be chosen between 0 and the number of spawnpoints in the level.
        transform.position = spawn[randomElement].transform.position;               //The transform of the player will be equal to that of the random spawn point.
        GFX.rotation = Quaternion.Euler(0f, 0f, 0f);                                //The player's graphics object rotation will be reset on respawn.

        GFX.gameObject.SetActive(true);
        characterController.enabled = true;
    }
    #endregion

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

    public void ClientHealthDecay()
    {
        float timer = 10f;
        //Debug.Log(timer);
        if(health > 3)
        {
            if(timer > 0f) { timer -= Time.deltaTime; }
            if(timer <= 0f && health > 3)
            {
                health -= 1;
                timer = 10f;
            }
        }
    }

    #region Powerup Effects
    [Command] public void CmdGiveHealth(int healthPoints) => RpcTakeHealth(healthPoints);

    [ClientRpc]
    public void RpcTakeHealth(int healthPoints)
    {
        health += healthPoints;
    }
    #endregion
}