using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemName;

    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            inventory.AddItem(itemName);
            Destroy(gameObject);
        }
    }
}