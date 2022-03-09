using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private string notificationMessage = string.Empty;
    public GameObject scoreBoard;

    public List<NetworkConnection> players = new List<NetworkConnection>();

    [ContextMenu("Send Notification")]private void SendNotification()
    {
        NetworkServer.SendToAll(new Notification { content = notificationMessage });
    }

    public override void OnStartClient()
    {
        scoreBoard.GetComponent<ScoreBoard>().FindPlayers();
    }
}
