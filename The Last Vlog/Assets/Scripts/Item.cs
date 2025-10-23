using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum InteractionType { None, PickUp, Examine }
    public InteractionType type;

    private void Reset()
    {
        GetComponent <Collider2D> ().isTrigger = true;
        gameObject.layer = 10;
    }

    public void Interact()
    {
        switch (type)
        {
            case InteractionType.PickUp:
                //Add the object to the PickedUpItems list in InteractionSystem
                FindObjectOfType<InteractionSystem>().PickItem(gameObject);
                //Disable the object in the scene
                gameObject.SetActive(false);
                break;
            case InteractionType.Examine:
                Debug.Log("Examining item: " + gameObject.name);
                break;
            default:
                Debug.Log("Null item");
                break;
        }
    }
}
