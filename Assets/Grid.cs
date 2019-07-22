using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public bool displayGrid = false;
    public Vector2 worldScale;
    public float nodeRadius;

    public LayerMask unwalkableMask;
    public TerrainType[] walkableRegions;
    private LayerMask walkableMask;
    public Dictionary<int, int> penaltyMap = new Dictionary<int,int>();

    public int obstacleProximityPenalty = 10;

    [Range(0,1)]
    public float offset = .1f;
    Node[,] grid;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    int penaltyMin = int.MaxValue;
    int penaltyMax = int.MinValue;

    private void Awake() {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(worldScale.x/nodeDiameter);
        gridSizeY = Mathf.RoundToInt(worldScale.y/nodeDiameter);

        foreach(TerrainType t in walkableRegions) {
            walkableMask |= t.terrainMask;
            penaltyMap.Add((int)Mathf.Log(t.terrainMask, 2), t.terrainPenalty);
        }

        CreateGrid();
    }

    void CreateGrid() {
        grid = new Node[gridSizeX, gridSizeY];

        Vector3 bottomLeft = transform.position - (Vector3.right * worldScale.x/2) - (Vector3.forward * worldScale.y/2);

        for(int x = 0; x < gridSizeX; x++) {
            for(int y = 0; y < gridSizeY; y++) {
                Vector3 worldPosition = bottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !Physics.CheckSphere(worldPosition, nodeRadius, unwalkableMask);
                
                int movementPenalty = 0;

                // Check which terrain type exists for that node.
                Ray ray = new Ray(worldPosition + Vector3.up * 10, Vector3.down);
                RaycastHit hit;
                if(Physics.Raycast(ray, out hit, 100, walkableMask)) {
                    penaltyMap.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
                }

                if(!walkable) {
                    movementPenalty += obstacleProximityPenalty;
                }
                
                grid[x,y] = new Node(walkable, worldPosition, x, y, movementPenalty);
            }
        }
        BlurPenalty(5);
    }

    public int MaxSize {
        get {
            return gridSizeX * gridSizeY;
        }
    }

    // Smooth movementpenalties across the entire grid
    void BlurPenalty(int blurSize) {
        int kernelSize = (blurSize * 2) + 1;
        int kernelOverflow = blurSize;

        int[,] horizontalPenalty = new int[gridSizeX,gridSizeY];
        int[,] verticalPenalty = new int[gridSizeX,gridSizeY];

        //Horizontal
        for(int y = 0; y < gridSizeY; y++) {
            // First we calculate the first edge value of each row.
            for(int x = -kernelOverflow; x <= kernelOverflow; x++) {
                int kernelIndex = Mathf.Clamp(x, 0, kernelOverflow);
                horizontalPenalty[0,y] += grid[kernelIndex,y].movementPenalty;
            }

            // Then we can use dynamic programming to figure out the rest.
            for(int x = 1; x < gridSizeX; x++) {
                int removeIndex = Mathf.Clamp(x - kernelOverflow, 0, gridSizeX - kernelOverflow - 1);
                int addIndex = Mathf.Clamp(x + kernelOverflow, 0, gridSizeX - 1);
                horizontalPenalty[x,y] = horizontalPenalty[x-1,y] - grid[removeIndex,y].movementPenalty + grid[addIndex,y].movementPenalty;
            }
        }

        //Vertical
        for(int x = 0; x < gridSizeX; x++) {
            // First we calculate the first edge value of each row.
            for(int y = -kernelOverflow; y <= kernelOverflow; y++) {
                int kernelIndex = Mathf.Clamp(y, 0, kernelOverflow);
                verticalPenalty[x,0] += horizontalPenalty[x,kernelIndex];
            }
            // Get the blur for the first row.
            int blurredPenalty = Mathf.RoundToInt((float)verticalPenalty[x,0]/ (kernelSize*kernelSize));
            grid[x,0].movementPenalty = blurredPenalty;

            // Then we can use dynamic programming to figure out the rest.
            for(int y = 1; y < gridSizeY; y++) {
                int removeIndex = Mathf.Clamp(y - kernelOverflow, 0, gridSizeY - kernelOverflow - 1);
                int addIndex = Mathf.Clamp(y + kernelOverflow, 0, gridSizeY - 1);
                verticalPenalty[x,y] = verticalPenalty[x, y-1] - horizontalPenalty[x,removeIndex] + horizontalPenalty[x,addIndex];
                blurredPenalty = Mathf.RoundToInt((float)verticalPenalty[x,y]/ (kernelSize*kernelSize));

                // For visualizing the movementPenalties in gizmos.
                if(blurredPenalty < penaltyMin) {
                    penaltyMin = blurredPenalty;
                }
                if(blurredPenalty > penaltyMax) {
                    penaltyMax = blurredPenalty;
                }

                grid[x,y].movementPenalty = blurredPenalty;
            }
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
                Gizmos.color = Color.Lerp(Color.white, Color.black, Mathf.InverseLerp(penaltyMin, penaltyMax, node.movementPenalty));
                Gizmos.color = node.walkable? Gizmos.color : Color.red;
                Gizmos.DrawCube(node.worldPosition, new Vector3(1,0,1) * (nodeDiameter - offset) + Vector3.up);
            }
        }
    }

    [System.Serializable]
    public struct TerrainType {
        public LayerMask terrainMask;
        public int terrainPenalty;
    }
}

