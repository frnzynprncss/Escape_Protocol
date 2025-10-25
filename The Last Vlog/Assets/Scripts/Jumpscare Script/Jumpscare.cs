using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    [SerializeField] public GameObject jumpscarePanel;
    public float displayDuration = 5f; // Duration to display the jumpscare

    public void ShowJumpscare()
    {
        StartCoroutine(scare());
    }

    private IEnumerator scare()
    {
        jumpscarePanel.SetActive(true);
        yield return new WaitForSeconds(displayDuration);
        jumpscarePanel.SetActive(false);
    }
}
