using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    /// <summary>
    /// "target" throws an error when disconnecting. Doesn't break the game in any way but yeah, we need to do something about it.
    /// </summary>
    public Transform target;
    private Transform oldObs = default;
    public Vector3 offset;
    public float smooth = 0.125f;
    //public LayerMask mask;

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
                Color objColor = hit.transform.GetComponent<Renderer>().material.color;
                float fade = objColor.a - (5 * Time.deltaTime);

                objColor = new Color(objColor.r, objColor.g, objColor.b, fade);
                hit.transform.GetComponent<Renderer>().material.color = objColor;
            }
            else if(oldObs == default)
            {
                return;
            }
            else
            {
                Color objColor = oldObs.GetComponent<Renderer>().material.color;
                float fade = objColor.a + (7.5f * Time.deltaTime);

                objColor = new Color(objColor.r, objColor.g, objColor.b, fade);
                oldObs.GetComponent<Renderer>().material.color = objColor;

                if(fade >= 1) { oldObs = default; }
            }
        }
    }
}