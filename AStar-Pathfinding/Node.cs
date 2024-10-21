using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    // Start is called before the first frame update
    public Node cameFrom;
    public List<Node> connections;
    public bool showGizmo = true;

    public float gScore = float.MaxValue;
    public float hScore = 0;

    public float FScore()
    {
        return gScore + hScore;
    }


    private void OnDrawGizmos()
    {       
        if(!showGizmo)
        {
            return;
        }
        Gizmos.color = Color.blue;
        if (connections.Count > 0)
        {
            foreach (Node connectedNode in connections)
            {
                Gizmos.DrawLine(transform.position, connectedNode.transform.position);
            }
        }
    }
}

 