using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PowerupScript : MonoBehaviour
{
    public Collider col;
    public GameObject GFX;
    public float respawnTime;

    [Header("Powerup Identities")]
        public bool aide;                   //Small health pickup. Offers one health point.
        public bool vitalis;                //Large health pickup. Offers three health points.
        public bool corruptus;              //Allows the player to deal double damage at the cost of being twice as vulnerable.
        public bool vaengr;                 //Refreshes the player's dash ability.
        public bool escren;                 //Offers temporary protection that shatters in one hit. The player is not subject to knockback however.

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        var consumer = other.gameObject;
        if(consumer.GetComponent<PlayerManager>() && GFX.activeInHierarchy == true)
        {
            if (aide)
            {
                consumer.GetComponent<PlayerManager>().CmdGiveHealth(1);
            }

            if (vitalis)
            {
                consumer.GetComponent<PlayerManager>().CmdGiveHealth(3);
            }

            if (corruptus)
            {
                consumer.GetComponent<PlayerManager>().corruptus = true;
            }

            if (vaengr)
            {
                //Refresh the Dash ability.
            }

            if (escren)
            {
                consumer.GetComponent<PlayerManager>().escren = true;
            }
            StartCoroutine(Respawn(respawnTime));
        }
    }

    IEnumerator Respawn(float time)
    {
        {
            //col.enabled = false;
            GFX.SetActive(false);
        }
        yield return new WaitForSeconds(time);
        {
            //col.enabled = true;
            GFX.SetActive(true);
        }
    }
}
