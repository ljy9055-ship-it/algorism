using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveData
{
    public int saveVersion = 1;

    [Header("Story")]
    public string currentNodeId;

    [Header("Player")]
    public string playerName;
    public int level;
    public int experience;

    public int hp;
    public int maxHp;
    public int attack;
    public int defense;
    public int gold;

    [Header("Inventory")]
    public List<ItemStackData> inventory =
        new List<ItemStackData>();

    [Header("Equipment")]
    public string equippedWeaponId;
    public string equippedArmorId;
    public string equippedAccessoryId;

    [Header("Progress")]
    public List<string> completedEventIds =
        new List<string>();

    public List<string> openedChestIds =
        new List<string>();

    public List<string> defeatedEnemyIds =
        new List<string>();

    public List<string> completedQuestIds =
        new List<string>();

    public List<EventCounter> eventCounters =
        new List<EventCounter>();

    [Header("Battle")]
    public BattleSaveData battle =
        new BattleSaveData();

    [Header("Settings")]
    public SettingsSaveData settings =
        new SettingsSaveData();
}