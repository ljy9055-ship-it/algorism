using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDUI : MonoBehaviour
{
    [SerializeField] private PlayerData playerData;

    [Header("HUD")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI itemCountText;

    [SerializeField] private InventoryUI inventoryUI;

    private void Start()
    {
        healthSlider.minValue = 0;
        healthSlider.maxValue = playerData.maxHp;

        Refresh();
    }

    public void Refresh()
    {
        healthSlider.maxValue = playerData.maxHp;
        healthSlider.value = playerData.currentHp;

        healthText.text =
            $"{playerData.currentHp} / {playerData.maxHp}";

        goldText.text =
            $"Gold : {playerData.gold}";

        itemCountText.text =
            $"Items : {inventoryUI.GetTotalItemCount()}";
    }
}