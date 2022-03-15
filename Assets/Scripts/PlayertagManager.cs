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
    }
}
