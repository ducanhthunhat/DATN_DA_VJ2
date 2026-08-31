using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Settings")]
    private const string MUSIC_KEY = "MusicVolume";
    private const string SFX_KEY = "SFXVolume";

    public float MusicVolume { get; private set; } = 1f;
    public float SFXVolume { get; private set; } = 1f;

    public override void Awake()
    {
        base.Awake();
        if (Instance != this) return;

        LoadVolume();
    }

    private void LoadVolume()
    {
        MusicVolume = PlayerPrefs.GetFloat(MUSIC_KEY, 1f);
        SFXVolume = PlayerPrefs.GetFloat(SFX_KEY, 1f);

        ApplyVolume();
    }

    private void ApplyVolume()
    {
        if (musicSource != null) musicSource.volume = MusicVolume;
        if (sfxSource != null) sfxSource.volume = SFXVolume;
    }

    public void SetMusicVolume(float volume)
    {
        MusicVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(MUSIC_KEY, MusicVolume);
        PlayerPrefs.Save();
        if (musicSource != null) musicSource.volume = MusicVolume;
    }

    public void SetSFXVolume(float volume)
    {
        SFXVolume = Mathf.Clamp01(volume);
        PlayerPrefs.SetFloat(SFX_KEY, SFXVolume);
        PlayerPrefs.Save();
        if (sfxSource != null) sfxSource.volume = SFXVolume;
    }

    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource == null || clip == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null) musicSource.Stop();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip, SFXVolume);
    }
}
