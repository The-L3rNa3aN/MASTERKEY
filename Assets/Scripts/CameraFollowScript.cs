using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraFollowScript : MonoBehaviour
{
    /// <summary>
    /// "target" throws an error when disconnecting. Doesn't break the game in any way but yeah, we need to do something about it.
    /// </summary>
    public Transform target;
    [SerializeField] private Transform oldObs = default;
    public Vector3 offset;
    public float smooth = 0.125f;

    public List<PlayerManager> playerList = new List<PlayerManager>();

    void Update()
    {
        Ray ray = new Ray(transform.position, target.transform.position - transform.position);
        RaycastHit hit;

        #region Smooth Camera Chase
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smooth);
        transform.position = smoothPos;
        #endregion

        Debug.DrawRay(transform.position, target.transform.position - transform.position, Color.green);
        if(Physics.Raycast(ray, out hit))
        {
            if(hit.transform.tag == "World")
            {
                ///For some reason, larger, more wider objects fade on slower than usual.
                //MAKE SURE THE SURFACE TYPE OF THE OBJECT IS SET TO "TRANSPARENT".

                oldObs = hit.transform;
                //Color32 objColor = hit.transform.GetComponent<Renderer>().material.color;
                //float fade = objColor.a - (10f * Time.deltaTime);

                //objColor = new Color32(objColor.r, objColor.g, objColor.b, (byte)fade);
                //hit.transform.GetComponent<Renderer>().material.color = objColor;

                oldObs.GetComponent<Renderer>().material.DOFade(0, 1f);


            }
            else if(oldObs == default)                  //Prevents an error that appears when the ray no longer hits any world geometry. Works with both "DEFAULT" and "NULL".
            {
                return;
            }
            else
            {
                //Color32 objColor = oldObs.GetComponent<Renderer>().material.color;
                //float fade = objColor.a + (10f * Time.deltaTime);

                //objColor = new Color32(objColor.r, objColor.g, objColor.b, (byte)fade);
                //oldObs.GetComponent<Renderer>().material.color = objColor;

                //if(fade >= 255) { oldObs = default; }

                if(hit.transform != oldObs)
                {
                    oldObs.GetComponent<Renderer>().material.DOFade(1, 1f).OnComplete(() => { oldObs = default; });
                }
            }
        }

        if(target.GetComponent<PlayerManager>().matchOver == true)
        {
            var players = FindObjectsOfType<PlayerManager>();

            foreach(PlayerManager player in players)
            {
                if(!playerList.Contains(player))
                {
                    playerList.Add(player);
                }
            }

            
        }
    }
}