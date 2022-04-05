using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerManager : NetworkBehaviour
{
    [SyncVar] public bool matchOver = false;
    public Vector3 velocity, move;
    public GameObject playerCamera;
    public GameObject terminal;
    public Transform GFX;
    private CharacterController characterController;
    private GameObject networkManager;
    private float gravity = -20f;
    private float mass = 3f;

    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"red", "#ff0000" },
        {"light blue", "#7070ff" },
        {"green", "#00ff00" },
        {"orange", "ffab0f" },
        {"yellow", "#ffea00" },
        {"aqua", "#00f7ff" },
        {"white", "#ffffff" }
    };

    [Header("Player Stats and Scores")]
    [SyncVar] public string playerTag;
    [SyncVar] public int kills;
    [SyncVar] public int deaths;
    [SyncVar] public int killsNoDeath;
    [SyncVar] public int spreesStarted;
    [SyncVar] public int spreesEnded;
    [SyncVar] public int fragLimit;
    [SyncVar] public float timeLimit;
    private int oldKills;
    private int oldDeaths;
    private int oldSpreesStarted;
    private int oldSpreesEnded;

    [Header("Visualizers")]
    [SyncVar] public Vector4 circleVis;
    [SyncVar] public bool enableCircle;
    [Range(0f, 1f)] public float interp;
    public float interpSpeed;
    public SpriteRenderer attackCircle;
    public RawImage hurtImage;
    public RawImage deathImage;

    [Header("Health and Attack related Vars")]
    [SyncVar] public int health = 3;
    public Vector3 impact = Vector3.zero;
    public Vector3 attackerPos;
    private SphereCollider attackSphere;
    private float healthTimer = 5f;
    [SyncVar] public bool doAttack;
    [HideInInspector] public bool toRespawn;

    [Header("Powerup Effects")]
    [SyncVar] public bool corruptus;
    [SyncVar] public bool escren;
    private float corruptusTimer = 20f;

    [Header("Normal Movement-Related Vars")]
    public string directLR;
    public string directUD;

    [Header("Dash Related Vars")]
    public string dashDir;
    public Vector3 dashOldPos;
    [SerializeField] private float dashTimer = 1f;

    public List<GameObject> spawnPoints = new List<GameObject>();                           //A list of spawnpoints for the player. I hope this doesn't hinder performance.

    private void Start()
    {
        if(isServer)                                                                        //Assigns the limits depending on the player being the host or the client.
        {
            fragLimit = PlayerPrefs.GetInt("FragLimit");
            timeLimit = PlayerPrefs.GetInt("TimeLimit");
        }
        else
        {
            var networkPlayers = FindObjectsOfType<NetworkIdentity>();
            foreach(NetworkIdentity player in networkPlayers)
            {
                if(player.isServer)
                {
                    player.GetComponent<PlayerManager>().fragLimit = fragLimit;
                    player.GetComponent<PlayerManager>().timeLimit = timeLimit;
                }
            }
        }

        oldKills = PlayerPrefs.GetInt("PlayerKills");
        oldDeaths = PlayerPrefs.GetInt("PlayerDeaths");
        oldSpreesStarted = PlayerPrefs.GetInt("PlayerSpreesStarted");
        oldSpreesEnded = PlayerPrefs.GetInt("PlayerSpreesEnded");

        characterController = GetComponent<CharacterController>();
        attackSphere = GetComponent<SphereCollider>();
        networkManager = GameObject.Find("NetworkManager");
        CmdSetName(PlayerPrefs.GetString("PlayerName"));                                    //Sets the name of the player. It is trying to send a command to an object of no authority. WHY?

        var spObjects = GameObject.FindGameObjectsWithTag("Spawn Point");                   //All spawnpoints are found at the start and are added to the list.
        for (int i = 0; i < spObjects.Length; i++) { spawnPoints.Add(spObjects[i]); }

        //Randomized spawnpoints on start.
        networkManager.GetComponent<NetworkManager>().playerPrefab.transform.position = spawnPoints[Random.Range(0, spawnPoints.Count)].transform.position;

        playerCamera = FindObjectOfType<Camera>().gameObject;                               //The camera in the scene will be assigned to the player upon connecting.
    }

    private void Update()
    {
        #region Death Camera Close-Up
        if (isLocalPlayer && health <= 0)                                           //Zooms in the place where the player died and zooms back when respawned. Zoom effect will sync with all clients if not using bool.
        {
            playerCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCamera.GetComponent<Camera>().fieldOfView, 30f, 0.5f * Time.deltaTime);
            deathImage.color = Color.Lerp(deathImage.color, new Color(1, 1, 1, 1), Time.deltaTime);
        }
        else if(isLocalPlayer && health > 0)
        {
            playerCamera.GetComponent<Camera>().fieldOfView = Mathf.Lerp(playerCamera.GetComponent<Camera>().fieldOfView, 60f, 3f * Time.deltaTime);
            deathImage.color = Color.Lerp(deathImage.color, new Color(1, 1, 1, 0), 5f * Time.deltaTime);
        }
        #endregion

        #region Hurt-Image Fade Out
        if (hurtImage.color.a > 0f) { hurtImage.color = Color.Lerp(hurtImage.color, new Color(1, 1, 1, 0), 2f * Time.deltaTime); }
        #endregion

        CircleVisualizer();
        DirectionRotation();                                                                                            //Player rotates based on which direction they are going.
        ClientHealthDecay();                                                                                            //Health decays every 10 seconds if beyond 3.
        CorruptusTimer();                                                                                               //20-second timer activated if the player picks up Corruptus.

        if(fragLimit != 0 && kills >= fragLimit)
        {
            CmdLimitReached();
        }

        /*if(timeLimit != 0f)
        {
            timeLimit -= Time.deltaTime;
        }*/

        if (!isLocalPlayer) {terminal.SetActive(false);}
        if (isLocalPlayer)
        {
            playerCamera.GetComponent<CameraFollowScript>().target = transform;             //The unsynced camera will only follow the player of this machine.
        }

        if (health <= 0 && toRespawn == true)
        {
            CmdRespawn();                                                                                                  //Adding the "Command" attribute to Update() wasn't a good idea.
            toRespawn = false;
        }

        if (impact.magnitude > 0.2f) { characterController.Move(impact * Time.deltaTime); }                                 //Knockback running in Update (duh) and decays with time thanks to Lerp.
        impact = Vector3.Lerp(impact, Vector3.zero, 5 * Time.deltaTime);

        if (characterController.isGrounded == true && velocity.y < 0f) { velocity.y = -2f; }                                //Gravity.
        velocity.y += gravity * Time.deltaTime;

        if (dashDir == null) { dashOldPos = transform.position; }

        //Dashing.
        switch (dashDir)
        {
            case "left":
                move = Vector3.left * 7.5f;
                dashTimer -= Time.deltaTime;
                if (dashTimer <= 0f)
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
        switch (directLR)
        {
            case "left":
                move = new Vector3(-1f, 0f, move.z);
                break;

            case "right":
                move = new Vector3(1f, 0f, move.z);
                break;
        }

        //Vertical Movement.
        switch (directUD)
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

    #region Match Over
    [Command] public void CmdLimitReached()
    {
        var players = FindObjectsOfType<PlayerManager>();
        int highestKills = players.Max<PlayerManager>().kills;

        foreach (PlayerManager pManager in players)
        {
            pManager.StopMovement();
            pManager.RpcLimitReached();

            if (pManager.kills == highestKills)     ///Returns an error and disconnects the client when they reach the score limit.
            {
                NetworkServer.SendToAll(new Notification { content = "Champion of the match: " + ColorString(pManager.playerTag, colors["yellow"]) + " with " + ColorString(highestKills.ToString(), colors["yellow"]) + " kills." });
            }
        }
    }

    [Command] public void CmdHostEndedMatch()
    {
        NetworkServer.SendToAll(new Notification { content = ColorString(playerTag, colors["yellow"]) + " ended the match." });
        var players = FindObjectsOfType<PlayerManager>();
        int highestKills = players.Max<PlayerManager>().kills;

        foreach (PlayerManager pManager in players)
        {
            pManager.StopMovement();
            pManager.RpcLimitReached();

            if(pManager.kills == highestKills)
            {
                NetworkServer.SendToAll(new Notification { content = "Champion of the match: " + ColorString(pManager.playerTag, colors["yellow"]) + " with " + ColorString(highestKills.ToString(), colors["yellow"]) + " kills." });
            }
        }
    }

    [ClientRpc] public void RpcLimitReached() => matchOver = true;
    #endregion

    #region Name Set Up
    [Command] public void CmdSetName(string name)
    {
        RpcSetName(name);
        NetworkServer.SendToAll(new Notification { content = ColorString(name, colors["yellow"]) + " has joined." });       //Running this in Start() prevents the client from joining the server.
    }

    [ClientRpc] public void RpcSetName(string name) => playerTag = name;
    #endregion

    #region Attacking and Damage
    [Command] public void CmdDoDamage(GameObject enemyGameObject)
    {
        var enemy = enemyGameObject.GetComponent<PlayerManager>();

        if (enemy.health <= 1)           //Because the info before striking is different from the final reduced health of the enemy / enemies.
        {
            kills++;
            enemy.deaths++;
            killsNoDeath++;
            //RpcVersusHandler(enemy);

            if(enemy.killsNoDeath >= 5)
            {
                NetworkServer.SendToAll(new Notification { content = ColorString(playerTag, colors["yellow"]) + " has ended " + ColorString(enemy.playerTag + "'s", colors["yellow"]) + ColorString(" killing spree", colors["red"]) });
            }
        }

        if (killsNoDeath == 5)          //Killing spree notification.
        {
            spreesStarted++;
            NetworkServer.SendToAll(new Notification { content = ColorString(playerTag, colors["yellow"]) + " is on a " + ColorString("KILLING SPREE", colors["red"]) });
        }

        if (corruptus == true) { enemy.RpcTakeDamage(2, playerTag); }                                                       //Victim takes twice the damage because of the player's Corruptus.
        else if (corruptus == false) { enemy.RpcTakeDamage(1, playerTag); }

        if (enemyGameObject.GetComponent<PlayerManager>().escren == false)                                                   //Victims under the Escren effect don't suffer knockback.
        {
            enemyGameObject.GetComponent<PlayerManager>().RpcKnockBack(enemyGameObject.transform.position - transform.position, 50f);
        }
    }

    [Command] public void CmdDoSelfDamage(int helth) => RpcTakeDamage(helth, string.Empty);

    [Command] public void CmdEnableCircle() => enableCircle = true;

    [Command] public void CmdSeppuku()                                                                                      //Player committing suicide by running the "kill" command.
    {
        RpcTakeDamage(health, string.Empty);

        if(killsNoDeath >= 5)                           //If the player committed suicide when on a killing spree.
        {
            NetworkServer.SendToAll(new Notification { content = ColorString(playerTag, colors["yellow"]) + " has ended their " + ColorString("killing spree", colors["red"]) });
        }

        kills--;                                        //Because suicide sucks and the easy way out isn't always the best way.
        deaths++;                                       //You committed suicide like a coward, so that counts as dying.
        killsNoDeath = 0;                               //Yeah, you ended your own spree.
    }

    [ClientRpc] public void RpcTakeDamage(int dmg, string attackerTag)                                                       //This RPC method handles taking damage and death.
    {
        hurtImage.color = new Color(1, 1, 1, 1);
        if (corruptus == true)                          //Players under the Corruptus effect take more damage then usual.
        {
            if (escren == true) escren = false;         //Players under the Escren effect don't take damage. The effect disppears after taking any form of damage.
            else health -= dmg * 2;
        }
        else if (corruptus == false)
        {
            if (escren == true) escren = false;
            else health -= dmg;
        }

        if (health <= 0)                                        //DIE!
        {
            //Prevents the notification from being sent if the player commits suicide.
            if (attackerTag != string.Empty) { NetworkServer.SendToAll(new Notification { content = ColorString(attackerTag, colors["yellow"]) + " has slain " + ColorString(playerTag, colors["yellow"]) }); }
            else { NetworkServer.SendToAll(new Notification { content = ColorString(playerTag, colors["yellow"]) + " committed suicide." }); }

            killsNoDeath = 0;
            GFX.gameObject.SetActive(false);
            characterController.enabled = false;
            StopMovement();
        }
    }

    public void RpcVersusHandler(PlayerManager enemyPlayerManager)
    {
        foreach (var item in GetComponent<VersusPlayerScript>().versusKills)
        {
            if (item.Key == enemyPlayerManager)
            {
                GetComponent<VersusPlayerScript>().versusKills[item.Key]++;
            }
        }
    }

    public void CircleVisualizer()
    {
        if (doAttack == true)
        {
            doAttack = false;
            StartCoroutine(AttackDelay(0.3f));                                                                          //The player character takes 0.3s to attack after typing "attack".
            return;
        }

        if (enableCircle == true)
        {
            circleVis = Vector4.Lerp(circleVis, new Vector4(1, 1, 1, 1), 3f * Time.deltaTime);
            attackCircle.color = circleVis;

            if (attackCircle.color.a >= 0.6f)
            {
                enableCircle = false;
            }
        }

        if (enableCircle == false)
        {
            circleVis = Vector4.Lerp(circleVis, new Vector4(1, 1, 1, 0), 6f * Time.deltaTime);
            attackCircle.color = circleVis;
        }
    }

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
    #endregion

    #region Player Respawning
    [Command] public void CmdRespawn() => RpcRespawn();

    [ClientRpc]
    public void RpcRespawn()                        //And you're ALIVE!
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

    #region Powerup Effects
    [Command] public void CmdGiveHealth(int healthPoints) => RpcTakeHealth(healthPoints);
    [ClientRpc]
    public void RpcTakeHealth(int healthPoints)
    {
        health += healthPoints;
        if (healthPoints == 1) terminal.GetComponent<PlayTerminalManager>().AidePickup();
        else terminal.GetComponent<PlayTerminalManager>().VitalisPickup();
    }

    public void CorruptusTimer()
    {
        if (corruptus == true)
        {
            terminal.GetComponent<PlayTerminalManager>().CorruptusPickup();
            if (corruptusTimer > 0f) { corruptusTimer -= Time.deltaTime; }
            if (corruptusTimer <= 0f)
            {
                corruptusTimer = 20f;
                corruptus = false;
            }
        }
    }
    #endregion

    [Command] public void DisconnectAsClient()                                                                              //Called when disconnecting either as host or as client.
    {
        PlayerPrefs.SetInt("PlayerKills", oldKills + kills);
        PlayerPrefs.SetInt("PlayerDeaths", oldDeaths + deaths);
        PlayerPrefs.SetInt("PlayerSpreesStarted", oldSpreesStarted + spreesStarted);
        PlayerPrefs.SetInt("PlayerSpreesEnded", oldSpreesEnded + spreesEnded);
        PlayerPrefs.Save();
        NetworkServer.SendToAll(new Notification { content = ColorString(playerTag, colors["yellow"]) + " has left." });
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

    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";

        return leftTag + s + rightTag;
    }
}