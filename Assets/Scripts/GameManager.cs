using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class GameManager : MonoBehaviour
{
    public NetworkManager networkManager;
    public string playerName;

    private void Awake()
    {
        networkManager.networkAddress = GetIP();                                                    //Local machine IP is initialized at the start.
        playerName = PlayerPrefs.GetString("PlayerName");
    }

    private void Start()
    {
        
    }

    public string GetIP()                                                                           //Returns the IP of the local machine.
    {
        string strHostName = Dns.GetHostName();
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;
        return addr[addr.Length - 1].ToString();
    }
}
