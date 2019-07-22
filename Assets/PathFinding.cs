using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathFinding : MonoBehaviour
{
    PathRequestManager requestManager;
    Grid grid;


    private void Awake() {
        grid = GetComponent<Grid>();
        requestManager = GetComponent<PathRequestManager>();
    }

    public void StartFindPath(Vector3 start, Vector3 targetPosition) {
        StartCoroutine(FindPath(start, targetPosition));
    }

    IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition) {
        Node startNode = grid.NodeFromWorldPosition(startPosition);
        Node targetNode = grid.NodeFromWorldPosition(targetPosition);

        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;
        if(startNode.walkable && targetNode.walkable) {
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
                    pathSuccess = true;
                    break;
                }

                // Calculate the fCost of each walkable neighbor with respect to the current node
                foreach(Node neighbor in grid.FindNeighbors(currentNode)) {
                    if(!neighbor.walkable || closedSet.Contains(neighbor)) {
                        continue;
                    }
                    int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor) + neighbor.movementPenalty;
                    if(newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor)) {
                        neighbor.gCost = newCostToNeighbor;
                        neighbor.hCost = GetDistance(neighbor, targetNode);
                        neighbor.parent = currentNode;
                        if(!openSet.Contains(neighbor)) {
                            openSet.Add(neighbor);
                            // UnityEngine.Debug.Log("Adding neighbor: " + neighbor.fCost + " _ " +neighbor.HeapIndex + " Coordinates: " + neighbor.gridX + "," + neighbor.gridY);
                            // openSet.Print();
                        } else {
                            openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }
        }
        yield return null;
        if(pathSuccess) {
            waypoints = RetracePath(startNode, targetNode);
        }
        requestManager.FinishedProcessingPath(waypoints, pathSuccess);
    }

    Vector3[] RetracePath(Node start, Node target) {
        List<Node> path = new List<Node>();

        Node current = target;
        path.Add(current);
        while(current != start) {
            current = current.parent;
            path.Add(current);
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path) {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector3.zero;

        for(int i = 1; i < path.Count; i++) {
            Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
            if(directionNew != directionOld) {
                waypoints.Add(path[i].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
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
