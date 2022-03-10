using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] Transform container;
    [SerializeField] GameObject scoreBoardItem;

    public List<PlayerManager> playerList = new List<PlayerManager>();
    public Dictionary<PlayerManager, ScoreboardItemManager> items = new Dictionary<PlayerManager,ScoreboardItemManager>();

    private void Start() => gameObject.SetActive(false);

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
        if(!items.ContainsKey(player)) { items.Add(player, item); }
    }

    public void RemoveScoreboardItems()
    {
        //var scoreboardItems = GameObject.FindGameObjectsWithTag("Scoreboard Items");
        //foreach (GameObject item in scoreboardItems) { Destroy(item); }

        for(var i = gameObject.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(gameObject.transform.GetChild(i).gameObject);
        }
    }

    /*public void RemoveScoreboardItem(PlayerManager player)
    {
        playerList.Clear();
        Destroy(items[player].gameObject);
        items.Remove(player);
    }*/
}
