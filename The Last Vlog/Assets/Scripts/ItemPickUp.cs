using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    private InventorySystem Is;
    public GameObject item;

    private void Start()
    {
        Is = GameObject.FindGameObjectWithTag("Player").GetComponent<InventorySystem>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            for (int i = 0; i < Is.slots.Length; i++)
            {
                if (Is.isFull[i] == false)
                {
                    Is.isFull[i] = true;
                    Instantiate(item, Is.slots[i].transform.position, Is.slots[i].transform.rotation, Is.slots[i].transform);
                    Destroy(gameObject);
                    break;
                }
            }
        }
    }
}
