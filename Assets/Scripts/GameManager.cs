using System.Net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : MonoBehaviour
{
    NetworkManager networkManager;

    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    private void Awake()
    {
        networkManager.networkAddress = GetIP();                    //Local machine IP is initialized at the start.
    }

    private void Update()
    {
        
    }

    private string GetIP()                                           //Returns the IP of the local machine.
    {
        string strHostName = "";
        strHostName = Dns.GetHostName();
        IPHostEntry ipEntry = Dns.GetHostEntry(strHostName);
        IPAddress[] addr = ipEntry.AddressList;
        return addr[addr.Length - 1].ToString();
    }
}
