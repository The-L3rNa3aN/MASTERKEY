using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObstruction : MonoBehaviour
{
    Material material;

    private void Start()
    {
        material = GetComponent<Material>();
    }

    public void OnRayHitEnter()
    {

    }

    public void OnRayHitExit()
    {

    }
}
