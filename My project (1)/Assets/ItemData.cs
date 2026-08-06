using UnityEngine;

public enum ItemType
{
    Healing,
    Damage,
    Buff,
    Key,
    Material
}

[CreateAssetMenu(
    fileName = "New Item",
    menuName = "Text Game/Item"
)]
public class ItemData : ScriptableObject
{
    [Header("기본 정보")]
    public string itemId;
    public string itemName;

    [TextArea(3, 5)]
    public string description;

    public Sprite icon;
    public ItemType itemType;

    [Header("사용 설정")]
    public bool usableInBattle;
    public bool consumable = true;

    [Header("효과")]
    public int healAmount;
    public int damageAmount;
    public int attackBuff;
    public int defenseBuff;
}