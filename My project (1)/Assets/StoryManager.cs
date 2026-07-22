using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StoryManager : MonoBehaviour
{
    [Header("UI")]
    public TMP_Text storyText;
    public TMP_Text statusText;
    public Button[] choiceButtons;

    [Header("Start Story")]
    public StoryNode currentNode;
    [Header("Panels")]
    public GameObject storyPanel;

    [Header("Battle")]
    public BattleManager battleManager;

    private void Start()
    {
        ShowNode();
    }

    private void ShowNode()
    {
        if (currentNode == null)
        {
            Debug.LogError("Current Node가 없습니다.");
            return;
        }

        ApplyNodeEffects();
        UpdateStatusUI();

        if (currentNode.enemy != null)
        {
            StartBattle();
            return;
        }

        storyPanel.SetActive(true);
        storyText.text = currentNode.storyText;

        UpdateChoiceButtons();
    }
    private void StartBattle()
    {
        storyPanel.SetActive(false);

        battleManager.StartBattle(
            currentNode.enemy,
            this
        );
    }

    private void ApplyNodeEffects()
    {
        PlayerData player = PlayerData.Instance;

        if (currentNode.runOnlyOnce &&
            !string.IsNullOrWhiteSpace(currentNode.eventId))
        {
            if (player.IsEventCompleted(currentNode.eventId))
            {
                return;
            }
        }

        player.ChangeHp(currentNode.hpChange);
        player.ChangeGold(currentNode.goldChange);

        if (!string.IsNullOrWhiteSpace(currentNode.itemToGive))
        {
            player.AddItem(currentNode.itemToGive);
        }

        if (currentNode.runOnlyOnce &&
            !string.IsNullOrWhiteSpace(currentNode.eventId))
        {
            player.CompleteEvent(currentNode.eventId);
        }
    }

    private void UpdateChoiceButtons()
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i >= currentNode.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(false);
                continue;
            }

            Choice choice = currentNode.choices[i];

            choiceButtons[i].gameObject.SetActive(true);

            TMP_Text buttonText =
                choiceButtons[i].GetComponentInChildren<TMP_Text>();

            buttonText.text = choice.buttonText;

            bool canSelect = CanSelectChoice(choice);
            choiceButtons[i].interactable = canSelect;

            int index = i;

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].onClick.AddListener(
                () => SelectChoice(index)
            );
        }
    }

    private bool CanSelectChoice(Choice choice)
    {
        PlayerData player = PlayerData.Instance;

        if (!string.IsNullOrWhiteSpace(choice.requiredItem))
        {
            if (!player.HasItem(choice.requiredItem))
                return false;
        }

        if (player.gold < choice.requiredGold)
            return false;

        return true;
    }

    private void SelectChoice(int index)
    {
        Choice selectedChoice = currentNode.choices[index];

        if (!CanSelectChoice(selectedChoice))
            return;

        ApplyChoiceEffects(selectedChoice);

        currentNode = selectedChoice.nextNode;

        if (currentNode != null)
        {
            ShowNode();
        }
        else
        {
            storyText.text = "이야기가 종료되었습니다.";

            foreach (Button button in choiceButtons)
            {
                button.gameObject.SetActive(false);
            }

            UpdateStatusUI();
        }
    }

    private void ApplyChoiceEffects(Choice choice)
    {
        PlayerData player = PlayerData.Instance;

        player.ChangeHp(choice.hpChange);
        player.ChangeGold(choice.goldChange);

        if (!string.IsNullOrWhiteSpace(choice.itemToGive))
        {
            player.AddItem(choice.itemToGive);
        }

        if (!string.IsNullOrWhiteSpace(choice.itemToRemove))
        {
            player.RemoveItem(choice.itemToRemove);
        }
    }


    private void UpdateStatusUI()
    {
        if (statusText == null)
            return;

        PlayerData player = PlayerData.Instance;

        string itemText = player.inventory.Count > 0
            ? string.Join(", ", player.inventory)
            : "없음";

        statusText.text =
            $"체력: {player.hp}\n" +
            $"골드: {player.gold}\n" +
            $"아이템: {itemText}";
    }
    public void FinishBattleVictory()
    {
        battleManager.CloseBattlePanel();

        currentNode = currentNode.victoryNode;
        ShowNode();
    }

    public void FinishBattleDefeat()
    {
        battleManager.CloseBattlePanel();

        currentNode = currentNode.defeatNode;
        ShowNode();
    }

    public void FinishBattleEscape()
    {
        battleManager.CloseBattlePanel();

        currentNode = currentNode.escapeNode;
        ShowNode();
    }
    public void LoadStoryNode(StoryNode loadedNode)
    {
        if (loadedNode == null)
        {
            Debug.LogError("불러올 StoryNode가 없습니다.");
            return;
        }

        currentNode = loadedNode;

        ShowLoadedNode();
    }
    private void ShowLoadedNode()
    {
        if (currentNode == null)
        {
            return;
        }

        UpdateStatusUI();

        if (currentNode.enemy != null)
        {
            StartBattle();
            return;
        }

        storyPanel.SetActive(true);
        battleManager.CloseBattlePanel();

        storyText.text = currentNode.storyText;
        UpdateChoiceButtons();
    }
}