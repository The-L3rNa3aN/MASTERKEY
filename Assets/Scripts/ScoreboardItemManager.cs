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
    public Text avgLpmText;

    public void Initialize(PlayerManager player)
    {
        //var avgLpmToInt = (int)(player.GetComponentInChildren<PlayTerminalManager>().AverageLPM());         //Implicit conversion of the float Average LPM to int.

        usernameText.text = player.playerTag;
        killsText.text = player.kills.ToString();
        deathsText.text = player.deaths.ToString();
        //avgLpmText.text = avgLpmToInt.ToString();
    }
}
