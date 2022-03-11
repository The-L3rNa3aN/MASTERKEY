using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    public float timeSession;
    [SerializeField] private string notificationMessage = string.Empty;

    [ContextMenu("Send Notification")] private void SendNotification()
    {
        NetworkServer.SendToAll(new Notification { content = notificationMessage });
    }

    private void Update()
    {
        timeSession = Time.realtimeSinceStartup;
    }
}
