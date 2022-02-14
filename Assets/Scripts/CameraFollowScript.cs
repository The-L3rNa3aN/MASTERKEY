using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    public float smooth = 0.125f;

    void Update()
    {
        Vector3 desiredPos = target.position + offset;
        Vector3 smoothPos = Vector3.Lerp(transform.position, desiredPos, smooth);
        transform.position = smoothPos;
    }
}
