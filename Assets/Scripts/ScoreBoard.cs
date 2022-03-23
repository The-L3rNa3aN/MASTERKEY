using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    public Transform container;
    public GameObject scoreBoardItem;
    Dictionary<ScoreboardItemManager, PlayerManager> itemsDict = new Dictionary<ScoreboardItemManager, PlayerManager>();

    private void Start() => gameObject.SetActive(false);

    private void Update()
    {
        var searchPlayer = FindObjectsOfType<PlayerManager>();
        ScoreboardItemManager sim = default;

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
            if (item.Value != null)
            {
                Debug.Log("Not null.");
                //sim = item.Key;
            }
            else
            {
                Debug.Log("Null.");
                sim = item.Key;
            }
        }

        if (sim != null)
        {
            Destroy(itemsDict[sim]);
            itemsDict.Remove(sim);
        }
    }
}