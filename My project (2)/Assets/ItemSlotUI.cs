using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI amountText;
    [SerializeField] private Button button;
    private Image backgroundImage;

    private InventoryItem inventoryItem;
    private InventoryUI inventoryUI;

    private Color normalColor;
    [SerializeField] private Color selectedColor = Color.yellow;

    private void Awake()
    {
        backgroundImage = GetComponent<Image>();

        if (backgroundImage == null)
        {
            Debug.LogError("ItemSlot에 Image 컴포넌트가 없습니다.");
            return;
        }

        normalColor = backgroundImage.color;
    }

    public void Setup(InventoryItem item, InventoryUI ui)
    {
        inventoryItem = item;
        inventoryUI = ui;

        itemIcon.sprite = item.itemData.icon;
        itemNameText.text = item.itemData.itemName;
        amountText.text = $"x{item.amount}";

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClickSlot);

        SetSelected(false);
    }

    private void OnClickSlot()
    {
        inventoryUI.SelectItem(inventoryItem, this);
    }

    public void SetSelected(bool selected)
    {
        backgroundImage.color = selected
            ? selectedColor
            : normalColor;
    }
}