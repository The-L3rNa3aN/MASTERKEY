using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public NetworkManager networkManager;
    public int spawnIndex;

    public List<GameObject> spawns = new List<GameObject>();                                        //Keeps track of the spawnpoints of a level. Needed for respawning.

    private void Awake()
    {
        networkManager.networkAddress = GetIP();                                                    //Local machine IP is initialized at the start.
    }

    public string GetIP()                                                                           //Returns the IP of the local machine.
    {
        //string strHostName = "";
        string strHostName = Dns.GetHostName();
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;
        return addr[addr.Length - 1].ToString();
    }
}
