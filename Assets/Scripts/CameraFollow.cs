using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform playerTransform;
    public float lockOnSpeed = 0.2f;
    public Vector3 offset;

    void Start()
    {
        playerTransform = GameObject.FindObjectOfType<PlayerMovement>().transform;
    }

    private void FixedUpdate()
    {
        Vector3 desiredPosition = playerTransform.position + offset;
        Vector3 smoothedPostion = Vector3.Lerp(transform.position, desiredPosition, lockOnSpeed);
        transform.position = smoothedPostion;

        //transform.LookAt(playerTransform); Fait une vue 3d cursed
    }

}
