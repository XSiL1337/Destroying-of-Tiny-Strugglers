using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{
    public static UserInterface instance;

    [SerializeField] private RectTransform humanIcon;
    [SerializeField] private RectTransform playerIcon;
    [SerializeField] private TextMeshProUGUI humansLeftText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthBarText;
    [SerializeField] private Image healthFilling;
    [SerializeField] private string animationTag = "wave";
    private Color defaultHealthColor = Color.red;
    [SerializeField] private VolumeProfile volume;
    [SerializeField] private ChromaticAberration cAberration;

    private Camera cam;
    private Vector3 defaultPosCam;
    private float defaultCA;
    private void Awake()
    {
        if (instance == null) 
            instance = this;    
    }

    void Start()
    {
        Heartbeat();
        HumanIcon();
        defaultHealthColor = healthFilling.color;
        cam = Camera.main;
        if (volume.TryGet(out cAberration))
        {
            defaultCA = 0.085f;
        }
        defaultPosCam = cam.transform.position;
        restart.onClick.AddListener(()=> { SceneManager.LoadScene(0); });
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void UpdateVictims(int start, int end, float percentage)
    {
        AnimateHumansLeftText(start, end, percentage);
        Punching(end - start);
    }

    private void HumanIcon()
    {
        float rotationAngle = 15f; 
        float duration = 1.2f; 

        Sequence rotationSequence = DOTween.Sequence();

        rotationSequence.Append(humanIcon.DORotate(new Vector3(0, 0, rotationAngle), duration)
            .SetEase(Ease.InOutCirc))
            .Append(humanIcon.DORotate(new Vector3(0, 0, -rotationAngle), duration)
            .SetEase(Ease.InOutCirc))
            .OnComplete(HumanIcon);
    }

    private void Heartbeat()
    {
        playerIcon.DOKill();
        float beatScaleFactor = 1.1f;
        float pauseDuration = .5f;
        float beatDuration = .1f;

        Sequence heartbeatSequence = DOTween.Sequence();

        heartbeatSequence
            .Append(playerIcon.DOScale(Vector3.one * beatScaleFactor, beatDuration))
            .Append(playerIcon.DOScale(Vector3.one, beatDuration).SetEase(Ease.InOutBack))
            .Append(playerIcon.DOScale(Vector3.one * beatScaleFactor, beatDuration))
            .Append(playerIcon.DOScale(Vector3.one, beatDuration).SetEase(Ease.InOutBack))
            .AppendInterval(pauseDuration) 
            .OnComplete(Heartbeat);
    }

    private void Punching(int arg)
    {
        humanIcon.DOKill();
        Sequence punchingSequence = DOTween.Sequence();


        for (; arg >= 0; --arg) 
        {
            punchingSequence
                .Append(humanIcon.DOPunchAnchorPos(new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)), 1).SetEase(Ease.InOutCubic))
                .OnComplete(() => SpawnPunchEffect());
        }
    }

    private void SpawnPunchEffect()
    {
        //
    }

    public void ChangeHP(int hp, int maxHp, bool isPositive)
    {
        healthBar.DOKill();
        healthFilling.DOKill();
        
        float percentage = (float)hp/ maxHp;
        healthBar.DOValue(percentage, 1f);
        if (!isPositive)
        {
            playerIcon.DOKill();
            playerIcon.DOShakeAnchorPos(.5f, 10f);
            ShakeScreen(percentage / 10f);
            healthFilling.DOColor(Color.white, 0.1f).SetEase(Ease.Flash).OnComplete(() => healthFilling.DOColor(defaultHealthColor, percentage).SetEase(Ease.InSine));
        }
        else
        {
            healthFilling.DOColor(Color.green, 0.1f).SetEase(Ease.Flash).OnComplete(() => healthFilling.DOColor(defaultHealthColor, percentage).SetEase(Ease.InSine));
        }
        
        healthBarText.text = hp.ToString();
    }

    private void AnimateHumansLeftText(int startValue, int endValue, float duration)
    {
        DOTween.To(() => startValue, x =>
        {
            startValue = x;
            humansLeftText.text = "Creatures left:\n<" + animationTag + $">{ FormatValue(startValue)}K";
        }, endValue, duration).OnComplete(() =>
        {
            humansLeftText.text = "Creatures left:\n<" + animationTag + $">{FormatValue(endValue)}K"; 
        });
    }

    private string FormatValue(int value)
    {
        return value.ToString("N0", CultureInfo.InvariantCulture);
    }

    public void ShakeScreen(float value)
    {
        cam.DOComplete();
        cam.DOShakePosition(value / 2, 2*value).SetEase(Ease.InCubic)
            .OnComplete(() => { cam.transform.position = defaultPosCam; });
        cAberration.intensity.value = 1f;

        
        DOTween.To(() => cAberration.intensity.value,
                   x => cAberration.intensity.value = x,
                   defaultCA,
                   0.5f);
    }


    [SerializeField] private GameObject winScreen;
    [SerializeField] private TextMeshProUGUI gradeText;
    [SerializeField] private Button restart;


    public void ShowWinScreen(string grade)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(0).gameObject.SetActive(false);


        }

        winScreen.SetActive(true);
        gradeText.text = grade;
    }
}
