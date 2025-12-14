using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Source")]
    [SerializeField] AudioSource musicSource;
    [SerializeField] AudioSource SFXSource;

    [Header("Audio Clip")]
    public AudioClip mainmenu_music;
    public AudioClip ingame_music;
    public AudioClip buttonselect;
    public AudioClip healthcollect;
    public AudioClip itemcollect;
    public AudioClip shoot;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // LOAD SAVED VOLUME SETTINGS
        // We use '1f' (full volume) as the default if no save exists yet.
        musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 1f);
        SFXSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // Start the Main Menu music
        PlayMusic(mainmenu_music);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "PlayerTestScene")
        {
            PlayMusic(ingame_music);
        }
        else if (scene.name == "MainMenu")
        {
            if (musicSource.clip != mainmenu_music)
            {
                PlayMusic(mainmenu_music);
            }
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip);
    }

    public void PlayButtonSound()
    {
        SFXSource.PlayOneShot(buttonselect);
    }

    // --- NEW VOLUME CONTROLS ---

    public void SetMusicVolume(float volume)
    {
        musicSource.volume = volume;
        // Save the setting instantly
        PlayerPrefs.SetFloat("MusicVolume", volume);
    }

    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
        // Save the setting instantly
        PlayerPrefs.SetFloat("SFXVolume", volume);
    }
}