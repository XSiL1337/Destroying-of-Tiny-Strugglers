using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayLine : TransformLine
{

    public Vector3 GetLerpPosition(float t)
    {
        return Vector3.Lerp(pointA.position, pointB.position, t);
    }

    public void SetPointA(Vector3 arg)
    {
        pointA.position = arg;
    }

    public void SetPointB(Vector3 arg)
    {
        pointA.position = arg;
    }

}
