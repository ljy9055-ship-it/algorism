using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public GameObject inventoryPanel;
    public Transform itemListParent;
    public GameObject itemTextPrefab;
    public PlayerInventory inventory;

    private bool isOpen = false;

    void Start()
    {
        inventoryPanel.SetActive(false);
    }

    void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.iKey.wasPressedThisFrame)
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryPanel.SetActive(isOpen);

        if (isOpen)
        {
            RefreshUI();
        }
    }

    void RefreshUI()
    {
        Debug.Log("RefreshUI 실행됨");

        foreach (Transform child in itemListParent)
        {
            Destroy(child.gameObject);
        }

        Debug.Log("아이템 수 : " + inventory.items.Count);

        foreach (KeyValuePair<string, int> item in inventory.items)
        {
            Debug.Log("UI 생성할 아이템 : " + item.Key);

            GameObject itemText = Instantiate(itemTextPrefab, itemListParent);

            itemText.name = "InventoryItemText";

            TMP_Text text = itemText.GetComponentInChildren<TMP_Text>();
            text.text = item.Key + " x " + item.Value;
            Debug.Log("생성됨: " + itemText.name + " / 부모: " + itemText.transform.parent.name);
        }
        
        
    }
}