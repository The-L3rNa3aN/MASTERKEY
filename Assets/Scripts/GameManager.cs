using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : MonoBehaviour
{
    NetworkManager networkManager;
    void Start()
    {
        networkManager = GetComponent<NetworkManager>();
    }

    void Update()
    {
        
    }
}
