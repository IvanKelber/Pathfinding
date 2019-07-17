using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{

    // public float scaleX;
    // public float scaleY;
    // public int numNodesX;
    // public int numNodesY;
    // public float percentageVisible;

    // private float nodeWidth;
    // private float nodeHeight;
    // private List<Node> nodes;

    // private float startX;
    // private float startY;

    // // Start is called before the first frame update
    // void Awake()
    // {
    //     nodeWidth = scaleX/numNodesX;
    //     nodeHeight = scaleY/numNodesY;
    //     nodes = new List<Node>();

    //     startX = -scaleX/2;
    //     startY = -scaleY/2;
    //     CreateGrid();
    // }

    // void CreateGrid() {
    //     float currentX = startX;
    //     for(int i = 0; i < numNodesX; i++) {
    //         float currentY = startY;

    //         for(int j = 0; j < numNodesY; j++) {
    //             nodes.Add(new Node(i, j, currentX, currentY, nodeWidth, nodeHeight));
    //             // print(i + " , " + j + ", " + currentX + ", " + currentY);
    //             currentY += nodeHeight;
    //         }
    //         currentX += nodeWidth;
    //     }
    // }

    // private void OnDrawGizmos() {
    //     nodeWidth = scaleX/numNodesX;
    //     nodeHeight = scaleY/numNodesY;
    //     nodes = new List<Node>();

    //     startX = -scaleX/2;
    //     startY = -scaleY/2;
    //     CreateGrid();
    //     foreach(Node node in nodes) {
    //         // print(node.posX + ", " + node.posY);
    //         Gizmos.DrawCube(new Vector3(node.posX, 0, node.posY), new Vector3(nodeWidth, 1, nodeHeight));
    //     }
    // }
}
