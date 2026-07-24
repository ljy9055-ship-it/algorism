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
    public List<string> inventory = new List<string>();

    [Header("Equipment")]
    public string equippedWeaponId;
    public string equippedArmorId;
    public string equippedAccessoryId;
    [Header("Progress")]
    public List<string> completedEventIds = new List<string>();
    public List<string> openedChestIds = new List<string>();
    public List<string> defeatedEnemyIds = new List<string>();
    public List<string> completedQuestIds = new List<string>();

    [Header("Battle")]
    public BattleSaveData battle = new BattleSaveData();

    [Header("Settings")]
    public SettingsSaveData settings = new SettingsSaveData();
}