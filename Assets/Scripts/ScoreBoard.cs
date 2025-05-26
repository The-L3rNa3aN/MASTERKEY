using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    public Transform container;
    public GameObject scoreBoardItem;
    //Dictionary<ScoreboardItemManager, PlayerManager> itemsDict = new Dictionary<ScoreboardItemManager, PlayerManager>();
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
            if (gameObject.transform.GetChild(i).name != "ScoreBoardDirectory")
            {
                Destroy(gameObject.transform.GetChild(i).gameObject);
            }
        }

        playerList.Clear();
    }

    /*private void Update()
    {
        var searchPlayer = FindObjectsOfType<PlayerManager>();

        foreach (PlayerManager player in searchPlayer)               //Adding players to the list.
        {
            if (!itemsDict.ContainsValue(player))
            {
                ScoreboardItemManager item = Instantiate(scoreBoardItem, container).GetComponent<ScoreboardItemManager>();
                item.Initialize(player);
                itemsDict[item] = player;
            }
        }

        foreach (var item in itemsDict)
        {
            if (item.Value == null)
            {
                //sim = item.Key;
                //Destroy(itemsDict[item.Key]);
                itemsDict.Remove(item.Key);
            }
        }
    }*/
}