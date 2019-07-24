using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    const float verticalSlope = 1e5f;

    float slope; //m
    float y_intercept; //b

    float perpendicularSlope;
    Vector2 pointOnLine_1;
    Vector2 pointOnLine_2;

    bool approachSide = false;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine) {
        float dx = pointOnLine.x - pointPerpendicularToLine.x;
        float dy = pointOnLine.y - pointPerpendicularToLine.y;

        if(dx == 0) {
            perpendicularSlope = verticalSlope;
        } else {
            perpendicularSlope = dy /dx;
        }

        if(perpendicularSlope == 0) {
            slope = verticalSlope;
        } else {
            slope = -1/ perpendicularSlope;
        }

        y_intercept = pointOnLine.y - slope* pointOnLine.x;

        pointOnLine_1 = pointOnLine;
        pointOnLine_2 = new Vector2(1, slope);

        approachSide = GetSide(pointPerpendicularToLine);
    }

    // Compute the cross product given the two points on our line and another point off the line somewhere
    bool GetSide(Vector2 p) {
        return (p.x - pointOnLine_1.x) * (pointOnLine_2.y - pointOnLine_1.y) > (p.y - pointOnLine_1.y) * (pointOnLine_2.x - pointOnLine_1.x);
    }

    public bool HasCrossedLine(Vector2 p) {
        return GetSide(p) != approachSide;
    }

    public float DistanceFromPoint(Vector2 p) {
        float perpendicularYIntercept = p.y - perpendicularSlope * p.x;
        float intersectX = (perpendicularYIntercept - y_intercept) / (slope - perpendicularSlope);

        float intersectY = slope * intersectX + y_intercept;
        return Vector2.Distance(p, new Vector2(intersectX, intersectY));
    }

    public void DrawWithGizmos(float length) {
        Vector3 lineDirection = new Vector3(1, 0, slope).normalized;
        Vector3 lineCenter = new Vector3(pointOnLine_1.x, 0, pointOnLine_1.y);
        Gizmos.DrawLine(lineCenter - lineDirection * length/2, lineCenter + lineDirection * length/2);
    }

}
