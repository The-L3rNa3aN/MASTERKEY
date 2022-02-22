using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PowerupScript : NetworkBehaviour
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
        if(consumer.GetComponent<PlayerManager>())          //Throws up an error after the collider deactivates.
        {
            /*if(aide)
            {
                consumer.GetComponent<PlayerManager>().health += 1;
            }*/
            CmdPlayerEffects(consumer);
            StartCoroutine(Respawn(respawnTime));
        }
    }

    IEnumerator Respawn(float time)
    {
        {
            col.enabled = false;
            GFX.SetActive(false);
        }
        yield return new WaitForSeconds(time);
        {
            col.enabled = true;
            GFX.SetActive(true);
        }
    }

    public void CmdPlayerEffects(GameObject playerObject)
    {
        if (aide)
        {
            playerObject.GetComponent<PlayerManager>().health += 1;
        }
    }
}
