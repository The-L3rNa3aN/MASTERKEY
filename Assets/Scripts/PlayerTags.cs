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
    Dictionary<Image, PlayerManager> playerDict = new Dictionary<Image, PlayerManager>();

    private void Start()
    {
        //if(!isLocalPlayer) { return; }
    }

    private void LateUpdate()
    {
        ApplyNameTags(FindObjectsOfType<PlayerManager>());
    }

    private void ApplyNameTags(PlayerManager[] playerManagers)
    {
        var canvasRect = canvas.GetComponent<RectTransform>();
        foreach (PlayerManager player in playerManagers)
        {
            if(!playerDict.ContainsValue(player))
            {
                image = Instantiate(nameTagObject, canvasRect).GetComponent<Image>();
                playerDict.Add(image, player);
            }
        }

        foreach (var item in playerDict)
        {
            if(item.Value == null)
            {
                playerDict.Remove(item.Key);
            }

            Vector2 viewPortPos = cam.WorldToViewportPoint(item.Value.transform.position);
            Vector2 playerScreenPos = new Vector2((viewPortPos.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f), (viewPortPos.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f));
            item.Key.rectTransform.anchoredPosition = playerScreenPos;
        }
    }
}