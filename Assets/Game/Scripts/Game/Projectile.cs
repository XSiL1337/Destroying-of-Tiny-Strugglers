using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : Enemy
{
    

    protected override void Start()
    {
        rt = GetComponent<RectTransform>();
        Destroy(gameObject, 10);
    }


    protected override void OnActive()
    {
        rt.anchoredPosition += (Vector2)(rt.right * movingSpeed * Time.deltaTime);
    }

}
