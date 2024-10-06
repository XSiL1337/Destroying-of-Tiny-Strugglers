using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Jet : Enemy
{


    private RectTransform target;

    private Vector2 movingPositionWorld;

    private GameManager gameManager;


    float speedMultiplier = 1f;

    [SerializeField] float rotationSpeed = 50f;

    [Header("Shooting")]
    [SerializeField] private RectTransform hittingPos0;
    [SerializeField] private RectTransform hittingPos1;
    private bool swap = false;
    private RectTransform activeHittingPos;
    private float fireRate = 0.3f;
    [SerializeField] private GameObject bulletPrefab;
    bool isShooting = false;

    //StateMachine


    bool isForced = false;

    enum State
    {
        lookingForPosition,
        gettingToPosition,
        preparingForAttack,
        attacking
    }

    private State state = 0;



    protected override void Start()
    {
        base.Start();
        activeHittingPos = hittingPos0;
        target = GameManager.instance.playerRT;
        gameManager = GameManager.instance;

    }

    protected override void OnActive()
    {

        switch (state)
        {
            case State.lookingForPosition:
                {
                    GameplayLine line = gameManager.GetGameplayLineT0();
                    isForced = false;
                    movingPositionWorld = line.GetRandomWorldPos();//new Vector3(worldPos.x, worldPos.y, canvas.position.z);
                    IncState();
                }
                break;
            case State.gettingToPosition:
                {
                    float distance = Vector2.Distance(rt.position, movingPositionWorld);
                    if (distance > 1)
                    {
                        if (!isForced) speedMultiplier = 1f;
                        MoveInDirection(movingPositionWorld);
                    }
                    else
                    {
                        IncState();
                    }
                }
                break;
            case State.preparingForAttack:
                {
                    GameplayLine line = gameManager.GetGameplayLineT0();
                    isForced = false;
                    line = gameManager.GetGameplayLineT0();
                    movingPositionWorld = GameManager.instance.playerRT.position;
                    IncState();
                }
                break;
            case State.attacking:
                {
                    float distance = Vector2.Distance(rt.position, movingPositionWorld);
                    if (distance > 1)
                    {
                        if (!isForced) speedMultiplier = 3f;
                        MoveInDirection(movingPositionWorld);
                        if (!isShooting && !isForced)
                        {
                            Debug.Log("start attacking");
                            StartCoroutine(ShootCoroutine());
                        }
                    }
                    else
                    {
                        IncState();
                    }
                }
                break;
        }
    }


    private void Shoot()
    {

        RectTransform temp = Instantiate(bulletPrefab, transform.parent).GetComponent<RectTransform>();
        temp.position = activeHittingPos.position;
        temp.rotation = activeHittingPos.rotation;
        AudioManager.instance.PlayShot();
        if (!swap)
        {
            activeHittingPos = hittingPos1;
        }
        else
        {
            activeHittingPos = hittingPos0;
        }
        swap = !swap;
        
    }



    private IEnumerator ShootCoroutine()
    {
        while (state == State.attacking)
        {
            isShooting = true;
            Shoot();
            yield return new WaitForSeconds(fireRate);
        }
        isShooting = false;
    }


    private void IncState()
    {
        if ((int)++state >= Enum.GetValues(typeof(State)).Length)
        {
            state = 0;
        }
        speedMultiplier = 1f;
        isShooting = false;
        isForced = false;
    }

    private void MoveInDirection(Vector2 pos)
    {
        float step = movingSpeed * Time.deltaTime;
        rt.position += rt.right * movingSpeed * Time.deltaTime * speedMultiplier;
        RotateTowardsTarget(pos);
    }

    private void RotateTowardsTarget(Vector2 pos)
    {
        Vector2 direction = pos - (Vector2)rt.position;

        if (direction != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            float angle = Mathf.MoveTowardsAngle(rt.eulerAngles.z, targetAngle, rotationSpeed * Time.deltaTime * speedMultiplier);
            rt.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    public override void GetHit()
    {
        if (!invincible)
        {
            if (--hp <= 0)
            {
                Death();
            }
            else
            {
                body.DOShakePosition(0.5f, .5f);
                state = (State)1;
                isForced = true;
                movingPositionWorld = GameManager.instance.GetGameplayLineT0().GetRandomWorldPos();
                speedMultiplier = 5f;
                StartCoroutine(InvincibilityCoroutine());
            }
        }
    }




}
