using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private string notificationMessage = string.Empty;

    [ContextMenu("Send Notification")]private void SendNotification()
    {
        NetworkServer.SendToAll(new Notification { content = notificationMessage });
    }

    private void Update()
    {
        Debug.Log(numPlayers);
    }
}
