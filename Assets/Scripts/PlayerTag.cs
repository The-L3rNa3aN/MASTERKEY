using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerTag : NetworkBehaviour
{
    public Text playerName;

    private void Start()
    {
        if(isLocalPlayer) { return; }

        playerName.text = gameObject.GetComponent<PlayerManager>().playerTag;
    }
}
