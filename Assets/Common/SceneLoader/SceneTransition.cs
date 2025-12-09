using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private Animator anim;
    [SerializeField] private AnimationClip transition_anim;

    public void transition_to(string scene_name)
    {
        anim.Play(transition_anim.name);
        StartCoroutine(scene_name);
    }

    private IEnumerator next_scene(string scene_name)
    {
        yield return new WaitForSeconds(transition_anim.length);
        SceneManager.LoadScene(scene_name);
    }
}
