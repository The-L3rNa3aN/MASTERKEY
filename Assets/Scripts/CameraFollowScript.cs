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

    [SerializeField] private List<ObstructingObjects> currentlyObstructing = new List<ObstructingObjects>();
    [SerializeField] private List<ObstructingObjects> alreadyTransparent = new List<ObstructingObjects>();

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
            else if(oldObs == default)                  //Prevents an error that appears when the ray no longer hits any world geometry. Works with both "DEFAULT" and "NULL".
            {
                return;
            }
            else
            {
                Color objColor = oldObs.GetComponent<Renderer>().material.color;
                float fade = objColor.a + (10f * Time.deltaTime);

                objColor = new Color(objColor.r, objColor.g, objColor.b, fade);
                oldObs.GetComponent<Renderer>().material.color = objColor;

                if(fade >= 1) { oldObs = default; }
            }
        }

        //GetAllObjectsInTheWay();
        //MakeObjectsTransparent();
        //MakeObjectsSolid();
    }

    private void GetAllObjectsInTheWay()
    {
        currentlyObstructing.Clear();

        float cameraPlayerDistance = Vector3.Magnitude(transform.position - target.position);
        Ray ray1_forward = new Ray(transform.position, target.position - transform.position);
        Ray ray1_backward = new Ray(target.position, transform.position - target.position);
        var hits1_forward = Physics.RaycastAll(ray1_forward, cameraPlayerDistance);
        var hits1_backward = Physics.RaycastAll(ray1_backward, cameraPlayerDistance);

        foreach (var hit in hits1_forward)
        {
            if(hit.collider.gameObject.TryGetComponent(out ObstructingObjects obstruct))
            {
                if(!currentlyObstructing.Contains(obstruct))
                {
                    currentlyObstructing.Add(obstruct);
                }
            }
        }

        foreach (var hit in hits1_backward)
        {
            if (hit.collider.gameObject.TryGetComponent(out ObstructingObjects obstruct))
            {
                if (!currentlyObstructing.Contains(obstruct))
                {
                    currentlyObstructing.Add(obstruct);
                }
            }
        }
    }

    private void MakeObjectsTransparent()
    {
        for(int i = 0; i < currentlyObstructing.Count; i++)
        {
            ObstructingObjects obstructing = currentlyObstructing[i];

            if(!alreadyTransparent.Contains(obstructing))
            {
                obstructing.ShowTransparent();
                alreadyTransparent.Add(obstructing);
            }
        }
    }

    private void MakeObjectsSolid()
    {
        for(int i = alreadyTransparent.Count - 1; i >= 0; i--)
        {
            ObstructingObjects obstructing = alreadyTransparent[i];

            if (!currentlyObstructing.Contains(obstructing))
            {
                obstructing.ShowSolid();
                alreadyTransparent.Remove(obstructing);
            }
        }
    }
}