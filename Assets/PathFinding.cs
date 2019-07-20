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

        Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
        HashSet<Node> closedSet = new HashSet<Node>();

        // Add startNode to set.
        startNode.hCost = GetDistance(startNode, targetNode);
        targetNode.gCost = GetDistance(startNode, targetNode);
        openSet.Add(startNode);

        while(openSet.Count > 0) {
            // Find the node with lowest fCost in the open set
            Node currentNode = openSet.Pop();
            // UnityEngine.Debug.Log("Starting new round with " + currentNode.gridX + ", " + currentNode.gridY +" fcost: " + currentNode.fCost);
            // openSet.Print();

            closedSet.Add(currentNode);

            // If we have reached the target then we are done.
            if(currentNode == targetNode) {
                grid.path = RetracePath(startNode, targetNode);
                return;
            }

            // Calculate the fCost of each walkable neighbor with respect to the current node
            foreach(Node neighbor in grid.FindNeighbors(currentNode)) {
                if(!neighbor.walkable || closedSet.Contains(neighbor)) {
                    continue;
                }
                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if(newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;
                    if(!openSet.Contains(neighbor)) {
                        openSet.Add(neighbor);
                        // UnityEngine.Debug.Log("Adding neighbor: " + neighbor.fCost + " _ " +neighbor.HeapIndex + " Coordinates: " + neighbor.gridX + "," + neighbor.gridY);
                        // openSet.Print();
                    }
                }
            }
            // UnityEngine.Debug.Log("End of round fCost: " + currentNode.fCost);
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
