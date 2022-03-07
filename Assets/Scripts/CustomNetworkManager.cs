using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private string notificationMessage = string.Empty;
    //public GameObject terminalManager;

    [ContextMenu("Send Notification")]private void SendNotification()
    {
        NetworkServer.SendToAll(new Notification { content = notificationMessage });
    }

    private new void Start()
    {
        //terminalManager = GameObject.Find("YOUandAYE");
    }
}
