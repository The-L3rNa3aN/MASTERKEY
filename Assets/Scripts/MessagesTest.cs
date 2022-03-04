using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public struct Notification : NetworkMessage
{
    public string content;
}

public class MessagesTest : MonoBehaviour
{
    [SerializeField] private PlayTerminalManager terminal = null;

    private void Start()
    {
        if(!NetworkClient.active) { return; }

        NetworkClient.RegisterHandler<Notification>(OnNotification);
    }

    private void OnNotification(Notification msg)
    {
        terminal.PlayerJoined(msg.content);
    }
}
