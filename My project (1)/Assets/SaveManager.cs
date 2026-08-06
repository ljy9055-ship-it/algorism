using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class SaveManager : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private StoryManager storyManager;
    [SerializeField] private StoryDatabase storyDatabase;
    [SerializeField] private BattleManager battleManager;
    [SerializeField] private GameSettings gameSettings;
    [SerializeField] private EquipmentDatabase equipmentDatabase;

    private List<EventCounter> CopyEventCounters(
    List<EventCounter> source
)
    {
        List<EventCounter> result =
            new List<EventCounter>();

        if (source == null)
            return result;

        foreach (EventCounter counter in source)
        {
            if (counter == null)
                continue;

            result.Add(new EventCounter
            {
                eventId = counter.eventId.Trim(),
                count = Mathf.Max(0, counter.count)
            });
        }

        return result;
    }
    private List<ItemStackData> CreateInventorySaveData(
    PlayerData player
)
    {
        List<ItemStackData> result =
            new List<ItemStackData>();

        if (player == null)
            return result;

        foreach (
            KeyValuePair<string, int> pair
            in player.Inventory
        )
        {
            if (string.IsNullOrWhiteSpace(pair.Key))
                continue;

            if (pair.Value <= 0)
                continue;

            result.Add(
                new ItemStackData
                {
                    itemId = pair.Key,
                    amount = pair.Value
                }
            );
        }

        return result;
    }
    private string SavePath
    {
        get
        {
            return Path.Combine(
                Application.persistentDataPath,
                "save.json"
            );
        }
    }
    private void ReplaceList(
    List<string> target,
    List<string> savedList
)
    {
        target.Clear();

        if (savedList != null)
        {
            target.AddRange(savedList);
        }
    }
    public void SaveGame()
    {
        if (PlayerData.Instance == null)
        {
            Debug.LogError("PlayerData가 없습니다.");
            return;
        }

        if (storyManager == null || storyManager.currentNode == null)
        {
            Debug.LogError("현재 StoryNode가 없습니다.");
            return;
        }

        PlayerData player = PlayerData.Instance;

        SaveData saveData = new SaveData
        {
            saveVersion = 1,

            currentNodeId = storyManager.currentNode.nodeId,

            playerName = player.playerName,
            level = player.level,
            experience = player.experience,

            hp = player.hp,
            maxHp = player.maxHp,
            attack = player.attack,
            defense = player.defense,
            gold = player.gold,

            inventory =
    CreateInventorySaveData(player),

            equippedWeaponId =
    player.equippedWeapon != null
        ? player.equippedWeapon.equipmentId
        : string.Empty,

            equippedArmorId =
    player.equippedArmor != null
        ? player.equippedArmor.equipmentId
        : string.Empty,
            equippedAccessoryId =
    player.equippedAccessory != null
        ? player.equippedAccessory.equipmentId
        : string.Empty,

            completedEventIds =
                new List<string>(player.completedEventIds),

            openedChestIds =
                new List<string>(player.openedChestIds),

            defeatedEnemyIds =
                new List<string>(player.defeatedEnemyIds),

            completedQuestIds =
                new List<string>(player.completedQuestIds),
            
            eventCounters =
            CopyEventCounters(player.eventCounters),

            battle = CreateBattleSaveData(),

            settings = CreateSettingsSaveData()
        };


        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            File.WriteAllText(SavePath, json);
            Debug.Log($"저장 완료: {SavePath}");
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"저장 실패: {exception.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning("저장 파일이 없습니다.");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData == null)
            {
                Debug.LogError("저장 데이터를 읽지 못했습니다.");
                return;
            }

            StoryNode savedNode =
                storyDatabase.GetNode(saveData.currentNodeId);

            if (savedNode == null)
            {
                Debug.LogError(
                    $"저장된 StoryNode를 찾지 못했습니다: {saveData.currentNodeId}"
                );
                return;
            }

            RestorePlayerData(saveData);
            RestoreSettings(saveData.settings);

            storyManager.LoadStoryNode(savedNode);

            Debug.Log("불러오기 완료");
        }
        catch (System.Exception exception)
        {
            Debug.LogError($"불러오기 실패: {exception.Message}");
        }
    }
    private void RestoreSettings(
    SettingsSaveData savedSettings
)
    {
        if (gameSettings == null || savedSettings == null)
            return;

        gameSettings.masterVolume =
            savedSettings.masterVolume;

        gameSettings.bgmVolume =
            savedSettings.bgmVolume;

        gameSettings.effectVolume =
            savedSettings.effectVolume;

        gameSettings.fullscreen =
            savedSettings.fullscreen;

        gameSettings.textSpeed =
            savedSettings.textSpeed;

        gameSettings.ApplySettings();
    }

    private void RestorePlayerData(SaveData saveData)
    {
        PlayerData player = PlayerData.Instance;

        player.playerName = saveData.playerName;
        player.level = saveData.level;
        player.experience = saveData.experience;

        player.maxHp = saveData.maxHp;
        player.hp = Mathf.Clamp(
            saveData.hp,
            0,
            player.maxHp
        );

        player.attack = saveData.attack;
        player.defense = saveData.defense;
        player.gold = Mathf.Max(0, saveData.gold);

        player.ClearInventory();

        if (saveData.inventory != null)
        {
            foreach (ItemStackData savedItem in saveData.inventory)
            {
                if (savedItem == null)
                    continue;

                if (string.IsNullOrWhiteSpace(savedItem.itemId))
                    continue;

                if (savedItem.amount <= 0)
                    continue;

                player.SetItemCount(
                    savedItem.itemId,
                    savedItem.amount
                );
            }
        }

        if (equipmentDatabase != null)
        {
            player.equippedWeapon =
                equipmentDatabase.GetEquipment(
                    saveData.equippedWeaponId
                );

            player.equippedArmor =
                equipmentDatabase.GetEquipment(
                    saveData.equippedArmorId
                );
            player.equippedAccessory =
    equipmentDatabase.GetEquipment(
        saveData.equippedAccessoryId
    );
        }
        else
        {
            Debug.LogWarning(
                "EquipmentDatabase가 SaveManager에 연결되지 않았습니다."
            );

            player.equippedWeapon = null;
            player.equippedArmor = null;
            player.equippedAccessory = null;
        }

        ReplaceList(
            player.completedEventIds,
            saveData.completedEventIds
        );

        ReplaceList(
            player.openedChestIds,
            saveData.openedChestIds
        );

        ReplaceList(
            player.defeatedEnemyIds,
            saveData.defeatedEnemyIds
        );

        ReplaceList(
            player.completedQuestIds,
            saveData.completedQuestIds
        );
        player.eventCounters.Clear();

        if (saveData.eventCounters != null)
        {
            foreach (EventCounter savedCounter in saveData.eventCounters)
            {
                if (savedCounter == null)
                    continue;

                if (string.IsNullOrWhiteSpace(savedCounter.eventId))
                    continue;

                player.eventCounters.Add(
                    new EventCounter
                    {
                        eventId = savedCounter.eventId.Trim(),
                        count = Mathf.Max(0, savedCounter.count)
                    }
                );
            }
        }
        player.hp = Mathf.Clamp(
    player.hp,
    0,
    player.MaxHp
);
    }

    public void DeleteSave()
    {
        if (!File.Exists(SavePath))
        {
            Debug.Log("삭제할 저장 파일이 없습니다.");
            return;
        }

        File.Delete(SavePath);
        Debug.Log("저장 파일을 삭제했습니다.");
    }

    public bool HasSaveFile()
    {
        return File.Exists(SavePath);
    }
    private BattleSaveData CreateBattleSaveData()
    {
        if (battleManager == null || !battleManager.IsInBattle)
        {
            return new BattleSaveData
            {
                isInBattle = false
            };
        }

        return new BattleSaveData
        {
            isInBattle = true,

            enemyId = battleManager.CurrentEnemyId,
            enemyCurrentHp = battleManager.CurrentEnemyHp,

            victoryNodeId =
                storyManager.currentNode.victoryNode != null
                    ? storyManager.currentNode.victoryNode.nodeId
                    : string.Empty,

            defeatNodeId =
                storyManager.currentNode.defeatNode != null
                    ? storyManager.currentNode.defeatNode.nodeId
                    : string.Empty,

            escapeNodeId =
                storyManager.currentNode.escapeNode != null
                    ? storyManager.currentNode.escapeNode.nodeId
                    : string.Empty
        };
    }
    private SettingsSaveData CreateSettingsSaveData()
    {
        if (gameSettings == null)
        {
            return new SettingsSaveData();
        }

        return new SettingsSaveData
        {
            masterVolume = gameSettings.masterVolume,
            bgmVolume = gameSettings.bgmVolume,
            effectVolume = gameSettings.effectVolume,
            fullscreen = gameSettings.fullscreen,
            textSpeed = gameSettings.textSpeed
        };
    }


}