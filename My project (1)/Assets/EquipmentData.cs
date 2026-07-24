using UnityEngine;

public enum EquipmentType
{
    Weapon,
    Armor,
    Accessory
}

[CreateAssetMenu(
    fileName = "New Equipment",
    menuName = "Text Game/Equipment"
)]
public class EquipmentData : ScriptableObject
{
    [Header("Identification")]
    public string equipmentId;
    public string equipmentName;

    [TextArea]
    public string description;

    public EquipmentType equipmentType;

    [Header("Stats")]
    public int attackBonus;
    public int defenseBonus;
    public int maxHpBonus;

    public Sprite icon;
}