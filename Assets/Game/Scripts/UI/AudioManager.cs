using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private List<AudioClip> playerDamage;
    [SerializeField] private List<AudioClip> lightExplostion;
    [SerializeField] private List<AudioClip> heavyExplostion;
    [SerializeField] private List<AudioClip> heal;
    [SerializeField] private List<AudioClip> ceroNoise;
    [SerializeField] private List<AudioClip> shot;
    [SerializeField] private List<AudioClip> barrier;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayDamage()
    {
        audioSource.PlayOneShot(playerDamage[Random.Range(0, playerDamage.Count)]);
    }

    public void PlayLexp()
    {
        audioSource.PlayOneShot(lightExplostion[Random.Range(0, lightExplostion.Count)]);
    }

    public void PlayHexp()
    {
        audioSource.PlayOneShot(heavyExplostion[Random.Range(0, heavyExplostion.Count)]);
    }
    public void PlayHeal()
    {
        audioSource.PlayOneShot(heal[Random.Range(0, heal.Count)]);
    }

    public void PlayCero()
    {
        audioSource.PlayOneShot(ceroNoise[Random.Range(0, ceroNoise.Count)]);
    }

    public void PlayShot()
    {
        audioSource.PlayOneShot(shot[Random.Range(0, shot.Count)]);
    }

    public void PlayBarrier()
    {
        audioSource.PlayOneShot(barrier[Random.Range(0, barrier.Count)]);
    }

}
