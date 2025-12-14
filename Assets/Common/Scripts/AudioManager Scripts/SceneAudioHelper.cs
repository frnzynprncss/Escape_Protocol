using UnityEngine;

public class SceneAudioHelper : MonoBehaviour
{
    // This function will find the "Live" Audio Manager and use it
    public void PlayClickSound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlayButtonSound();
        }
        else
        {
            Debug.LogWarning("No Audio Manager found! Did you start from the Title Screen?");
        }
    }
}