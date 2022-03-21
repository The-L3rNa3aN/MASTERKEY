using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VersusPlayerScript : MonoBehaviour
{
    public Dictionary<PlayerManager, int> versusKills = new Dictionary<PlayerManager, int>();

    private void Update()
    {
        InitializeDictionary(FindObjectsOfType<PlayerManager>());

        foreach(var item in versusKills)
        {
            if(item.Key == null)
            {
                versusKills.Remove(item.Key);
            }
        }
    }

    private void InitializeDictionary(PlayerManager[] players)
    {
        foreach(PlayerManager plr in players)
        {
            if(!versusKills.ContainsKey(plr))
            {
                versusKills.Add(plr, 0);
            }
        }
    }
}
