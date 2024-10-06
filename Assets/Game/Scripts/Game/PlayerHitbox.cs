using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [SerializeField] BoxCollider2D hitboxCollider;

    private void Start()
    {
        if (gameManager == null)
        gameManager = GameManager.instance;
        if (gameManager == null)
            hitboxCollider = GetComponent<BoxCollider2D>();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            if (collision != null) //fixes weird behaviour?
                gameManager.CollidedWith(collision.gameObject);
        }
    }

    public void SetActive(bool arg)
    {
        hitboxCollider.enabled = arg;
    }
}
