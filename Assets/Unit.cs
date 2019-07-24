using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Transform target;
    public float baseSpeed = 20;
    public float turnDistance = 5;
    public float turnSpeed = 3;
    private float speed;

    Path path;

    public Grid grid;

    void Start() {
        speed = baseSpeed;
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
        if(pathSuccessful) {
            path = new Path(waypoints, transform.position, turnDistance);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath() {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[pathIndex]);

        while(followingPath) {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            if(path.turnBoundaries[pathIndex].HasCrossedLine(pos2D)) {
                if(pathIndex == path.finishLineIndex) {
                    followingPath = false; //break
                } else {
                    pathIndex++;
                }
            }

            if(followingPath) {
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self);
            }

            yield return null;
        }
    }

    private void UpdateSpeed() {
        if(grid != null) {
            Node n = grid.NodeFromWorldPosition(transform.position);
            speed = baseSpeed - n.movementPenalty;
        }
    }

    private void OnDrawGizmos() {
        if(path != null) {
          path.DrawWithGizmos();
        }
    }
}
