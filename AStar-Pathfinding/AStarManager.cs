using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Xml;
using UnityEngine;
using UnityEngine.UIElements;

public class AStarManager : MonoBehaviour
{
    // Start is called before the first frame update
    public static AStarManager instance;
    private Dictionary<Vector2, Node> nodesWithCoords;
    private bool isAlreadyGeneratingPath = false;
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask targetLayerSight;
    void Awake()
    {
       instance = this;
    }

    private void Start()
    {
        nodesWithCoords = new Dictionary<Vector2, Node>();
        foreach (Node node in GameObject.FindObjectsOfType<Node>())
        {
            nodesWithCoords.Add(node.transform.position, node);
        }
        SetNodeConnections();
    }

    private void SetNodeConnections()
    {
        List<Node> nodeList = nodesWithCoords.Values.ToList();
        foreach (Node node in nodeList)
        {
           int count = 0;
            for (float XCord = node.transform.position.x - 1;XCord <= node.transform.position.x + 1; XCord++)
            {
                for (float YCord = node.transform.position.y - 1; YCord <= node.transform.position.y + 1; YCord++)
                {
                    count++;
                    if (count == 1 || count == 3 || count == 7 || count == 9)
                    {
                        //continue;
                    }
                    Vector2 connectedNodePos = new Vector2(XCord, YCord);
                    nodesWithCoords.TryGetValue(connectedNodePos, out Node connectedNode);
                    if( connectedNode != null && connectedNode != node)
                    {
                        node.connections.Add(connectedNode);
                    }
                }
            }
        }
    }
    public List<Node> GeneratePath(Vector2 startPos, Vector2 endPos)
    {
        Node startNode = GetClosestNode(startPos);
        Node endNode = GetClosestNode(endPos);
        return GeneratePath(startNode, endNode);
    }



    public List<Node> GeneratePath(Node startNode, Node endNode)
    {
        if (isAlreadyGeneratingPath)
        {
            return null;
        }
        isAlreadyGeneratingPath = true;
        List<Node> openSet = new List<Node>();
        HashSet<Node> closeSet = new HashSet<Node>();
        if (startNode == null || endNode == null)
        {
            isAlreadyGeneratingPath = false;
            return null;
        }
        List<Node> allNodesList = nodesWithCoords.Values.ToList();
        foreach (Node node in allNodesList)
        {
            node.gScore = float.MaxValue;
        }
        startNode.gScore = 0;
        startNode.hScore = Vector2.Distance(startNode.transform.position, endNode.transform.position);
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            int lowestF = 0;
            for (int i = 0; i < openSet.Count; i++)
            {
                if (openSet[i].FScore() < openSet[lowestF].FScore())
                {
                    lowestF = i;
                }
            }
            Node currentNode = openSet[lowestF];
            openSet.Remove(currentNode);
            closeSet.Add(currentNode);

            if (currentNode == endNode)
            {
                List<Node> path = new List<Node>();
                path.Insert(0, endNode);
                while (currentNode != startNode)
                {
                    currentNode = currentNode.cameFrom;
                    path.Add(currentNode);
                }
                path.Reverse();
                if (path.Count > 1)
                {
                    if (Vector2.Distance(startNode.transform.position, path[1].transform.position) - Vector2.Distance(path[0].transform.position, path[1].transform.position) < 0.1)
                    {
                        path.RemoveAt(0);
                    }
                }
                isAlreadyGeneratingPath = false;
                Debug.Log("FINISHED PATHFINDING");
                return path;


            }
            foreach (Node connectedNode in currentNode.connections)
            {
                if (closeSet.Contains(connectedNode))
                {
                    continue;
                }
                float heldGScore = currentNode.gScore + Vector2.Distance(currentNode.transform.position, connectedNode.transform.position);
                if (heldGScore < connectedNode.gScore)
                {
                    connectedNode.cameFrom = currentNode;
                    connectedNode.gScore = heldGScore;
                    connectedNode.hScore = Vector2.Distance(connectedNode.transform.position, endNode.transform.position);
                    if (!openSet.Contains(connectedNode))
                    {
                        openSet.Add(connectedNode);
                    }
                }
            }
        }

        isAlreadyGeneratingPath = false;
        return null;
    }


    public Node GetClosestNode(Vector2 pos)
    {
        float normalizedX;
        float normalizedY;
        if (pos.x>=0)
        {
            normalizedX = (int)pos.x + 0.5f;
        }
        else
        {
            normalizedX = (int)pos.x - 0.5f;
        }
        if (pos.y >= 0)
        {
            normalizedY = (int)pos.y + 0.5f;
        }
        else
        {
            normalizedY = (int)pos.y - 0.5f;
        }
        Vector2 normalizedPos = new Vector2(normalizedX, normalizedY);
        nodesWithCoords.TryGetValue(normalizedPos, out Node node);

        return node;
    }

    public Node GetRandomNode()
    {
        int randNum = Random.Range(0, nodesWithCoords.Count);
        return nodesWithCoords.Values.ElementAt(randNum);
    }

    private List<Node> GetSuitableNodes(bool inSight)
    {
        List<Node> suitableNodes = new List<Node>();
        List<Node> allNodesList = nodesWithCoords.Values.ToList();
        foreach (Node node in allNodesList)
        {
            if (NodeHasSightWithPlayer(node) == inSight)
            {
                suitableNodes.Add(node);
            }
        }
        return suitableNodes;
    }

    
    public Node GetRandomNodeInSight()
    {
        List<Node> suitableNodes = GetSuitableNodes(true);
        if(suitableNodes.Count != 0)
        {
            int rand = Random.Range(0, suitableNodes.Count);
            return suitableNodes.ElementAt(rand);
        }
        else
        {
            return null;
        }
    }

    public Node GetRandomNodeOutOfSight() 
    {
        List<Node> suitableNodes = GetSuitableNodes(false);
        if (suitableNodes.Count != 0)
        {
            int rand = Random.Range(0, suitableNodes.Count);
            return suitableNodes.ElementAt(rand);
        }
        else
        {
            return null;
        }
    }


    public bool NodeHasSightWithPlayer(Node node)
    {
        RaycastHit2D sightRaycast = Physics2D.Raycast(node.transform.position, (MainManager.instance.playerPos - transform.position).normalized, maxDistance, targetLayerSight);
        Debug.DrawLine(transform.position, transform.position + (MainManager.instance.playerPos - transform.position).normalized * maxDistance, Color.green);
        return sightRaycast.collider.CompareTag("Player");
    }


}
