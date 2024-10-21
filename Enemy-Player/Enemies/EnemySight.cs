using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySight : MonoBehaviour
{
    public bool isInSight = false;
    public float maxDistance = 100f;
    [SerializeField] private LayerMask targetLayerSight;
    [SerializeField] private LayerMask targetLayerTouch;
    private CapsuleCollider2D capsuleCollider;
    [SerializeField] private float wallDetectRange;
    public bool leftWallCheck = false;
    public bool rightWallCheck = false;
    public bool upWallCheck = false;
    public bool downWallCheck = false;


    // Update is called once per frame    
    private void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }
    void FixedUpdate()
    {
        RaycastHit2D sightRaycast = Physics2D.Raycast(transform.position, (MainManager.instance.playerPos - transform.position).normalized, maxDistance, targetLayerSight);
        Debug.DrawLine(transform.position, transform.position + (MainManager.instance.playerPos - transform.position).normalized * maxDistance, Color.red);
        if (sightRaycast.collider.CompareTag("Player"))
        {
            if (!isInSight)
            {
                GetComponent<Enemy>().OnSightGained();
                isInSight = true;
            }
        }
        else
        {
            if (isInSight)
            {
                GetComponent<Enemy>().OnSightLost();
                isInSight = false;
            }
        }
        Debug.Log(isInSight);
        leftWallCheck = RaycastOnCollider(0);
        rightWallCheck = RaycastOnCollider(1);
        upWallCheck = RaycastOnCollider(2);
        downWallCheck = RaycastOnCollider(3);



    }

    private bool RaycastOnCollider(int direction) // 0 left, 1 right , 2 up, 3 down
    { // fix this mess later
        // LOOK AWAY PLEASE
        if (direction == 0)
        {
            Vector3 upCheckLocation = new Vector3(transform.position.x, transform.position.y + capsuleCollider.size.y / 2);
            Vector3 downCheckLocation = new Vector3(transform.position.x, transform.position.y - capsuleCollider.size.y / 2);
            RaycastHit2D horizontalUpRaycast = Physics2D.Raycast(upCheckLocation, Vector2.left, wallDetectRange, targetLayerTouch);
            Debug.DrawLine(upCheckLocation, upCheckLocation + Vector3.left * wallDetectRange, Color.red);
            RaycastHit2D horizontalDownRaycast = Physics2D.Raycast(downCheckLocation, Vector2.left, wallDetectRange, targetLayerTouch);
            Debug.DrawLine(downCheckLocation, downCheckLocation + Vector3.left * wallDetectRange, Color.red);
            return horizontalUpRaycast.collider != null || horizontalDownRaycast.collider != null;
        }
        else if (direction == 1)
        {
            Vector3 upCheckLocation = new Vector3(transform.position.x, transform.position.y + capsuleCollider.size.y / 2);
            Vector3 downCheckLocation = new Vector3(transform.position.x, transform.position.y - capsuleCollider.size.y / 2);
            RaycastHit2D horizontalUpRaycast = Physics2D.Raycast(upCheckLocation, Vector2.right, wallDetectRange, targetLayerTouch);
            Debug.DrawLine(upCheckLocation, upCheckLocation + Vector3.right * wallDetectRange, Color.red);
            RaycastHit2D horizontalDownRaycast = Physics2D.Raycast(downCheckLocation, Vector2.right, wallDetectRange, targetLayerTouch);
            Debug.DrawLine(downCheckLocation, downCheckLocation + Vector3.right * wallDetectRange, Color.red);
            return horizontalUpRaycast.collider != null || horizontalDownRaycast.collider != null;
        }
        else if(direction == 2)
        {
            Vector3 leftCheckLocation = new Vector3(transform.position.x - capsuleCollider.size.x / 2, transform.position.y);
            Vector3 rightCheckLocation = new Vector3(transform.position.x + capsuleCollider.size.x / 2, transform.position.y);
            RaycastHit2D verticalLeftRaycast = Physics2D.Raycast(leftCheckLocation, Vector2.up, wallDetectRange, targetLayerTouch);
            Debug.DrawLine(leftCheckLocation, leftCheckLocation + Vector3.up * wallDetectRange, Color.red);
            RaycastHit2D verticalRightRaycast = Physics2D.Raycast(rightCheckLocation, Vector2.up, wallDetectRange, targetLayerTouch);
            Debug.DrawLine(rightCheckLocation, rightCheckLocation + Vector3.up * wallDetectRange, Color.red);
            return verticalLeftRaycast.collider != null || verticalRightRaycast.collider != null;
        }
        else if(direction == 3)
        {
            Vector3 leftCheckLocation = new Vector3(transform.position.x - capsuleCollider.size.x / 2, transform.position.y);
            Vector3 rightCheckLocation = new Vector3(transform.position.x + capsuleCollider.size.x / 2, transform.position.y);
            RaycastHit2D verticalLeftRaycast = Physics2D.Raycast(leftCheckLocation, Vector2.down, wallDetectRange, targetLayerTouch);
            Debug.DrawLine(leftCheckLocation, leftCheckLocation + Vector3.down * wallDetectRange, Color.red);
            RaycastHit2D verticalRightRaycast = Physics2D.Raycast(rightCheckLocation, Vector2.down, wallDetectRange, targetLayerTouch);
            Debug.DrawLine(rightCheckLocation, rightCheckLocation + Vector3.down * wallDetectRange, Color.red);
            return verticalLeftRaycast.collider != null || verticalRightRaycast.collider != null;
        }
        else
        {
            return false;
        }
    }

}


