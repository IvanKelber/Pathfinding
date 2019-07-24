using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float pathUpdateMoveThreshold = .5f;
    const float minPathUpdateTime = .2f;

    public Transform target;
    public float baseSpeed = 20;
    public float turnDistance = 5;
    public float turnSpeed = 3;
    private float speed;

    public float stoppingDistance = 10;

    Path path;

    public Grid grid;

    void Start() {
        speed = baseSpeed;
        StartCoroutine(UpdatePath());
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful) {
        if(pathSuccessful) {
            path = new Path(waypoints, transform.position, turnDistance, stoppingDistance);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }

    IEnumerator FollowPath() {
        bool followingPath = true;
        int pathIndex = 0;
        transform.LookAt(path.lookPoints[pathIndex]);

        float speedPercent = 1;
        while(followingPath) {
            Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
            while(path.turnBoundaries[pathIndex].HasCrossedLine(pos2D)) {
                if(pathIndex == path.finishLineIndex) {
                    followingPath = false; //break
                    break;
                } else {
                    pathIndex++;
                }
            }

            if(followingPath) {

                if(pathIndex >= path.slowDownIndex && stoppingDistance > 0) {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDistance);
                    if(speedPercent < 0.01f) {
                        followingPath = false;
                    }
                }
                Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }

            yield return null;
        }
    }

    IEnumerator UpdatePath() {
        if(Time.timeSinceLevelLoad < .3f) {
            yield return new WaitForSeconds(.3f);
        }
        PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 posOld = target.position;
        while(true) {
            yield return new WaitForSeconds(minPathUpdateTime);
            if((target.position - posOld).sqrMagnitude > sqrMoveThreshold) {
                PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
                posOld = target.position;
            }

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
