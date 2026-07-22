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

    [Header("Equipment")]
    public string equippedWeaponId;
    public string equippedArmorId;

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

    public void CompleteEvent(string eventId)
    {
        AddUnique(completedEventIds, eventId);
    }

    public bool IsEventCompleted(string eventId)
    {
        return completedEventIds.Contains(eventId);
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
        hp = Mathf.Clamp(hp, 0, maxHp);

        Debug.Log($"현재 체력: {hp}/{maxHp}");
    }

    public void ChangeGold(int amount)
    {
        gold += amount;
        gold = Mathf.Max(0, gold);

        Debug.Log($"현재 골드: {gold}");
    }
}