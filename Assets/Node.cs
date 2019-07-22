using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;
    private int heapIndex;
    public int movementPenalty {
        get;
    }

    //==== For A* algorithm ====//

    // For retracing our chosen path.
    public Node parent;
    // Distance from the start node
    public int gCost;
    // Distance from the target node
    public int hCost;

    public Node(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY, int _penalty) {
        this.walkable = _walkable;
        this.worldPosition = _worldPosition;
        this.gridX = _gridX;
        this.gridY = _gridY;
        this.movementPenalty = _penalty;
    }

    // For use in A* algorithm for node to node distance comparisons
    public int fCost {
        get {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node other) {
        int compareTo = fCost.CompareTo(other.fCost);
        if(compareTo == 0) {
            compareTo = hCost.CompareTo(other.hCost);
        }
        return -compareTo;
    }

    public int Val {
        get {
            return this.fCost;
        }
    }



}
