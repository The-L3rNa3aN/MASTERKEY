using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayertagManager : MonoBehaviour
{
    public Text playerName;

    public void InitializeNameTag(PlayerManager player)
    {
        playerName.text = player.playerTag;

        if (player.health == 1) { playerName.color = Color.red; }
        if (player.health == 2) { playerName.color = Color.yellow; }
        if (player.health == 3) { playerName.color = Color.green; }
        if (player.health >= 4) { playerName.color = Color.cyan; }
    }
}