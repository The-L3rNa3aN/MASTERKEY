using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreBoard : MonoBehaviour
{
    public Transform container;
    public GameObject scoreBoardItem;
    public List<PlayerManager> playerList = new List<PlayerManager>();

    private void Start() => gameObject.SetActive(false);

    public void FindPlayers()
    {
        var searchPlayer = FindObjectsOfType<PlayerManager>();
        for (int i = 0; i < searchPlayer.Length; i++) { playerList.Add(searchPlayer[i]); }

        foreach (PlayerManager player in playerList) { AddScoreboardItem(player); }
    }

    public void AddScoreboardItem(PlayerManager player)
    {
        ScoreboardItemManager item = Instantiate(scoreBoardItem, container).GetComponent<ScoreboardItemManager>();
        item.Initialize(player);
    }

    public void RemoveScoreboardItems()
    {
        for (var i = gameObject.transform.childCount - 1; i >= 0; i--)
        {
            if(gameObject.transform.GetChild(i).name != "ScoreBoardDirectory")
            {
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }
    }
}