using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : MonoBehaviour
{
    public NetworkManager networkManager;
    public int spawnIndex;
    public GameObject[] spawnPoints;

    private void Start()
    {
        
    }

    private void Awake()
    {
        networkManager.networkAddress = GetIP();                    //Local machine IP is initialized at the start.
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(networkManager.clientLoadedScene == true)
        {
            Debug.Log("Test");
        }
    }

    public string GetIP()                                            //Returns the IP of the local machine.
    {
        //string strHostName = "";
        string strHostName = Dns.GetHostName();
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;
        return addr[addr.Length - 1].ToString();
    }

    private void OnConnectedToServer()
    {
        Debug.Log("This function runs.");
        //GameObject.Find("SpawnPoint");
        if (GameObject.Find("SpawnPoint"))
        {
            Debug.Log("Found a spawnpoint.");
        }
    }
}
