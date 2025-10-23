using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class InteractionSystem : MonoBehaviour
{
    [Header("Dectection Parameters")]
    //detection point
    public Transform detectionPoint;
    //detection radius for interaction
    private const float detectionRadius = 0.2f;
    //detection layer
    public LayerMask detectionLayer;
    //catched trigger object
    public GameObject detectedObject;
    [Header("Others")]
    //list of picked items
    public List<GameObject> pickedItems = new List<GameObject>();

    void Update()
    {
        if (DetectedObject())
        {
            if (InteractInput())
            {
                detectedObject.GetComponent<Item>().Interact();
            }
        }
    }

    bool InteractInput()
    {
        //check for interaction input
        return Input.GetKeyDown(KeyCode.E);
    }

    bool DetectedObject()
    {
        Collider2D obj = Physics2D.OverlapCircle(detectionPoint.position, detectionRadius, detectionLayer);
        if (obj == null)
        {
            detectedObject = null;
            return false;
        }
        else
        {
            detectedObject = obj.gameObject;
            return true;
        }
    }

    public void PickItem(GameObject item)
    {
        pickedItems.Add(item);
    }
}