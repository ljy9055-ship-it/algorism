using System.Collections.Generic;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    [Header("Basic")]
    public string playerName = "플레이어";
    public int level = 1;
    public int experience;

  

    [Header("Stats")]
    public int hp = 100;
    public int maxHp = 100;
    public int attack = 10;
    public int defense = 2;
    public int gold;

    [Header("Inventory")]
    public List<string> inventory = new List<string>();
    public List<EquipmentData> ownedEquipments =
    new List<EquipmentData>();

    [Header("장착 장비")]
    public EquipmentData equippedWeapon;
    public EquipmentData equippedArmor;
    public EquipmentData equippedAccessory;



    [Header("Progress")]
    public List<string> completedEventIds = new List<string>();
    public List<string> openedChestIds = new List<string>();
    public List<string> defeatedEnemyIds = new List<string>();
    public List<string> completedQuestIds = new List<string>();



    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddEquipment(EquipmentData equipment)
    {
        if (equipment == null)
            return;

        if (!ownedEquipments.Contains(equipment))
        {
            ownedEquipments.Add(equipment);
            Debug.Log($"{equipment.equipmentName} 획득");
        }
    }
    public void Equip(EquipmentData equipment)
    {
        if (equipment == null)
            return;

        switch (equipment.equipmentType)
        {
            case EquipmentType.Weapon:
                equippedWeapon = equipment;
                break;

            case EquipmentType.Armor:
                equippedArmor = equipment;
                break;

            case EquipmentType.Accessory:
                equippedAccessory = equipment;
                break;
        }

        if (hp > MaxHp)
            hp = MaxHp;

        Debug.Log(
            $"{equipment.equipmentName} 장착 완료 / " +
            $"기본 공격력: {attack} / " +
            $"장비 공격력: {equipment.attackBonus} / " +
            $"최종 공격력: {Attack}"
        );
    }
    public int Attack
    {
        get
        {
            int bonus = 0;

            if (equippedWeapon != null)
                bonus += equippedWeapon.attackBonus;

            if (equippedArmor != null)
                bonus += equippedArmor.attackBonus;

            if (equippedAccessory != null)
                bonus += equippedAccessory.attackBonus;

            return attack + bonus;
        }
    }

    public int Defense
    {
        get
        {
            int bonus = 0;

            if (equippedWeapon != null)
                bonus += equippedWeapon.defenseBonus;

            if (equippedArmor != null)
                bonus += equippedArmor.defenseBonus;

            if (equippedAccessory != null)
                bonus += equippedAccessory.defenseBonus;

            return defense + bonus;
        }
    }
    public void Unequip(EquipmentType equipmentType)
    {
        switch (equipmentType)
        {
            case EquipmentType.Weapon:
                equippedWeapon = null;
                break;

            case EquipmentType.Armor:
                equippedArmor = null;
                break;

            case EquipmentType.Accessory:
                equippedAccessory = null;
                break;
        }

        if (hp > MaxHp)
            hp = MaxHp;
    }
    public int MaxHp
    {
        get
        {
            int bonus = 0;

            if (equippedWeapon != null)
                bonus += equippedWeapon.maxHpBonus;

            if (equippedArmor != null)
                bonus += equippedArmor.maxHpBonus;

            if (equippedAccessory != null)
                bonus += equippedAccessory.maxHpBonus;

            return maxHp + bonus;
        }
    }
    public void AddExperience(int amount)
    {
        if (amount <= 0)
            return;

        experience += amount;

        CheckLevelUp();
    }
    private void CheckLevelUp()
    {
        int requiredExperience = level * 100;

        while (experience >= requiredExperience)
        {
            experience -= requiredExperience;
            level++;

            maxHp += 10;
            hp = maxHp;
            attack += 2;
            defense += 1;

            requiredExperience = level * 100;
        }
    }
    public void AddItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return;

        inventory.Add(itemId);
    }

    public bool HasItem(string itemId)
    {
        return inventory.Contains(itemId);
    }

    public bool RemoveItem(string itemId)
    {
        return inventory.Remove(itemId);
    }

   

    public bool IsEventCompleted(string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            return false;

        eventId = eventId.Trim();

        return completedEventIds.Contains(eventId);
    }

    public void CompleteEvent(string eventId)
    {
        if (string.IsNullOrWhiteSpace(eventId))
            return;

        eventId = eventId.Trim();

        if (!completedEventIds.Contains(eventId))
        {
            completedEventIds.Add(eventId);
            Debug.Log($"완료 이벤트 추가: {eventId}");
        }
    }

    public void OpenChest(string chestId)
    {
        AddUnique(openedChestIds, chestId);
    }

    public bool IsChestOpened(string chestId)
    {
        return openedChestIds.Contains(chestId);
    }

    public void DefeatEnemy(string enemyId)
    {
        AddUnique(defeatedEnemyIds, enemyId);
    }

    public bool IsEnemyDefeated(string enemyId)
    {
        return defeatedEnemyIds.Contains(enemyId);
    }

    public void CompleteQuest(string questId)
    {
        AddUnique(completedQuestIds, questId);
    }

    public bool IsQuestCompleted(string questId)
    {
        return completedQuestIds.Contains(questId);
    }

    private void AddUnique(List<string> list, string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return;

        if (!list.Contains(id))
            list.Add(id);
    }
    public void ChangeHp(int amount)
    {
        hp += amount;
        hp = Mathf.Clamp(hp, 0, MaxHp);

        Debug.Log($"현재 체력: {hp}/{MaxHp}");
    }

    public void ChangeGold(int amount)
    {
        gold += amount;
        gold = Mathf.Max(0, gold);

        Debug.Log($"현재 골드: {gold}");
    }
}