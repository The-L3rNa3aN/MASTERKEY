using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayertagToWorld : MonoBehaviour
{
    public GameObject nameTagObject;
    public Canvas playerCanvas;
    public Camera cam;
    protected Image image;
    protected Text playerName;
    public List<PlayerManager> idList = new List<PlayerManager>();

    private void Update()
    {
        AddPlayersToList(FindObjectsOfType<PlayerManager>());
    }

    private void AddPlayersToList(PlayerManager[] player)
    {
        foreach (PlayerManager playerManager in player)
        {
            if (!idList.Contains(playerManager))
            {
                idList.Add(playerManager);
                image = Instantiate(nameTagObject, playerCanvas.transform).GetComponent<Image>();
                image.transform.position = cam.WorldToScreenPoint(playerManager.transform.position);
            }
        }
    }
}