using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGrid = false;
    public LayerMask unwalkableMask;
    public Vector2 worldScale;
    public Transform player;
    public Transform target;
    public float nodeRadius;

    [Range(0,1)]
    public float offset = .1f;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;


    private void Awake() {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(worldScale.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(worldScale.y/nodeDiameter);
        CreateGrid();
    }

    void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 bottomLeft = transform.position - (Vector3.right * worldScale.x/2) - (Vector3.forward * worldScale.y/2);

        for(int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {
                Vector3 worldPosition = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPosition, nodeRadius, unwalkableMask);
                grid[x,y] = new Node(walkable, worldPosition, x, y);
            }
        }
    }

    public int MaxSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    public List<Node> FindNeighbors(Node node) {
        List<Node> neighbors = new List<Node>();
        for(int i = -1; i <= 1; i++) {
            for(int j = -1; j <= 1; j++) {
                if(i == 0 && j == 0) {
                    continue;
                }
                int neighborX = node.gridX + i;
                int neighborY = node.gridY + j;

                if(neighborX < gridSizeX && neighborX >= 0
                && neighborY < gridSizeY && neighborY >= 0) {
                    neighbors.Add(grid[neighborX,neighborY]);
                }
            }
        }
        return neighbors;
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition) {
        float percentX = Mathf.Clamp01((worldPosition.x + worldScale.x/2) / worldScale.x);
        float percentY = Mathf.Clamp01((worldPosition.z + worldScale.y/2) / worldScale.y);

        int x = Mathf.RoundToInt(percentX * (gridSizeX - 1));
        int y = Mathf.RoundToInt(percentY * (gridSizeY - 1));

        return grid[x,y];
    }

    private void OnDrawGizmos() {
        if(grid != null && displayGrid) {
            foreach(Node node in grid) {
                Gizmos.color = node.walkable? Color.white : Color.red;
                Gizmos.DrawCube(node.worldPosition, new Vector3(1,0,1) * (nodeDiameter - offset) + Vector3.up);
            }
        }
    }
}

