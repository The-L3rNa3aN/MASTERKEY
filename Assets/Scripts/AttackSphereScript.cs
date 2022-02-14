using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AttackSphereScript : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.GetComponent<NetworkIdentity>().isLocalPlayer == false && other.gameObject.GetComponent<PlayerManager>())
        {
            other.gameObject.GetComponent<PlayerManager>().health -= 1;
        }
    }
}
