using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Earth : MonoBehaviour
{

    [SerializeField] private RectTransform surfaceImage1;
    [SerializeField] private RectTransform surfaceImage2;
    [SerializeField] private float surfaceScrollSpeed = 1f;
    private const float surfaceY = 0;
    private float surfaceWidth;

    [SerializeField] private RectTransform cloudsImage1;
    [SerializeField] private RectTransform cloudsImage2;
    [SerializeField] private float cloudsScrollSpeed = 1f;
    private const float cloudsY = 96;
    private float cloudsWidth;

    void Start()
    {
        surfaceWidth = surfaceImage1.rect.width;

        surfaceImage1.anchoredPosition = new Vector2(0, surfaceY);
        surfaceImage2.anchoredPosition = new Vector2(surfaceWidth, surfaceY);
        MoveImage(surfaceImage1, surfaceScrollSpeed, surfaceWidth);
        MoveImage(surfaceImage2, surfaceScrollSpeed, surfaceWidth);


        cloudsWidth = cloudsImage1.rect.width;

        cloudsImage1.anchoredPosition = new Vector2(0, cloudsY);
        cloudsImage2.anchoredPosition = new Vector2(cloudsWidth, cloudsY);
        MoveImage(cloudsImage1, cloudsScrollSpeed, surfaceWidth);
        MoveImage(cloudsImage2, cloudsScrollSpeed, surfaceWidth);
    }


    void MoveImage(RectTransform image, float speed, float width)
    {
        float startPosX = image.anchoredPosition.x;
        float startPosY = image.anchoredPosition.y;

        float targetPos = startPosX - width;
        image.DOAnchorPosX(targetPos, speed)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                image.anchoredPosition = new Vector2(startPosX, startPosY);
                MoveImage(image, speed, width); 
            });
    }


}
