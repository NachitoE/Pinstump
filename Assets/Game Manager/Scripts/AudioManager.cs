using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;
    [Header("Sources")]
    public AudioSource SFXSource;
    public AudioSource MusicSource;

    [Header("Audio Clips")]
    public AudioClip bgMusic;
    public AudioClip plunger;
    public AudioClip spinner;
    public AudioClip bumper;
    public AudioClip flipper;
    public AudioClip enemySpawning;
    public AudioClip enemyDead;

    private void Awake()
    {
        instance = this;
        MusicSource.clip = bgMusic;
        MusicSource.Play();


    }

    public void PlaySFX(AudioClip sfx)
    {
        SFXSource.pitch = Random.Range(0.95f, 1.05f);
        SFXSource.PlayOneShot(sfx);
    }
}
