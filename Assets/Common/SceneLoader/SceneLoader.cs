using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public Animator anim;

    public void transition_to(string scene_name)
    {
        anim.Play("transition");
        StartCoroutine(change_scene(scene_name));
    }

    private IEnumerator change_scene(string scene_name)
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(scene_name);
    }
}
