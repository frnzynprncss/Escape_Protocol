using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jumpscare : MonoBehaviour
{
    [SerializeField] public GameObject jumpscarePanel;
    public float displayDuration = 5f; // Duration to display the jumpscare

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartCoroutine(ShowJumpscare());
        }
    }

    public IEnumerator ShowJumpscare()
    {
        jumpscarePanel.SetActive(true); // Show the jumpscare image
        yield return new WaitForSeconds(displayDuration); // Wait for the specified duration
        jumpscarePanel.SetActive(false); // Hide the jumpscare image
    }
}
