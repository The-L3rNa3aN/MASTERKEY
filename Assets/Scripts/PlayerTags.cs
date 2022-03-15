using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerTags : NetworkBehaviour
{
    public GameObject nameTagObject;
    public Canvas canvas;
    public Camera cam;
    Image image;

    private void Start()
    {
        //if(!isLocalPlayer) { return; }
    }

    private void LateUpdate()
    {
        myOldMethod(FindObjectsOfType<PlayerManager>());
    }

    private void myOldMethod(PlayerManager[] playerManagers)
    {
        var canvasRect = canvas.GetComponent<RectTransform>();
        foreach (PlayerManager player in playerManagers)
        {
            image = Instantiate(nameTagObject, canvasRect).GetComponent<Image>();
            Vector2 viewPortPos = cam.WorldToViewportPoint(player.transform.position);
            Vector2 playerScreenPos = new Vector2((viewPortPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f), (viewPortPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
            image.rectTransform.anchoredPosition = playerScreenPos;
        }
    }
}