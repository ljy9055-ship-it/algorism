using System;
using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public string itemName;

    [Header("Effect")]
    public bool increasespeed;
    public float speedbonus = 0f;

    PlayerMover playerMover;
    private void OnTriggerEnter(Collider other)
    {
        PlayerInventory inventory = other.GetComponent<PlayerInventory>();

        if (inventory != null)
        {
            playerMover.IncreaseSpeed(speedbonus);
            Debug.Log("이동속도 증가!");
            inventory.AddItem(itemName);
            Destroy(gameObject);
        }
    }
}