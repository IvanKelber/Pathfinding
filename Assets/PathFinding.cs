using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    Grid grid;

    public Transform seeker;
    public Transform target;

    private void Awake() {
        grid = GetComponent<Grid>();
    }

    private void Update() {
        FindPath(seeker.position, target.position);
    }

    public void FindPath(Vector3 startPosition, Vector3 targetPosition) {
        Node startNode = grid.NodeFromWorldPosition(startPosition);
        Node targetNode = grid.NodeFromWorldPosition(targetPosition);

        List<Node> openSet = new List<Node>();
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while(openSet.Count > 0) {
            Node currentNode = openSet[0];
            foreach(Node n in openSet) {
                if(n.fCost < currentNode.fCost ||  n.fCost == currentNode.fCost && n.hCost < currentNode.hCost) {
                    currentNode = n;
                }
            }
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if(currentNode == targetNode) {
                grid.path = RetracePath(startNode, targetNode);
                return;
            }

            foreach(Node neighbor in grid.FindNeighbors(currentNode)) {
                if(!neighbor.walkable || closedSet.Contains(neighbor)) {
                    continue;
                }

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if(newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;
                    if(!openSet.Contains(neighbor)) 
                        openSet.Add(neighbor);
                }
            }
        }
    }

    List<Node> RetracePath(Node start, Node target) {
        List<Node> path = new List<Node>();

        Node current = target;
        path.Add(current);
        while(current != start) {
            current = current.parent;
            path.Add(current);
        }
        path.Reverse();
        return path;
    }

    int GetDistance(Node a, Node b) {
        int y = Mathf.Abs(a.gridY - b.gridY);
        int x = Mathf.Abs(a.gridX - b.gridX);

        if(y > x) {
            int tmp = x;
            x = y;
            y = tmp;
        } 

        return 14 * y + 10 * (x-y);
    }
}
