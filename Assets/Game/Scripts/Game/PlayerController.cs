using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Android;

public class PlayerController : MonoBehaviour
{
    Transform targetPosition;
    RectTransform rt;

    [SerializeField] private RectTransform leftHand;

    [SerializeField] private RectTransform rightHand;

    [SerializeField] private Vector2 leftHandOffset;
    [SerializeField] private Vector2 rightHandOffset;

    private float screenBorderOffset = 100f;

    [SerializeField] private PlayerHitbox leftHB;
    [SerializeField] private PlayerHitbox rightHB;



    [SerializeField] private float mouseSensivity = 10;

    [SerializeField] RectTransform head;

    private float timer = 0;

    [SerializeField] private GameObject ceroPrefab;
    private int ceroStage = 0;
    private float ceroTimer = 0;
    private float ceroInitialTime = 1.5f;
    private float fullChargeTime = 10f;
    private float maxChargeTime = 15f;
    [SerializeField] private RectTransform playerMouth;
    private RectTransform activeCero = new RectTransform();

    private Vector2 leftHandOffsetDefault;
    private Vector2 rightHandOffsetDefault;
    [SerializeField] private Vector2 leftHandOffsetCero;
    [SerializeField] private Vector2 rightHandOffsetCero;
    [SerializeField] private RectTransform playerBody;
    private bool justReseted = false;
    // Start is called before the first frame update
    void Awake()
    {
        rt= GetComponent<RectTransform>();
        screenBorderOffset = Screen.height / 4f;
    }

    private void Start()
    {
        HeadAnimation();
        leftHandOffsetDefault = leftHandOffset;
        rightHandOffsetDefault = rightHandOffset;
    }



    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            justReseted = false;
        }
        if (Input.GetKey(KeyCode.Space) && !justReseted)
        {
            if (ceroTimer == 0)
            {
                playerMouth.DOScaleX(0.66f, 9.9f).SetEase(Ease.OutSine);
            }
            ceroTimer += Time.deltaTime;
            switch (ceroStage)
            {
                case 0:
                   
                    if (ceroTimer > ceroInitialTime)
                    {
                        ceroStage++;
                        activeCero = Instantiate(ceroPrefab, rt.parent.GetComponent<RectTransform>()).GetComponent<RectTransform>();
                        activeCero.anchoredPosition = rt.anchoredPosition + Vector2.down * 50;
                        activeCero.DOScale(150, 8.4f);
                    }
                    else
                    {
                        float percentage = ceroTimer / ceroInitialTime;
                        leftHand.anchoredPosition = Vector2.Lerp(rt.anchoredPosition + leftHandOffsetDefault, rt.anchoredPosition + leftHandOffsetCero, percentage);
                        rightHand.anchoredPosition = Vector2.Lerp(rt.anchoredPosition + rightHandOffsetDefault, rt.anchoredPosition + rightHandOffsetCero, percentage);
                    }
                    break;
                case 1:
                    if (ceroTimer > fullChargeTime)
                    {
                        ceroStage++;
                        activeCero.DOScale(300, 4.9f);
                        playerMouth.DOShakePosition(8.4f, 5.5f);
                    }
                    else
                    {
                        
                    }
                    break;
                case 2:
                    if (ceroTimer > maxChargeTime)
                    {
                        ceroStage++;
                        AudioManager.instance.PlayCero();
                    }
                    else
                    {
                        float percentage = ceroTimer / maxChargeTime;
                        leftHand.anchoredPosition = Vector2.Lerp(rt.anchoredPosition + leftHandOffsetCero, rt.anchoredPosition + leftHandOffsetDefault, percentage/3f);
                        rightHand.anchoredPosition = Vector2.Lerp(rt.anchoredPosition + rightHandOffsetCero, rt.anchoredPosition + rightHandOffsetDefault, percentage/3f);
                    }
                    break;
                case 3:
                    AudioManager.instance.PlayCero();
                    ReleaseCero();
                    break;
            }

        }
        else
        {

            ReleaseCero();

            bool m1 = false, m2 = false;


            if (Input.GetMouseButton(0))
            {
                Cursor.lockState = CursorLockMode.Locked;
                leftHand.DOComplete();
                leftHand.anchoredPosition += GetMouseDelta() * mouseSensivity;
                leftHB.SetActive(true);
                m1 = true;
            }
            else
            {
                leftHand.anchoredPosition = Vector2.Lerp(leftHand.anchoredPosition, rt.anchoredPosition + leftHandOffset, .1f);
                leftHB.SetActive(false);
            }

            if (Input.GetMouseButton(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                rightHand.DOComplete();
                rightHand.anchoredPosition += GetMouseDelta() * mouseSensivity;
                rightHB.SetActive(true);
                m2 = true;
            }
            else
            {

                rightHand.anchoredPosition = Vector2.Lerp(rightHand.anchoredPosition, rt.anchoredPosition + rightHandOffset, .1f);
                rightHB.SetActive(false);
            }

            if (!m1 && !m2)
            {
                Cursor.lockState = CursorLockMode.None;
                Vector2 temp = rt.anchoredPosition;
                temp.x = Mathf.Clamp(temp.x + Input.GetAxis("Mouse X") * mouseSensivity, screenBorderOffset, Screen.width - screenBorderOffset);
                temp.y = Mathf.Clamp(temp.y + Input.GetAxis("Mouse Y") * mouseSensivity, screenBorderOffset, Screen.height - screenBorderOffset);
                rt.anchoredPosition = temp;
            }
            else
            if (m1 && m2)
            {

            }
        }

       
    }



    private Vector2 GetMouseDelta()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    private void  HeadAnimation()
    {
        float moveDistance = 10f, duration = .8f;
        head.anchoredPosition = Vector2.zero;
        // Move up
        head.DOAnchorPosY(head.anchoredPosition.y + moveDistance, duration)
            .SetEase(Ease.InOutSine) // Smooth in and out movement
            .OnComplete(() =>
            {
                // Move down
                head.DOAnchorPosY(head.anchoredPosition.y - moveDistance, duration)
                    .SetEase(Ease.InOutSine)
                    .OnComplete(HeadAnimation); // Call Animate again for looping
            });
    }

    private void ReleaseCero()
    {
        
        if (ceroStage > 0)
        {
            activeCero.DOKill();
            GameManager.instance.CeroFired(ceroTimer/maxChargeTime);
            activeCero.GetComponent<Rigidbody2D>().simulated = true;
            activeCero.GetComponentInChildren<ParticleSystem>().Stop();
        }

        ceroTimer = 0;
        ceroStage = 0;

        playerMouth.localScale = Vector3.one;
        playerMouth.anchoredPosition = Vector3.zero;
        if (activeCero != null)
        {
            Destroy(activeCero.gameObject, 3);
            activeCero = null;
        }
        justReseted = true;
    }

}
