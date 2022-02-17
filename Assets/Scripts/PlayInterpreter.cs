using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayInterpreter : MonoBehaviour
{
    PlayTerminalManager terminalManager;
    GameObject networkManager;
    public PlayerManager player;
    int units;

    [SerializeField] float dashTimer = 0f;

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

        /*if(player.attackSphere.activeInHierarchy == true)
        {
            player.attackSphere.SetActive(false);
        }*/
    }

    public List<string> Interpret(string userInput)
    {
        response.Clear();

        string[] args = userInput.Split();

        #region Multiplayer Related Commands
        if(args[0].ToLower() == "disconnect" && player.gameObject.GetComponent<NetworkBehaviour>().isServer)
        {
            networkManager.GetComponent<NetworkManager>().StopHost();
            return response;
        }
        else if(args[0].ToLower() == "disconnect" && player.gameObject.GetComponent<NetworkBehaviour>().isClient)
        {
            networkManager.GetComponent<NetworkManager>().StopClient();
            return response;
        }
        #endregion

        #region Gameplay Commands
        if (args[0] == "help")
        {
            response.Add("Here is a list of commands you can use." + ColorString("CAUTION", colors["red"]) + ": " + "Commands are case sensitive!");
            ListEntry("move [direction] [number]", "To move the character around. Directions include: up, down, left and right.");
            ListEntry("dash [direction]", "Performs a high speed, short ranged dash. Refreshes every 30 seconds.");
            ListEntry("clear", "Clears the terminal screen.");
            ListEntry("disconnect", "Disconnects and returns to the main terminal screen.");
            ListEntry("exit", "Exits the game.");
            return response;
        }

        if(args[0] == "move" && args[1] != null && args[2] != null)
        {
            units = Int32.Parse(args[2]);
            if (args[1] == "left" || args[1] == "right")
            {
                player.directLR = args[1];
                player.unitsLR = (float)units;
            }
            else
            {
                player.directUD = args[1];
                player.unitsUD = (float)units;
            }
            return response;
        }
        
        if(args[0] == "stop")
        {
            player.StopMovement();
            return response;
        }

        if (args[0] == "dash" && args[1] != null && dashTimer == 0f)
        {
            player.dashDir = args[1];
            dashTimer = 10f;
            return response;
        }
        else if (args[0] == "dash" && args[1] != null && dashTimer != 0f)
        {
            response.Add("Your Dash ability refreshes in " + ColorString(dashTimer.ToString(), colors["white"]) + " seconds.");
            return response;
        }

        if(args[0] == "attack")
        {
            player.doAttack = true;
            return response;
        }
        #endregion

        #region Easter Eggs and Jokes
        if (args[0] == "masterkey")
        {
            response.Add("At your service!");
            return response;
        }

        if (args[0] == "Lernaean" || args[0] == "lernaean" || args[0] == "L3rNa3aN")
        {
            response.Add(ColorString("The L3rNa3aN>", colors["green"]) + " Aren't you supposed to be fighting?");
            return response;
        }

        if (args[0] == "Hi" || args[0] == "hi" || args[0] == "Hey" || args[0] == "hey")
        {
            response.Add("This ain't the right time to greet someone.");
            return response;
        }

        if (args[0] == "Shakira" || args[0] == "shakira")
        {
            response.Add("I understand hips don't lie, but this isn't the time.");
            return response;
        }
        #endregion

        #region Normal Commands
        if (args[0] == "clear")
        {
            terminalManager.ClearScreen();
            return response;
        }

        if (args[0] == "quit" || args[0] == "exit")
        {
            Application.Quit();
            return response;
        }
        else
        {
            response.Add(ColorString("Command not recognized. Type help for a list of commands.", colors["red"]));
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
}