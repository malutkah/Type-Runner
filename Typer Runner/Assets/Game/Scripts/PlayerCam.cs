using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public Transform target;
    public Vector3 Offset;

    private readonly float smoothSpeed = 5.2f;


    private void LateUpdate()
    {
        Vector3 desiredPostition = target.position + Offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPostition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}