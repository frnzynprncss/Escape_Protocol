using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource audio_prefab;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void play_sound(AudioClip clip, Transform audio_transform, float volume)
    {
        AudioSource audio_source = Instantiate(audio_prefab, audio_transform.position, Quaternion.identity);

        audio_source.clip = clip;
        audio_source.volume = volume;
        audio_source.Play();

        float clip_length = clip.length;
        Destroy(audio_source, clip_length);
    }

    public void play_rand_sound(AudioClip[] clips, Transform audio_transform, float volume)
    {
        AudioClip rand_clip = clips[Random.Range(0, clips.Length - 1)];
        play_sound(rand_clip, audio_transform, volume);
    }
}
