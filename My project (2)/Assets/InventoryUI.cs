using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("Inventory Data")]
    [SerializeField] private List<InventoryItem> inventoryItems;

    [Header("Item List")]
    [SerializeField] private Transform content;
    [SerializeField] private ItemSlotUI itemSlotPrefab;

    [Header("Item Detail")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TextMeshProUGUI detailNameText;
    [SerializeField] private TextMeshProUGUI detailDescriptionText;
    [SerializeField] private TextMeshProUGUI detailAmountText;
    [SerializeField] private Button useButton;

    [Header("Player")]
    [SerializeField] private PlayerData playerData;
    [SerializeField] private HUDUI hudUI;

    [SerializeField] private ToastMessageUI toastMessage;
    private ItemSlotUI selectedSlot;
    private InventoryItem selectedItem;

    private void Start()
    {
        RefreshInventory();
        ClearDetail();
    }

    public void RefreshInventory()
    {
        selectedSlot = null;
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        foreach (InventoryItem item in inventoryItems)
        {
            if (item.amount <= 0)
                continue;

            ItemSlotUI slot = Instantiate(itemSlotPrefab, content);
            slot.Setup(item, this);
        }

        if (selectedItem != null)
        {
            ShowDetail(selectedItem);
        }
    }

    public void SelectItem(InventoryItem item, ItemSlotUI slot)
    {
        if (selectedSlot != null)
        {
            selectedSlot.SetSelected(false);
        }

        selectedItem = item;
        selectedSlot = slot;

        selectedSlot.SetSelected(true);

        ShowDetail(item);
    }

    private void ShowDetail(InventoryItem item)
    {
        detailIcon.sprite = item.itemData.icon;
        detailIcon.enabled = item.itemData.icon != null;

        detailNameText.text = item.itemData.itemName;
        detailDescriptionText.text = item.itemData.description;
        detailAmountText.text = $"보유 수량: {item.amount}";

        useButton.interactable = item.itemData.canUse && item.amount > 0;
    }

    public void UseSelectedItem()
    {
        if (selectedItem == null)
            return;

        if (!selectedItem.itemData.canUse)
            return;

        if (selectedItem.amount <= 0)
            return;

        string usedItemName = selectedItem.itemData.itemName;

        if (selectedItem.itemData.healAmount > 0)
        {
            playerData.Heal(selectedItem.itemData.healAmount);
        }

        selectedItem.amount--;

        toastMessage.ShowMessage(
            $"{usedItemName}을 사용했습니다."
        );

        RefreshInventory();
        hudUI.Refresh();

        if (selectedItem.amount <= 0)
        {
            selectedItem = null;
            ClearDetail();
        }
    }
    public int GetTotalItemCount()
    {
        int total = 0;

        foreach (InventoryItem item in inventoryItems)
        {
            total += item.amount;
        }

        return total;
    }

    private void ClearDetail()
    {
        detailIcon.sprite = null;
        detailIcon.enabled = false;

        detailNameText.text = "아이템을 선택하세요";
        detailDescriptionText.text = "";
        detailAmountText.text = "";

        useButton.interactable = false;
    }
}