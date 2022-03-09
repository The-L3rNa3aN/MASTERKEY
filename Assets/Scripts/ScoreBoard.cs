using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreBoardItem;
    [SerializeField] GameObject gameManager;

    public List<PlayerManager> playerList = new List<PlayerManager>();
    Dictionary<PlayerManager, ScoreboardItemManager> items = new Dictionary<PlayerManager,ScoreboardItemManager>();

    private void Start()
    {
        gameManager = GameObject.Find("NetworkManager");
        gameManager.GetComponent<CustomNetworkManager>().scoreBoard = gameObject;
    }

    public void FindPlayers()
    {
        var searchPlayer = FindObjectsOfType<PlayerManager>();
        for (int i = 0; i < searchPlayer.Length; i++) { playerList.Add(searchPlayer[i]); }

        foreach (PlayerManager player in playerList)
        {
            AddScoreboardItem(player);
        }
    }

    public void AddScoreboardItem(PlayerManager player)
    {
        ScoreboardItemManager item = Instantiate(scoreBoardItem, container).GetComponent<ScoreboardItemManager>();
        item.Initialize(player);
    }

    public void RemoveScoreboardItem(PlayerManager player)
    {
        Destroy(items[player].gameObject);
        items.Remove(player);
    }
}
