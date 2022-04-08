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
    public bool dea = false;

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
            ListEntry("about", "Returns some information about the game.");
            ListEntry("clear", "Clears the terminal screen.");
            ListEntry("create server", "Starts a server creation wizard used to host LAN games.");
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

        if (args[0] == "create" && args[1] == "server")
        {
            conn = true;
            response.Add(ColorString("Welcome to the SERVER CREATION WIZARD!", colors["flesh tint"]));
            response.Add("For more information about commands here, type " + ColorString("help", colors["white"]) + " to get started.");
            response.Add("To leave this wizard, type " + ColorString("leave", colors["white"]));
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

        if(args[0] == "Fortnite" || args[0] == "fortnite")
        {
            response.Add("You mean 'Fornight'?");
            return response;
        }

        if(args[0] == "Zork" || args[0] == "zork")
        {
            response.Add("Zork isn't at your service.");
            return response;
        }

        if(args[0].ToLower() == "saul" && args[1].ToLower() == "goodman")
        {
            LoadTitle("kevin costner.txt", "green", 2);
            response.Add("Is this Kevin Costner?");
            return response;
        }

        if(args[0].ToLower() == "gordon" && args[1].ToLower() == "freeman")
        {
            response.Add("Are you talking about the 27 year old scientist or the 52 year old coughing meth emperor?");
            return response;
        }

        if(args[0].ToLower() == "blue" && args[1].ToLower() == "sky")
        {
            dea = true;
            response.Add(ColorString("The DEA would like to know your location.", colors["light blue"]));
            return response;
        }

        if(args[0].ToLower() == "ezekiel" && args[1] == "25" && args[2] == "17")
        {
            response.Add("If you don't know what this scripture is, you are not a Pulp Fiction fan.");
            return response;
        }

        if(args[0].ToLower() == "pulp" && args[1].ToLower() == "fiction")
        {
            response.Add("This game is medieval themed anyway. Marsellus will love this one.");
            return response;
        }

        /*if (args[0] == "who" || args[0] == "Who" && args[1] == "are" && args[2] == "you")
        {
            response.Add("I'm a machine written by The L3rNa3aN.");
            return response;
        }*/
        #endregion

        #region Utilitarian
        if (args[0] == "stats" && args[1] == "display")
        {
            response.Add("Your personal records, " + ColorString(PlayerPrefs.GetString("PlayerName"), colors["yellow"]) + ": -");
            ListEntry("The number of times you slayed           ", PlayerPrefs.GetInt("PlayerKills").ToString());
            ListEntry("The number of times you died             ", PlayerPrefs.GetInt("PlayerDeaths").ToString());
            ListEntry("The number of killing sprees you started ", PlayerPrefs.GetInt("PlayerSpreesStarted").ToString());
            ListEntry("The number of killing sprees you ended   ", PlayerPrefs.GetInt("PlayerSpreesEnded").ToString());
            ListEntry("Your typing speed                        ", PlayerAverageSpeed().ToString());
            ListEntry("The number of matches you participated in", PlayerPrefs.GetInt("PlayerMatches").ToString());
            ListEntry("The number of commands you ran           ", PlayerPrefs.GetInt("PlayerTotalCommands").ToString());
            ListEntry("The number of mistakes you made          ", PlayerPrefs.GetInt("PlayerErrors").ToString());
            ListEntry("Your accuracy                            ", PlayerAccuracy().ToString() + "%");
            ListEntry("The total time that you spent here       ", PlayerTimeFormat());
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
            LoadTitle("ascii.txt", "aqua", 2);
            response.Add("A game where typing and action go together. Made with UNITY (and injured fingers).");
            response.Add("");
            response.Add(ColorString("Meet the team!", colors["light blue"]));
            response.Add(ColorString("  Veman ", colors["yellow"]) + ColorString("'The L3rNa3aN'", colors["red"]) + ColorString(" Jadhav.", colors["yellow"]) + ": Programming, sound and general game design.");
            response.Add(ColorString("  Adarsh Kumar ", colors["yellow"]) + ColorString("'Thunderbolt'", colors["red"]) + ColorString(" Singh.", colors["yellow"]) + ": Modelling, texturing and animation.");
            response.Add("");
            response.Add(ColorString("Special thanks...", colors["light blue"]));
            response.Add("  Tarang Vijay 'Scoobs' Soni.");
            response.Add("  FourierSoft.");
            response.Add("  The Mirror development team and community.");
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

        if(args[0] == "help")
        {
            ListEntry("map <mapname>", "Set the map for the server.");
            ListEntry("fraglimit", "Sets the frag limit of the server. For no limit, type 0.");
            ListEntry("maplist", "Returns a list of map names.");
            ListEntry("settings", "Displays the server settings you have configured in the wizard.");
            ListEntry("start", "Starts the server based on the settings you've configured.");
            return response;
        }

        if(args[0] == "settings")
        {
            response.Add(ColorString("Your server settings so far...", colors["flesh tint"]));
            response.Add(ColorString("Map        : ", colors["flesh tint"]) + ColorString(mapName, colors["white"]));
            response.Add(ColorString("Frag Limit : ", colors["flesh tint"]) + ColorString(fragLimit.ToString(), colors["white"]));
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
                response.Add(ColorString("The Administrator>", colors["green"]) + " You cannot visit Black Mesa at the moment. There is a containment breach but it is being handled, so fear not.");
                return response;
            }
            else if(args[1] == "dust2" || args[1] == "de_dust2")
            {
                response.Add("Are you sure you are playing the correct game? This is " + ColorString("MASTERKEY", colors["aqua"]) + ", not " + ColorString("COUNTER STRIKE", colors["yellow"]));
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
            response.Add(ColorString("Frag Limit", colors["white"]) + " has been set to " + ColorString(args[1], colors["white"]));
            return response;
        }

        if (args[0] == "timelimit" && args[1] != null)
        {
            timeLimit = int.Parse(args[1]);
            response.Add(ColorString("Time Limit", colors["white"]) + " has been set to " + ColorString(args[1], colors["white"]) + "seconds.");
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
                PlayerPrefs.SetInt("FragLimit", fragLimit);
                networkManager.StartHost();
                return response;
            }
            else if(mapName == null)
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
            response.Add(ColorString("Command not recognizable. Please try again.", colors["red"]));
            return response;
        }
    }

    public List<string> BreakingBadDEA(string userInput)
    {
        response.Clear();
        string[] args = userInput.Split();

        if(args[0].ToLower() == "dea")
        {
            response.Add(ColorString("Yes. The DEA is coming for you. You can't play this game anymore. I don't want to go to jail.", colors["light blue"]));
            return response;
        }

        if(args[0].ToLower() == "help")
        {
            response.Add(ColorString("I can't help you. You're on your own.", colors["light blue"]));
            return response;
        }

        if (args[0].ToLower() == "saul" && args[1].ToLower() == "goodman")
        {
            response.Add(ColorString("Saul Goodman left the game a long time ago.", colors["light blue"]));
            return response;
        }

        if(args[0].ToLower() == "gus" || args[0].ToLower() == "gustavo" && args[1].ToLower() == "fring")
        {
            response.Add(ColorString("What does an honest chicken restaurant owner got to do with Blue Sky?", colors["light blue"]));
            return response;
        }

        if(args[0].ToLower() == "walter" && args[1].ToLower() == "white")
        {
            response.Add(ColorString("Chemistry is the study of change, a wise man once said. It is time for you to change cities, my friend.", colors["light blue"]));
            return response;
        }

        if(args[0].ToLower() == "jesse" && args[1].ToLower() == "pinkman")
        {
            response.Add(ColorString("Yeah, he's a hard man to find.", colors["light blue"]));
            return response;
        }

        if(args[0].ToLower() == "cartel")
        {
            response.Add(ColorString("Because of your little stunt trying to enquire about our product, one of our high ranking members now has the Federales in his rosebushes.", colors["light blue"]));
            return response;
        }

        if(args[0].ToLower() == "heisenberg")
        {
            response.Add(ColorString("Yeah, that's another reason why the DEA is coming.", colors["light blue"]));
            return response;
        }

        if(args[0].ToLower() == "exit" || args[0].ToLower() == "quit" || args[0].ToLower() == "mike")
        {
            PlayerPrefs.Save();
            Application.Quit();
            return response;
        }
        else
        {
            response.Add(ColorString("You seem to be under a lot of stress. Use the terminal when you see things more clearly.", colors["light blue"]));
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