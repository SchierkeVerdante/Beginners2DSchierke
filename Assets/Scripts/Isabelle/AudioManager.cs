using UnityEngine;
using UnityEngine.Audio;
using System;
using System.Collections;


public class AudioManager : MonoBehaviour
{
    [SerializeField]
    AudioSource musicSource;

    [SerializeField]
    AudioSource sfxSource;

    public int musicIndex = 0;

    public AudioClip background1;
    public AudioClip background2;
    public AudioClip thrusters;
    public AudioClip shoot;
    public AudioClip slash;
    public AudioClip hit;
    public AudioClip modulePickup;
    public AudioClip win;
    public AudioClip lose;

    private float musicVolume;
    private float effectVolume;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    
    void Start()
    {
        if(musicIndex==0){
            musicSource.clip=background1;
        }
        else{
            musicSource.clip=background2;
        }
        Debug.Log("Playing Music");
        musicSource.Play();
        
    }

    void PlayMusic(int songIndex){

    }

    // Update is called once per frame
    void Update()
    {
    }
}
