using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AttackSphereScript : MonoBehaviour
{
    public GameObject player;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<NetworkIdentity>().isLocalPlayer == false)
        {
            other.gameObject.GetComponent<PlayerManager>().Damage(1);
            other.gameObject.GetComponent<PlayerManager>().attackerPos = player.transform.position;    //Vector3 variable required for calculating knockback.
        }
    }
}
