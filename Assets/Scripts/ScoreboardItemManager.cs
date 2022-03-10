using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class ScoreboardItemManager : MonoBehaviour
{
    public Text usernameText;
    public Text killsText;
    public Text deathsText;

    public void Initialize(PlayerManager player)
    {
        usernameText.text = player.playerTag;
        killsText.text = player.kills.ToString();
        deathsText.text = player.deaths.ToString();
    }
}
