using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Effect : MonoBehaviour
{
    [SerializeField] private RectTransform body;
    [SerializeField] private float time;
    void Start()
    {
        Color img = body.GetComponent<Image>().color;
        img.a = 0;
        body.GetComponent<Image>().DOColor(img, time).SetEase(Ease.OutExpo);
        StartCoroutine(RotateEffect());
        AudioManager.instance.PlayLexp();
        Destroy(gameObject, time + 2);
    }

    IEnumerator RotateEffect()
    {
        float[] angles = { 0f, 90f, 180f, 270f, 45f, 135f };


        while (true) 
        {
            float targetZRotation = angles[Random.Range(0, angles.Length)];

            Quaternion initialRotation = transform.rotation;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetZRotation);

            body.transform.rotation = targetRotation;

            yield return new WaitForSeconds(.2f);
        }
    }
}

