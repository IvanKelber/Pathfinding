using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 worldScale;
    public Transform player;
    public float nodeRadius;

    [Range(0,1)]
    public float offset = .1f;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    private void Start() {
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
                grid[x,y] = new Node(walkable, worldPosition);
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition) {
        float percentX = Mathf.Clamp01((worldPosition.x + worldScale.x/2) / worldScale.x);
        float percentY = Mathf.Clamp01((worldPosition.z + worldScale.y/2) / worldScale.y);

        int x = Mathf.RoundToInt(percentX * (gridSizeX - 1));
        int y = Mathf.RoundToInt(percentY * (gridSizeY - 1));

        return grid[x,y];
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(transform.position, new Vector3(worldScale.x, 1, worldScale.y));

        if(grid != null) {
            Node playerNode = NodeFromWorldPosition(player.position);
            foreach(Node node in grid) {
                if(node == playerNode) {
                    Gizmos.color = Color.cyan;
                } else {
                    Gizmos.color = node.walkable? Color.white : Color.red;
                }
                Gizmos.DrawCube(node.worldPosition, new Vector3(1,0,1) * (nodeDiameter - offset) + Vector3.up);
                
            }
        }
    }

}

