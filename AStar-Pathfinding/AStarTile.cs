using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "New AStar Tile", menuName = "Tiles/AStar Tile")]

public class AStarTile :Tile
{
    [SerializeField] private GameObject node;
    void Start()
    {
        Debug.Log("Spawning");
        Instantiate(node);
    }

}
