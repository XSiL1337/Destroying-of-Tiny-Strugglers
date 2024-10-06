using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TransformLine : MonoBehaviour
{
    [SerializeField] protected RectTransform pointA, pointB;

    RectTransform rt;
    RectTransform c;


    protected virtual void Start()
    {
        rt = GetComponent<RectTransform>();
        c = GetComponentInParent<RectTransform>();
    }

    public Vector3 GetRandomWorldPos()
    {
        Vector3 res = Vector2.zero;
        res.x = Random.Range(pointA.position.x, pointB.position.x);
        res.y = Random.Range(pointA.position.y, pointB.position.y);
        res.z = rt.parent.position.z;
        return res;
    }

    public Vector2 GetRandomPos()
    {
        Vector2 res = Vector2.zero;
        Vector2 a = c.InverseTransformPoint(pointA.position);
        Vector2 b = c.InverseTransformPoint(pointB.position);

        res.x = Random.Range(a.x, b.x);
        res.y = Random.Range(a.y, b.y);

        return res;
    }
    
}

public class SpawnLine : TransformLine
{
    [SerializeField] private Vector2 facingDirection = Vector2.zero;
    [SerializeField] public readonly int type = 0; //up down left right

    public Vector2 GetFacingDirection()
    {
        return facingDirection;
    }

}
