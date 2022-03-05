using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Interpreter : MonoBehaviour
{
    public NetworkManager networkManager;
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

        var playerName = networkManager.GetComponent<GameManager>().playerName;

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

        if(args[0] == "gettag")
        {
            response.Add("Your playertag: " + ColorString(playerName, colors["yellow"]));
            return response;
        }

        if(args[0] == "settag" && args[1] != null)
        {
            response.Add("Your player tag is now set to " + ColorString(args[1], colors["yellow"]));
            //PlayerPrefs.SetString("PlayerName", args[1]);
            return response;
        }

        if(args[0] == "help")
        {
            response.Add("Here is a list of commands you can use." + ColorString("CAUTION", colors["red"]) + ": " + "Commands are case sensitive!");
            ListEntry("help", "Returns a list of commands.");
            ListEntry("about", "Returns a small but brief paragraph about this game.");
            ListEntry("clear", "Clears the terminal screen.");
            ListEntry("start host", "Starts a LAN server on your computer.");
            ListEntry("connect <IP Address>", "Manually connect to a LAN host in the network.");
            ListEntry("settag", "Set your name.");
            ListEntry("gettag", "Get your name.");
            ListEntry("exit", "Exits the game.");
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

        if(args[0] == "getnetaddress")
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

        if(args[0] == "exit")
        {
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

    void ListEntry(string a, string b)
    {
        response.Add(ColorString(a, colors["light blue"]) + ": " + ColorString(b, colors["red"]));
    }

    void LoadTitle(string path, string color, int spacing)
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
}