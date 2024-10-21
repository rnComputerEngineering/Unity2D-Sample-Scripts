using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private GameObject player;  
    [SerializeField] private Vector3 offset;     
    [SerializeField] private float smoothSpeed = 0.125f;

    public void SetPlayer(GameObject playerRef)
    {
        player = playerRef;
        Debug.Log(player);
    }
    void FixedUpdate()
    {
        if (player != null)
        {
            Vector3 desiredPosition = player.transform.position + offset;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
        }
    }
}
