using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interpreter : MonoBehaviour
{
    public CustomNetworkManager networkManager;
    TerminalManager terminalManager;

    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"red", "#ff0000" },
        {"light blue", "#7070ff" },
        {"green", "#00ff00" },
        {"orange", "ffab0f" },
        {"yellow", "#ffea00" },
        {"aqua", "#00f7ff" },
        {"white", "ffffff" }
    };

    List<string> response = new List<string>();

    private void Start()
    {
        terminalManager = GetComponent<TerminalManager>();
    }

    public List<string> Interpret(string userInput)
    {
        response.Clear();
        string[] args = userInput.Split();

        #region Welcome Responses
        if (userInput == "welcome")
        {
            response.Add(ColorString("Hello, and welcome to ", colors["yellow"]) + ColorString("MASTERKEY!", colors["aqua"]));
            return response;
        }

        if(userInput == "welcomehelp")
        {
            response.Add("To get started, type " + ColorString("help ", colors["yellow"]) + "to get a list of commands or use the" + ColorString(" ? ", colors["yellow"]) + "before the command you want to learn about.");
            return response;
        }
        #endregion

        if (args[0] == "help")
        {
            response.Add("Here is a list of commands you can use." + ColorString("CAUTION", colors["red"]) + ": " + "Commands are case sensitive!");
            ListEntry("help", "Returns a list of commands.");
            ListEntry("about", "Returns a small but brief paragraph about this game.");
            ListEntry("clear", "Clears the terminal screen.");
            ListEntry("start host", "Starts a LAN server on your computer.");
            ListEntry("connect <IP Address>", "Manually connect to a LAN host in the network.");
            ListEntry("ipaddress", "Reveal your machine's IP Address. Useful when someone needs to connect to a server you're hosting.");
            ListEntry("settag", "Set your name.");
            ListEntry("gettag", "Get your name.");
            ListEntry("stats <display / reset>", "View and manage your personal stats.");
            ListEntry("quit / exit", "Exits the game.");
            response.Add("");
            response.Add(ColorString("[ONLINE COMMANDS]", colors["aqua"]));
            ListEntry("move <direction>", "Moves your character around. Directions include: up, down, left and right.");
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
            return response;
        }

        if (args[0] == "stats" && args[1] == "display")
        {
            response.Add("Thy p'rsonal r'cord, " + ColorString(PlayerPrefs.GetString("PlayerName"), colors["yellow"]) + ": -");
            ListEntry("Times thee has't slay'd", PlayerPrefs.GetInt("PlayerKills").ToString());
            ListEntry("Times thee has't fallen", PlayerPrefs.GetInt("PlayerDeaths").ToString());
            ListEntry("Thy swiftness", PlayerAverageSpeed().ToString());
            ListEntry("Times thee've did join combat", PlayerPrefs.GetInt("PlayerMatches").ToString());
            ListEntry("The total hests thee've runneth", PlayerPrefs.GetInt("PlayerTotalCommands").ToString());
            ListEntry("The total mishaps thee've madeth", PlayerPrefs.GetInt("PlayerErrors").ToString());
            ListEntry("Thy exactness", PlayerAccuracy().ToString() + "%");
            ListEntry("Thy timeth hath spent h're", PlayerTimeFormat());
            return response;
        }

        if (args[0] == "stats" && args[1] == "reset")
        {
            PlayerPrefs.SetInt("PlayerKills", 0);
            PlayerPrefs.SetInt("PlayerDeaths", 0);
            PlayerPrefs.SetInt("PlayerLPM", 0);
            PlayerPrefs.SetInt("PlayerMatches", 0);
            PlayerPrefs.SetInt("PlayerTotalCommands", 0);
            PlayerPrefs.SetInt("PlayerErrors", 0);
            PlayerPrefs.SetInt("PlayerTime", 0);
            response.Add("Your stats have been reset, " + ColorString(PlayerPrefs.GetString("PlayerName"), colors["yellow"]));
            return response;
        }

        if (args[0] == "gettag")
        {
            response.Add("Your playertag: " + ColorString(PlayerPrefs.GetString("PlayerName"), colors["yellow"]));
            return response;
        }

        if(args[0] == "settag" && args[1] != null)
        {
            response.Add("Your player tag is now set to " + ColorString(args[1], colors["yellow"]));
            PlayerPrefs.SetString("PlayerName", args[1]);
            return response;
        }

        if(args[0] == "about")
        {
            response.Add(ColorString("MASTERKEY", colors["green"]) + " is a game where you need to type in order to play, a little like other typing games but this time with a little action involved.");
            return response;
        }

        if (args[0] == "clear")
        {
            terminalManager.ClearScreen();
            return response;
        }

        if(args[0] == "ipaddress")
        {
            response.Add("Your network address: " + ColorString(networkManager.networkAddress, colors["white"]));
            return response;
        }

        if (args[0] == "start" && args[1] == "host")
        {
            networkManager.StartHost();
            return response;
        }

        if(args[0] == "connect" && args[1] != null)
        {
            networkManager.networkAddress = args[1];
            response.Add("Connecting to " + ColorString(args[1], colors["white"]) + "...");
            networkManager.StartClient();
            return response;
        }

        if (args[0] == "masterkey")
        {
            LoadTitle("ascii.txt", "aqua", 2);
            response.Add("At your service!");
            return response;
        }

        if(args[0] == "Lernaean" || args[0] == "lernaean" || args[0] == "L3rNa3aN" || args[0] == "The Lernaean" || args[0] == "the lernaean" || args[0] == "The L3rNa3aN")
        {
            LoadTitle("lernaean 160.txt", "green", 2);
            response.Add("What is it you want?");
            return response;
        }

        if(args[0] == "Hi" || args[0] == "hi" || args[0] == "Hey" || args[0] == "hey")
        {
            response.Add("Hey!");
            return response;
        }

        if(args[0] == "Shakira" || args[0] == "shakira")
        {
            response.Add("Three golden words: " + ColorString("HIPS DON'T LIE", colors["yellow"]));
            return response;
        }

        if(args[0] == "who" || args[0] == "Who" && args[1] == "are" && args[2] == "you")
        {
            response.Add("I'm a machine written by The L3rNa3aN.");
            return response;
        }

        if(args[0] == "quit" || args[0] == "exit")
        {
            PlayerPrefs.SetInt("PlayerTime", PlayerPrefs.GetInt("PlayerTime") + (int)networkManager.timeSession);
            Application.Quit();
            return response;
        }
        else
        {
            response.Add(ColorString("Command not recognized. Type ", colors["red"]) + ColorString("help", colors["yellow"]) + ColorString(" for a list of commands", colors["red"]));
            return response;
        }
    }

    public string ColorString(string s, string color)
    {
        string leftTag = "<color=" + color + ">";
        string rightTag = "</color>";

        return leftTag + s + rightTag;
    }

    private void ListEntry(string a, string b)
    {
        response.Add(ColorString(a, colors["light blue"]) + ": " + ColorString(b, colors["red"]));
    }

    private void LoadTitle(string path, string color, int spacing)
    {
        StreamReader file = new StreamReader(Path.Combine(Application.streamingAssetsPath, path));
        Debug.Log(path);
        for(int i = 0; i < spacing; i++)
        {
            response.Add("");
        }

        while(!file.EndOfStream)
        {
            response.Add(ColorString(file.ReadLine(), colors[color]));
        }

        for (int i = 0; i < spacing; i++)
        {
            response.Add("");
        }

        file.Close();
    }

    private string PlayerTimeFormat()                               //Formats the PlayerTime preference to hours, minutes and seconds.
    {
        var time = PlayerPrefs.GetInt("PlayerTime");
        var timeSpan = TimeSpan.FromSeconds(time);
        string timeFormatted = timeSpan.Hours.ToString() + "h " + timeSpan.Minutes.ToString() + "m " + timeSpan.Seconds.ToString() + "s";
        return timeFormatted;
    }

    private int PlayerAccuracy()                                    //Calculates accuracy by subtracting 100 by the percentage of errors.
    {
        float accuracy = 0f;
        float errors = PlayerPrefs.GetInt("PlayerErrors");
        float commands = PlayerPrefs.GetInt("PlayerTotalCommands");
        if(errors != 0f && commands != 0f) { accuracy = 100 - ((errors / commands) * 100); }
        else { accuracy = 0; }
        
        return (int)accuracy;
    }

    private int PlayerAverageSpeed()
    {
        float speed = 0f;
        float lpm = PlayerPrefs.GetInt("PlayerLPM");
        float matches = PlayerPrefs.GetInt("PlayerMatches");
        if (lpm != 0f && matches != 0f) { speed = lpm / matches; }
        else speed = 0f;
        return (int)speed;
    }
}