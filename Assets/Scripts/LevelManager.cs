using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class LevelManager : MonoBehaviour
{
    GameObject networkManager;

    private void Awake()
    {
        networkManager = GameObject.Find("NetworkManager");
        var test = GameObject.FindGameObjectsWithTag("Spawn Point");
        for(int i = 0; i < test.Length; i++)
        {
            networkManager.GetComponent<GameManager>().spawns.Add(test[i]);                     //Adds all the spawnpoints of the online level into the current GameManager instance.
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
