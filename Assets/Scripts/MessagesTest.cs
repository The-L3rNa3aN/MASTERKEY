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

        NetworkClient.ReplaceHandler<Notification>(OnNotification);         //Using "RegisterHandler" caused warnings and won't broadcast some messages.
    }

    private void OnNotification(Notification msg)
    {
        terminal.Notifs(msg.content);
    }
}
