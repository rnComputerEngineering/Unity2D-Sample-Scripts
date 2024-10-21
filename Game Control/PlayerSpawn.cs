using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private GameObject cameraPrefab;
    private void Awake()
    {
        Debug.Log("Start");
        GameObject player = MainManager.instance.characterPrefab;
        GameObject instantiatedPlayer =  Instantiate(player,transform);
        GameObject instantiatedCamera = Instantiate(cameraPrefab,transform);
        CameraFollow cameraFollow = instantiatedCamera.GetComponent<CameraFollow>();
        if (cameraFollow != null)
        {
            cameraFollow.SetPlayer(instantiatedPlayer);
        }
        else
        {
            Debug.LogError("CameraFollow component not found on camera prefab.");
        }

    }

}

