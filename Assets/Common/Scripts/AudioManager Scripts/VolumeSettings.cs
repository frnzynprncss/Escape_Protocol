using UnityEngine;
using UnityEngine.UI; // Required for handling Sliders

public class VolumeSettings : MonoBehaviour
{
    [Header("UI Sliders")]
    public Slider musicSlider;
    public Slider sfxSlider;

    private void Start()
    {
        // 1. Check if Audio Manager exists
        if (AudioManager.instance == null)
        {
            Debug.LogWarning("Audio Manager not found!");
            return;
        }

        // 2. Set the slider positions to match the saved volume
        // We use the same keys "MusicVolume" and "SFXVolume" as the AudioManager
        musicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        // 3. Add listeners to detect when the player moves the slider
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        AudioManager.instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        AudioManager.instance.SetSFXVolume(value);
    }
}