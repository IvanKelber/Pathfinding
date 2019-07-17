using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    // For A* algorithm
    public Node parent;
    public int gCost;
    public int hCost;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY) {
        this.walkable = _walkable;
        this.worldPosition = _worldPosition;
        this.gridX = _gridX;
        this.gridY = _gridY;
    }

    // For use in A* algorithm for node to node distance comparisons
    public int fCost {
        get {
            return gCost + hCost;
        }
    }




}
