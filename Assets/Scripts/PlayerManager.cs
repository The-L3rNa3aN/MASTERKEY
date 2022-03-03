using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    CharacterController characterController;
    public Vector3 velocity, move;
    [SerializeField] GameObject networkManager;
    [HideInInspector] public float gravity = -20f;
    [HideInInspector] public float mass = 3f;
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
        public Vector3 impact = Vector3.zero;
        public Vector3 attackerPos;
        private SphereCollider attackSphere;
        float healthTimer = 5f;

    [Header("Powerup Effects")]
        [SyncVar] public bool corruptus;
        [SerializeField] float corruptusTimer = 20f;
        [SyncVar] public bool escren;

    [Header("Normal Movement-Related Vars")]
        public string directLR;
        public string directUD;

    [Header("Dash Related Vars")]
        public Vector3 dest;
        public string dashDir;
        public Vector3 dashOldPos;
        [SerializeField] private float dashTimer = 1f;

    public List<GameObject> spawnPoints = new List<GameObject>();                           //A list of spawnpoints for the player. I hope this doesn't hinder performance.

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        attackSphere = GetComponent<SphereCollider>();
        networkManager = GameObject.Find("NetworkManager");

        var spObjects = GameObject.FindGameObjectsWithTag("Spawn Point");                   //All spawnpoints are found at the start and are added to the list.
        for(int i = 0; i < spObjects.Length; i++)
        {
            spawnPoints.Add(spObjects[i]);
        }

        var plyrs = GameObject.FindGameObjectsWithTag("Player");                            //Finds players in the server and broadcasts a message on joining.
        for (int i = 0; i < plyrs.Length; i++)
        {
            plyrs[i].GetComponentInChildren<PlayTerminalManager>().PlayerJoined(PlayerPrefs.GetString("PlayerName"));
        }

        //Randomized spawnpoints on start.
        networkManager.GetComponent<NetworkManager>().playerPrefab.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;
    }

    private void Update()
    {
        DirectionRotation();                                                                                            //Player rotates based on which direction they are going.
        ClientHealthDecay();                                                                                            //Health decays every 10 seconds if beyond 3.
        CorruptusTimer();                                                                                               //20-second timer activated if the player picks up Corruptus.
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

        switch(dashDir)                                                                                                     //Dashing.
        {
            case "left":
                move = Vector3.left * 7.5f;
                dashTimer -= Time.deltaTime;
                if(dashTimer <= 0f)
                {
                    dashDir = null;
                    move = Vector3.zero;
                    dashTimer = 1f;
                }
                break;

            case "right":
                move = Vector3.right * 7.5f;
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f)
                {
                    dashDir = null;
                    move = Vector3.zero;
                    dashTimer = 1f;
                }
                break;

            case "up":
                move = Vector3.forward * 7.5f;
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f)
                {
                    dashDir = null;
                    move = Vector3.zero;
                    dashTimer = 1f;
                }
                break;

            case "down":
                move = Vector3.back * 7.5f;
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f)
                {
                    dashDir = null;
                    move = Vector3.zero;
                    dashTimer = 1f;
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

    public struct PlayerData : NetworkMessage
    {
        public string playerName;
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
        if (other.GetComponent<NetworkIdentity>().isLocalPlayer == false) CmdDoDamage(other.GetComponent<NetworkIdentity>().gameObject);
    }

    [Command] public void CmdDoDamage(GameObject enemyGameObject)
    {
        if (corruptus == true) { enemyGameObject.GetComponent<PlayerManager>().RpcTakeDamage(2); }                          //Victim takes twice the damage because of the player's Corruptus.
        else if (corruptus == false) { enemyGameObject.GetComponent<PlayerManager>().RpcTakeDamage(1); }

        if(enemyGameObject.GetComponent<PlayerManager>().escren == false)                                                   //Victims under the Escren effect don't suffer knockback.
        {
            enemyGameObject.GetComponent<PlayerManager>().RpcKnockBack(enemyGameObject.transform.position - transform.position, 50f);
        }
    }

    [Command] public void CmdDoSelfDamage(int helth) => RpcTakeDamage(helth);

    [Command] public void CmdSeppuku() => RpcTakeDamage(health);                                                  //Player committing suicide by running the "kill" command.
    
    [ClientRpc] public void RpcTakeDamage(int dmg)                                                                //This RPC method handles taking damage and death.
    {
        if (corruptus == true)                          //Players under the Corruptus effect take more damage then usual.
        {
            if (escren == true) escren = false;         //Players under the Escren effect don't take damage. The effect disppears after taking any form of damage.
            else health -= dmg * 2;
        }
        else if(corruptus == false)
        {
            if (escren == true) escren = false;
            else health -= dmg;
        }

        if (health <= 0)                                        //DIE!
        {
            GFX.gameObject.SetActive(false);
            characterController.enabled = false;
            StopMovement();
        }
    }
    #endregion

    #region Player Respawning
    [Command] public void CmdRespawn() => RpcRespawn();

    [ClientRpc] public void RpcRespawn()                        //And you're ALIVE!
    {
        health = 3;
        transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;                            //New position on respawning.
        GFX.rotation = Quaternion.Euler(0f, 0f, 0f);                                                                        //The player's graphics object rotation will be reset on respawn.
        corruptus = false;                                                                                                  //Any powerup effects on the player will disappear on death.
        escren = false;
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
        if (health > 3)
        {
            if (healthTimer > 0f) { healthTimer -= Time.deltaTime; }
            if (healthTimer <= 0f && health > 3)
            {
                healthTimer = 5f;
                CmdDoSelfDamage(1);
            }
        }
    }

    #region Powerup Effects
    [Command] public void CmdGiveHealth(int healthPoints) => RpcTakeHealth(healthPoints);
    [ClientRpc]public void RpcTakeHealth(int healthPoints) => health += healthPoints;

    public void CorruptusTimer()
    {
        if(corruptus == true)
        {
            if(corruptusTimer > 0f) { corruptusTimer -= Time.deltaTime; }
            if(corruptusTimer <= 0f)
            {
                corruptusTimer = 20f;
                corruptus = false;
            }
        }
    }
    #endregion

    /*private void OnConnectedToServer()
    {
        PlayerData joinMsg = new PlayerData()
        {
            playerName = networkManager.GetComponent<GameManager>().playerName
        };
        NetworkServer.SendToAll(joinMsg);
        NetworkClient.RegisterHandler<PlayerData>(Function3);
    }

    void Function3(PlayerData msg)
    {
        Debug.Log(msg.playerName);
    }*/
}