using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public abstract class Enemy : MonoBehaviour
{

    [SerializeField] protected int hp = 1;
    [SerializeField] protected int damageOnCollide = 10;
    [SerializeField] protected Vector3 movingDirection = Vector3.right;
    [SerializeField] protected float movingSpeed = 1;
    [SerializeField] protected bool physicsOnDeath = true;
    [SerializeField] protected int victimsOnDeath = 1;

    [SerializeField] protected RectTransform body;
    protected RectTransform rt;
    [SerializeField] protected SpriteRenderer sr;

    protected bool isActive = true;

    public readonly bool[] spawnDirections = new bool[] { false, false, true, true }; //up down left right

    [SerializeField] protected bool flippable = false;
    [SerializeField] protected bool invincible = false;
    [SerializeField] protected bool isProjectile = false;

    [SerializeField] GameObject barrier;
    [Header("Effects")]
    [SerializeField] protected GameObject spawnNoticePrefab;
    [SerializeField] protected GameObject deathEffectPrefab;

    protected virtual void Start()
    {
        rt = GetComponent<RectTransform>();
        if (sr == null) sr = body.GetComponent<SpriteRenderer>();//doesnt work otherwise?
    }

    protected virtual void Update()
    {
        if (isActive)
        {
            OnActive();
        }
    }

    protected virtual void OnActive()
    {
        Move();
    }

    protected virtual void Move()
    {
        rt.anchoredPosition += ((Vector2)movingDirection * movingSpeed * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            CollidedWithPlayer();
        }
    }

    protected virtual void CollidedWithPlayer()
    {
        if (!invincible || (invincible && isProjectile))
        {
            GameManager.instance.TakeDamage(damageOnCollide);
            Death();
        }
    }

    public virtual void GetHit()
    {
        if (!invincible)
        {
            if (--hp <= 0)
            {
                Death();
            }
            else
            {
                body.DOShakePosition(0.2f, 0.5f);
            }
        }
    }

    protected void Death()
    {

        isActive = false;
        GameManager.instance.AddVictims(victimsOnDeath, false);
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, rt.parent.GetComponent<RectTransform>()).transform.position = transform.position; 
        }

        if (physicsOnDeath)
        {
            GetComponent<Collider2D>().enabled = false;


            Rigidbody2D rb2d = gameObject.AddComponent<Rigidbody2D>();
            rb2d.gravityScale = 0.5f;
            float sign = (Random.Range(0, 2) > 0) ? -1 : 1;
            rb2d.AddTorque(sign * Random.Range(600, 1200));
            rb2d.AddForce(GameManager.instance.DirectionToPlayer(rt.anchoredPosition) * -2.5f);

            Destroy(gameObject, 10);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SetMovingDirection(Vector2 arg)
    {
        arg.y = 0;
        movingDirection = arg.normalized;
        if (arg.x < 0 && flippable)
        {
            sr.flipX = true;
        }
    }

    protected IEnumerator InvincibilityCoroutine()
    {
        invincible = true;
        if (barrier != null) { 
            barrier.SetActive(true);
            AudioManager.instance.PlayBarrier();
        }
        
        yield return new WaitForSeconds(3f);
        invincible = false;
        if (barrier != null) barrier.SetActive(false);
    }
}
