using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayInterpreter : MonoBehaviour
{
    PlayTerminalManager terminalManager;
    [SerializeField] GameObject networkManager;
    public PlayerManager player;
    public ScoreBoard scoreboard;
    public float dashTimer = 0f;
    public int commandsRun;
    public int errors;

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

    List<string> response = new List<string>();

    private void Start()
    {
        terminalManager = GetComponent<PlayTerminalManager>();
        networkManager = GameObject.Find("NetworkManager");
    }

    private void Update()
    {
        if (dashTimer > 0f) { dashTimer -= Time.deltaTime; }
        if (dashTimer <= 0f) { dashTimer = 0f; }
    }

    public List<string> Interpret(string userInput)
    {
        response.Clear();
        string[] args = userInput.Split();

        if (args[0] == "help")
        {
            response.Add("Here is a list of commands you can use." + ColorString("CAUTION", colors["red"]) + ": " + "Commands are case sensitive!");
            ListEntry("match end", "Host command that immediately ends the match and announces the winner based on the highest kill count.");
            ListEntry("match start", "Host command that starts a match on the same map after it has gotten over.");
            ListEntry("move <direction>", "To move the character around. Directions include: up, down, left and right.");
            ListEntry("stop <param>", "Stops all movement in any direction. Parameters include: lateral, medial and all.");
            ListEntry("dash <direction>", "Performs a high speed, short ranged dash. Refreshes every 10 seconds.");
            ListEntry("attack", "Perform a radial attack that deals damage to anyone in your vicinity. Stops movement.");
            ListEntry("kill", "Commit seppuku and receive a penalty for bringing honor to your family.");
            ListEntry("respawn", "Respawn if you died during combat.");
            ListEntry("scoreboard", "Toggle a scoreboard that displays the scores of all players in the server. IT ONLY UPDATES WHEN TOGGLING IT ON.");
            ListEntry("clear", "Clears the terminal screen.");
            ListEntry("ipaddress", "Prints your IP Address and your status as the host or the client.");
            ListEntry("disconnect", "Disconnects and returns to the main terminal screen. This saves your stats.");
            ListEntry("quit / exit", "Immediately exits the game. This also saves your stats.");
            commandsRun++;
            return response;
        }

        #region Powerup Pick Up Responses
        if(userInput == "aide")
        {
            response.Add("You've picked up Aide.");
            return response;
        }

        if (userInput == "vitalis")
        {
            response.Add("You've picked up Vitalis.");
            return response;
        }

        if (userInput == "corruptus")
        {
            response.Add("You've picked up Corruptus.");
            return response;
        }
        #endregion

        #region On Connect Responses
        if (userInput == "isClient" && networkManager != null)
        {
            response.Add("Successfully connected to " + ColorString(networkManager.GetComponent<NetworkManager>().networkAddress, colors["yellow"]) + " as a client.");
            return response;
        }
        
        if(userInput == "isHost" && networkManager != null)             //This doesn't work for some reason, and I dunno why.
        {
            Debug.Log("test");
            response.Add("Successfully started a server at " + ColorString(networkManager.GetComponent<NetworkManager>().networkAddress, colors["yellow"]));
            return response;
        }
        #endregion

        #region Multiplayer Related Commands
        if (args[0].ToLower() == "disconnect" && player.GetComponent<NetworkBehaviour>().isServer)
        {
            if (commandsRun != 0)                   //Disconnecting without having typed anything will not affect specific stats.
            {
                commandsRun++;
                PlayerPrefs.SetInt("PlayerTotalCommands", PlayerPrefs.GetInt("PlayerTotalCommands") + commandsRun);
                PlayerPrefs.SetInt("PlayerMatches", PlayerPrefs.GetInt("PlayerMatches") + 1);
                PlayerPrefs.SetInt("PlayerLPM", PlayerPrefs.GetInt("PlayerLPM") + (int)terminalManager.AverageLPM());
                PlayerPrefs.SetInt("PlayerErrors", PlayerPrefs.GetInt("PlayerErrors") + errors);
            }
            player.DisconnectAsClient();
            PlayerPrefs.Save();
            networkManager.GetComponent<NetworkManager>().StopHost();
            return response;
        }
        else if(args[0].ToLower() == "disconnect" && player.GetComponent<NetworkBehaviour>().isClient)
        {
            if (commandsRun != 0)
            {
                commandsRun++;
                PlayerPrefs.SetInt("PlayerTotalCommands", PlayerPrefs.GetInt("PlayerTotalCommands") + commandsRun);
                PlayerPrefs.SetInt("PlayerMatches", PlayerPrefs.GetInt("PlayerMatches") + 1);
                PlayerPrefs.SetInt("PlayerLPM", PlayerPrefs.GetInt("PlayerLPM") + (int)terminalManager.AverageLPM());
                PlayerPrefs.SetInt("PlayerErrors", PlayerPrefs.GetInt("PlayerErrors") + errors);
            }
            player.DisconnectAsClient();
            PlayerPrefs.Save();
            StartCoroutine(GetOut());
            return response;
        }

        if(args[0] == "ipaddress" && player.GetComponent<NetworkBehaviour>().isServer)
        {
            response.Add("Your IP Address: " + ColorString(networkManager.GetComponent<GameManager>().GetIP(), colors["white"]) + ". You are the " + ColorString("host", colors["white"]));
            commandsRun++;
            return response;
        }
        else if(args[0] == "ipaddress" && player.GetComponent<NetworkBehaviour>().isClient)
        {
            response.Add("Your IP Address: " + ColorString(networkManager.GetComponent<GameManager>().GetIP(), colors["white"]) + ". You are the " + ColorString("client", colors["white"]));
            commandsRun++;
            return response;
        }

        if (args[0] == "gettag")
        {
            response.Add("Your gamertag: " + ColorString(player.playerTag, colors["yellow"]));
            commandsRun++;
            return response;
        }

        if (args[0] == "scoreboard" && scoreboard.gameObject.activeSelf == false)
        {
            scoreboard.gameObject.SetActive(true);
            commandsRun++;
            return response;
        }
        else if (args[0] == "scoreboard" && scoreboard.gameObject.activeSelf == true)
        {
            scoreboard.gameObject.SetActive(false);
            commandsRun++;
            return response;
        }

        if (args[0].ToLower() == "match" && args[1].ToLower() == "end" && player.isServer)
        {
            player.CmdHostEndedMatch();
            commandsRun++;
            return response;
        }
        else if (args[0].ToLower() == "match" && args[1].ToLower() == "end" && player.isClient)
        {
            response.Add("Only the " + ColorString("host", colors["yellow"]) + " of the match can run this command.");
            commandsRun++;
            return response;
        }

        if (args[0] == "versusscores")
        {
            commandsRun++;
            foreach(var item in player.GetComponent<VersusPlayerScript>().versusKills)
            {
                response.Add(item.Key.playerTag + ", " + item.Value);
            }
            return response;
        }
        #endregion

        #region Gameplay Commands
        if(args[0] == "move" && args[1] != null)
        {
            if (args[1] == "left" || args[1] == "right"){ player.directLR = args[1]; }
            else { player.directUD = args[1]; }
            commandsRun++;
            return response;
        }

        if(args[0] == "stop" && args[1] == "all")
        {
            player.StopMovement();
            commandsRun++;
            return response;
        }
        else if(args[0] == "stop" && args[1] == "lateral")
        {
            player.directLR = null;
            player.move = new Vector3(0f, 0f, player.move.z);
            commandsRun++;
            return response;
        }
        else if(args[0] == "stop" && args[1] == "medial")
        {
            player.directUD = null;
            player.move = new Vector3(player.move.x, 0f, 0f);
            commandsRun++;
            return response;
        }

        if (args[0] == "dash" && args[1] != null && dashTimer == 0f)
        {
            player.dashDir = args[1];
            dashTimer = 10f;
            commandsRun++;
            return response;
        }
        else if (args[0] == "dash" && args[1] != null && dashTimer != 0f)
        {
            response.Add("Your Dash ability refreshes in " + ColorString(dashTimer.ToString(), colors["white"]) + " seconds.");
            commandsRun++;
            return response;
        }

        if(args[0] == "attack")
        {
            player.doAttack = true;
            player.CmdEnableCircle();
            player.StopMovement();
            commandsRun++;
            return response;
        }

        if(args[0] == "respawn" && player.health <= 0)          //Running the command when alive resulted in the player immediately respawning upon death.
        {
            player.toRespawn = true;
            commandsRun++;
            return response;
        }
        else if(args[0] == "respawn" && player.health != 0)
        {
            response.Add("You may be dead inside in real life, but you're alive here, in the game that is.");
            commandsRun++;
            return response;
        }

        if(args[0] == "kill")
        {
            player.CmdSeppuku();
            response.Add("You acquitted yourself with honor, I guess.");
            commandsRun++;
            return response;
        }
        #endregion

        #region Easter Eggs and Jokes
        if (args[0] == "masterkey")
        {
            response.Add("At your service!");
            commandsRun++;
            return response;
        }

        if (args[0] == "Lernaean" || args[0] == "lernaean" || args[0] == "L3rNa3aN")
        {
            response.Add(ColorString("The L3rNa3aN>", colors["green"]) + " Aren't you supposed to be fighting?");
            commandsRun++;
            return response;
        }

        if (args[0] == "Hi" || args[0] == "hi" || args[0] == "Hey" || args[0] == "hey")
        {
            response.Add("This ain't the right time to greet someone.");
            commandsRun++;
            return response;
        }

        if (args[0] == "Shakira" || args[0] == "shakira")
        {
            response.Add("I understand hips don't lie, but this isn't the time.");
            commandsRun++;
            return response;
        }
        #endregion

        #region Normal Commands
        if (args[0] == "clear")
        {
            terminalManager.ClearScreen();
            commandsRun++;
            return response;
        }

        if (args[0] == "quit" || args[0] == "exit")
        {
            if(commandsRun != 0)
            {
                commandsRun++;
                PlayerPrefs.SetInt("PlayerTotalCommands", PlayerPrefs.GetInt("PlayerTotalCommands") + commandsRun);
                PlayerPrefs.SetInt("PlayerMatches", PlayerPrefs.GetInt("PlayerMatches") + 1);
                PlayerPrefs.SetInt("PlayerLPM", PlayerPrefs.GetInt("PlayerLPM") + (int)terminalManager.AverageLPM());
                PlayerPrefs.SetInt("PlayerErrors", PlayerPrefs.GetInt("PlayerErrors") + errors);
            }
            player.DisconnectAsClient();
            PlayerPrefs.Save();
            Application.Quit();
            return response;
        }
        else
        {
            response.Add(ColorString("Command not recognized. Type ", colors["red"]) + ColorString("help", colors["yellow"]) + ColorString(" for a list of commands", colors["red"]));
            commandsRun++;
            errors++;
            return response;
        }
        #endregion
    }

    public List<string> MatchOver(string userInput)
    {
        response.Clear();
        string[] args = userInput.Split();

        if(args[0] == "move" || args[0] == "attack" || args[0] == "dash" || args[0] == "kill")
        {
            response.Add("The fight is over. The best you can do is leave.");
            return response;
        }

        #region Multiplayer Related Commands
        if (args[0].ToLower() == "disconnect" && player.GetComponent<NetworkBehaviour>().isServer)
        {
            if (commandsRun != 0)                   //Disconnecting without having typed anything will not affect specific stats.
            {
                commandsRun++;
                PlayerPrefs.SetInt("PlayerTotalCommands", PlayerPrefs.GetInt("PlayerTotalCommands") + commandsRun);
                PlayerPrefs.SetInt("PlayerMatches", PlayerPrefs.GetInt("PlayerMatches") + 1);
                PlayerPrefs.SetInt("PlayerLPM", PlayerPrefs.GetInt("PlayerLPM") + (int)terminalManager.AverageLPM());
                PlayerPrefs.SetInt("PlayerErrors", PlayerPrefs.GetInt("PlayerErrors") + errors);
            }
            player.DisconnectAsClient();
            PlayerPrefs.Save();
            networkManager.GetComponent<NetworkManager>().StopHost();
            return response;
        }
        else if (args[0].ToLower() == "disconnect" && player.GetComponent<NetworkBehaviour>().isClient)
        {
            if (commandsRun != 0)
            {
                commandsRun++;
                PlayerPrefs.SetInt("PlayerTotalCommands", PlayerPrefs.GetInt("PlayerTotalCommands") + commandsRun);
                PlayerPrefs.SetInt("PlayerMatches", PlayerPrefs.GetInt("PlayerMatches") + 1);
                PlayerPrefs.SetInt("PlayerLPM", PlayerPrefs.GetInt("PlayerLPM") + (int)terminalManager.AverageLPM());
                PlayerPrefs.SetInt("PlayerErrors", PlayerPrefs.GetInt("PlayerErrors") + errors);
            }
            player.DisconnectAsClient();
            PlayerPrefs.Save();
            StartCoroutine(GetOut());
            return response;
        }

        if (args[0] == "ipaddress" && player.GetComponent<NetworkBehaviour>().isServer)
        {
            response.Add("Your IP Address: " + ColorString(networkManager.GetComponent<GameManager>().GetIP(), colors["white"]) + ". You are the " + ColorString("host", colors["white"]));
            commandsRun++;
            return response;
        }
        else if (args[0] == "ipaddress" && player.GetComponent<NetworkBehaviour>().isClient)
        {
            response.Add("Your IP Address: " + ColorString(networkManager.GetComponent<GameManager>().GetIP(), colors["white"]) + ". You are the " + ColorString("client", colors["white"]));
            commandsRun++;
            return response;
        }

        if (args[0] == "gettag")
        {
            response.Add("Your gamertag: " + ColorString(player.playerTag, colors["yellow"]));
            commandsRun++;
            return response;
        }

        if (args[0] == "scoreboard" && scoreboard.gameObject.activeSelf == false)
        {
            scoreboard.gameObject.SetActive(true);
            commandsRun++;
            return response;
        }
        else if (args[0] == "scoreboard" && scoreboard.gameObject.activeSelf == true)
        {
            scoreboard.gameObject.SetActive(false);
            commandsRun++;
            return response;
        }

        if (args[0] == "versusscores")
        {
            foreach (var item in player.GetComponent<VersusPlayerScript>().versusKills)
            {
                response.Add(item.Key.playerTag + ", " + item.Value);
            }
            return response;
        }
        #endregion

        #region Normal Commands
        if (args[0] == "clear")
        {
            terminalManager.ClearScreen();
            commandsRun++;
            return response;
        }

        if (args[0] == "quit" || args[0] == "exit")
        {
            if (commandsRun != 0)
            {
                commandsRun++;
                PlayerPrefs.SetInt("PlayerTotalCommands", PlayerPrefs.GetInt("PlayerTotalCommands") + commandsRun);
                PlayerPrefs.SetInt("PlayerMatches", PlayerPrefs.GetInt("PlayerMatches") + 1);
                PlayerPrefs.SetInt("PlayerLPM", PlayerPrefs.GetInt("PlayerLPM") + (int)terminalManager.AverageLPM());
                PlayerPrefs.SetInt("PlayerErrors", PlayerPrefs.GetInt("PlayerErrors") + errors);
            }
            player.DisconnectAsClient();
            PlayerPrefs.Save();
            Application.Quit();
            return response;
        }
        else
        {
            response.Add(ColorString("Command not recognized. Type ", colors["red"]) + ColorString("help", colors["yellow"]) + ColorString(" for a list of commands", colors["red"]));
            commandsRun++;
            errors++;
            return response;
        }
        #endregion
    }

    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";

        return leftTag + s + rightTag;
    }

    void ListEntry(string a, string b)
    {
        response.Add(ColorString(a, colors["light blue"]) + ": " + ColorString(b, colors["red"]));
    }

    public List<string> ReceiveNetworkNotifs(string notif)              //Used in PlayTerminalManager. DO NOT DELETE.
    {
        response.Clear();

        if(notif != null)  { response.Add(notif); }
        return response;
    }

    IEnumerator GetOut()
    {
        yield return new WaitForSeconds(0.125f);
        networkManager.GetComponent<NetworkManager>().StopClient();
    }
}