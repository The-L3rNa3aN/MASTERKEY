using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interpreter : MonoBehaviour
{
    public CustomNetworkManager networkManager;
    TerminalManager terminalManager;
    public bool conn = false;

    [Header("Server Creation Wizard")]
    public string hostName;
    public string mapName;
    public int fragLimit;
    public int timeLimit;

    Dictionary<string, string> colors = new Dictionary<string, string>()
    {
        {"red", "#ff0000" },
        {"light blue", "#347deb" },
        {"green", "#00ff00" },
        {"orange", "ffab0f" },
        {"yellow", "#ffea00" },
        {"aqua", "#00f7ff" },
        {"white", "ffffff" },
        {"flesh tint", "#ff5e97" }
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

        #region Connecting and Hosting
        if (args[0] == "ipaddress")
        {
            response.Add("Your network address: " + ColorString(networkManager.networkAddress, colors["white"]));
            return response;
        }

        if (args[0] == "play" && args[1] == "lan" && args[2] == "create")
        {
            conn = true;
            terminalManager.ServerWizardStart();
            return response;
        }

        if (args[0] == "connect" && args[1] != null)
        {
            networkManager.networkAddress = args[1];
            response.Add("Connecting to " + ColorString(args[1], colors["white"]) + "...");
            networkManager.StartClient();
            return response;
        }
        #endregion

        #region Easter Eggs
        if (args[0] == "masterkey")
        {
            LoadTitle("ascii.txt", "aqua", 2);
            response.Add("At your service!");
            return response;
        }

        if (args[0] == "Lernaean" || args[0] == "lernaean" || args[0] == "L3rNa3aN" || args[0] == "The Lernaean" || args[0] == "the lernaean" || args[0] == "The L3rNa3aN")
        {
            LoadTitle("lernaean 160.txt", "green", 2);
            response.Add("What is it you want?");
            return response;
        }

        if (args[0] == "Hi" || args[0] == "hi" || args[0] == "Hey" || args[0] == "hey")
        {
            response.Add("Hey!");
            return response;
        }

        if (args[0] == "Shakira" || args[0] == "shakira")
        {
            response.Add("Three golden words: " + ColorString("HIPS DON'T LIE", colors["yellow"]));
            return response;
        }

        if (args[0] == "who" || args[0] == "Who" && args[1] == "are" && args[2] == "you")
        {
            response.Add("I'm a machine written by The L3rNa3aN.");
            return response;
        }
        #endregion

        #region Utilitarian
        if (args[0] == "stats" && args[1] == "display")
        {
            response.Add("Thy p'rsonal r'cord, " + ColorString(PlayerPrefs.GetString("PlayerName"), colors["yellow"]) + ": -");
            ListEntry("Times thee has't slay'd", PlayerPrefs.GetInt("PlayerKills").ToString());
            ListEntry("Times thee has't fallen", PlayerPrefs.GetInt("PlayerDeaths").ToString());
            ListEntry("Killing sprees thee did start", PlayerPrefs.GetInt("PlayerSpreesStarted").ToString());
            ListEntry("Killing sprees thee end'd", PlayerPrefs.GetInt("PlayerSpreesEnded").ToString());
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

        if (args[0] == "settag" && args[1] != null)
        {
            response.Add("Your player tag is now set to " + ColorString(args[1], colors["yellow"]));
            PlayerPrefs.SetString("PlayerName", args[1]);
            return response;
        }

        if (args[0] == "about")
        {
            response.Add(ColorString("MASTERKEY", colors["green"]) + " is a game where you need to type in order to play, a little like other typing games but this time with a little action involved.");
            return response;
        }

        if (args[0] == "clear")
        {
            terminalManager.ClearScreen();
            return response;
        }

        if (args[0] == "quit" || args[0] == "exit")
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
        #endregion
    }

    public List<string> ServerCreationWizard(string userInput)
    {
        response.Clear();
        string[] args = userInput.Split();

        if(userInput == "wizard")
        {
            response.Add(ColorString("Welcome to the SERVER CREATION WIZARD!", colors["flesh tint"]));
            response.Add("For more information about commands here, type " + ColorString("help", colors["white"]) + " to get started.");
            response.Add("To leave this wizard, type " + ColorString("leave", colors["white"]));
            return response;
        }

        if(args[0] == "help")
        {
            ListEntry("hostname <name>", "Sets the name of the host.");
            ListEntry("map <mapname>", "Set the map for the server.");
            ListEntry("fraglimit", "Sets the frag limit of the server. For no limit, type 0.");
            ListEntry("timelimit", "Sets the time limit in seconds. For no limit, type 0");
            ListEntry("maplist", "Returns a list of map names.");
            ListEntry("settings", "Displays the server settings you have configured in the wizard.");
            ListEntry("start", "Starts the server based on the settings you've configured.");
            return response;
        }

        if(args[0] == "settings")
        {
            response.Add(ColorString("Your server settings so far...", colors["flesh tint"]));
            response.Add(ColorString("Host Name  : ", colors["flesh tint"]) + ColorString(hostName, colors["white"]));
            response.Add(ColorString("Map        : ", colors["flesh tint"]) + ColorString(mapName, colors["white"]));
            response.Add(ColorString("Frag Limit : ", colors["flesh tint"]) + ColorString(fragLimit.ToString(), colors["white"]));
            response.Add(ColorString("Time Limit : ", colors["flesh tint"]) + ColorString(timeLimit.ToString(), colors["white"]));
            response.Add("");
            response.Add(ColorString("Always make sure to check your server settings before hosting!", colors["red"]));
            return response;
        }

        if(args[0] == "maplist")
        {
            response.Add("Here is a list of maps: -");
            response.Add(ColorString("map_chapel", colors["light blue"]));
            response.Add(ColorString("map_heahlond", colors["light blue"]));
            response.Add(ColorString("map_ruins", colors["light blue"]));
            response.Add(ColorString("map_siege", colors["light blue"]));
            response.Add(ColorString("map_waelstow", colors["light blue"]));
            return response;
        }

        if(args[0] == "map" && args[1] != null)
        {
            if(args[1] == "map_chapel" || args[1] == "map_heahlond" || args[1] == "map_ruins" || args[1] == "map_siege" || args[1] == "map_waelstow")
            {
                mapName = args[1];
                response.Add("The map has been set to " + ColorString(args[1], colors["yellow"]));
                return response;
            }
            else if(args[1] == "blackmesa" || args[1] == "black_mesa")
            {
                response.Add("Unfortunately, you cannot visit Black Mesa at the moment. There appears to be a containment breach and the facility has been quarantined. Apologies for the inconvenience.");
                return response;
            }
            else
            {
                response.Add(ColorString("Map not recognizable. Use ", colors["red"]) + ColorString("maplist", colors["yellow"]) + ColorString(" to get a list of maps you can refer to.", colors["red"]));
                return response;
            }
        }

        if (args[0] == "fraglimit" && args[1] != null)
        {
            fragLimit = int.Parse(args[1]);
            response.Add(ColorString("Frag Limit", colors["white"]) + " has been set to " + args[1]);
            return response;
        }

        if (args[0] == "timelimit" && args[1] != null)
        {
            timeLimit = int.Parse(args[1]);
            response.Add(ColorString("Time Limit", colors["white"]) + " has been set to " + args[1]);
            return response;
        }

        if(args[0] == "clear")
        {
            terminalManager.ClearScreen();
            response.Add("You are still running the " + ColorString("Server Creation Wizard", colors["flesh tint"]));
            response.Add("If you wish to leave, type " + ColorString("leave", colors["flesh tint"]));
            return response;
        }

        if(args[0] == "start")
        {
            if(mapName != null)
            {
            networkManager.onlineScene = mapName;
            networkManager.StartHost();
            return response;
            }
            else
            {
                response.Add(ColorString("You haven't chosen a map you want to play on! Use ", colors["red"]) + ColorString("map", colors["yellow"]) + ColorString(" to set the server's map.", colors["red"]));
                return response;
            }
        }

        if (args[0] == "leave")
        {
            conn = false;
            response.Add("Exiting the server creation wizard...");
            return response;
        }
        else
        {
            response.Add(ColorString("We couldn't recognize that. Try again, perhaps?", colors["red"]));
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