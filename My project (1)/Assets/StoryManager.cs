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
    [SerializeField] private PlayerData playerData;
    [Header("스토리 배경")]
    [SerializeField] private Image storyBackgroundImage;

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
    private void UpdateStoryBackground(Sprite backgroundSprite)
    {
        if (storyBackgroundImage == null)
        {
            Debug.LogWarning("스토리 배경 Image가 연결되지 않았습니다.");
            return;
        }

        if (backgroundSprite == null)
        {
            storyBackgroundImage.enabled = false;
            return;
        }

        storyBackgroundImage.sprite = backgroundSprite;
        storyBackgroundImage.enabled = true;
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
        if (currentNode == null)
            return;

        bool alreadyCompleted =
            !string.IsNullOrEmpty(currentNode.eventId) &&
            playerData.IsEventCompleted(currentNode.eventId);

        if (currentNode.runOnlyOnce && alreadyCompleted)
        {
            return;
        }

        playerData.ChangeHp(currentNode.hpChange);
        playerData.ChangeGold(currentNode.goldChange);

        if (!string.IsNullOrEmpty(currentNode.itemToGive))
        {
            playerData.AddItem(currentNode.itemToGive);
        }

        if (!string.IsNullOrEmpty(currentNode.eventId))
        {
            playerData.CompleteEvent(currentNode.eventId);
        }
    }


    private void UpdateChoiceButtons()
    {
        if (currentNode == null)
        {
            Debug.LogError("현재 StoryNode가 없습니다.");
            return;
        }

        for (int i = 0; i < choiceButtons.Length; i++)
        {
            Button button = choiceButtons[i];

            if (button == null)
                continue;

            button.onClick.RemoveAllListeners();

            if (currentNode.choices == null ||
                i >= currentNode.choices.Length)
            {
                button.gameObject.SetActive(false);
                continue;
            }

            Choice choice = currentNode.choices[i];

            // 필요한 이벤트를 보지 않았다면 버튼 숨기기
            if (!CanShowChoice(choice))
            {
                button.gameObject.SetActive(false);
                continue;
            }

            button.gameObject.SetActive(true);
            button.interactable = CanSelectChoice(choice);

            TMP_Text buttonText =
                button.GetComponentInChildren<TMP_Text>();

            if (buttonText != null)
            {
                buttonText.text = choice.buttonText;
            }

            int capturedIndex = i;

            button.onClick.AddListener(() =>
            {
                SelectChoice(capturedIndex);
            });
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
        if (currentNode == null ||
            currentNode.choices == null ||
            index < 0 ||
            index >= currentNode.choices.Length)
        {
            Debug.LogError("잘못된 선택지 인덱스입니다.");
            return;
        }

        Choice selectedChoice = currentNode.choices[index];

        if (selectedChoice == null)
        {
            Debug.LogError("선택지 데이터가 없습니다.");
            return;
        }

        if (!CanSelectChoice(selectedChoice))
            return;

        ApplyChoiceEffects(selectedChoice);

        currentNode = GetNextNode(selectedChoice);

        if (currentNode != null)
        {
            ShowNode();
        }
        else
        {
            storyText.text = "이야기가 종료되었습니다.";

            foreach (Button button in choiceButtons)
            {
                if (button != null)
                {
                    button.gameObject.SetActive(false);
                }
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
    private bool CanShowChoice(Choice choice)
    {
        if (choice == null)
            return false;

        // 필요한 이벤트가 지정되지 않았다면 표시
        if (string.IsNullOrEmpty(choice.requiredEventId))
            return true;

        // 해당 이벤트를 완료했을 때만 표시
        return playerData.IsEventCompleted(choice.requiredEventId);
    }
    private StoryNode GetNextNode(Choice choice)
    {
        if (choice == null)
            return null;

        // 랜덤 이동을 사용하지 않는 경우
        if (!choice.useRandomNextNode)
        {
            return choice.nextNode;
        }

        // 랜덤 노드 배열이 비어 있는 경우 기본 노드 사용
        if (choice.randomNextNodes == null ||
            choice.randomNextNodes.Length == 0)
        {
            Debug.LogWarning(
                $"'{choice.buttonText}' 선택지의 랜덤 노드가 비어 있습니다. " +
                "기본 Next Node로 이동합니다."
            );

            return choice.nextNode;
        }

        // 배열에 들어 있는 null 노드를 제외하고 선택
        int validNodeCount = 0;

        foreach (StoryNode node in choice.randomNextNodes)
        {
            if (node != null)
            {
                validNodeCount++;
            }
        }

        if (validNodeCount == 0)
        {
            Debug.LogWarning(
                $"'{choice.buttonText}' 선택지의 랜덤 노드가 모두 비어 있습니다."
            );

            return choice.nextNode;
        }

        StoryNode[] validNodes = new StoryNode[validNodeCount];
        int validIndex = 0;

        foreach (StoryNode node in choice.randomNextNodes)
        {
            if (node == null)
                continue;

            validNodes[validIndex] = node;
            validIndex++;
        }

        int randomIndex = Random.Range(0, validNodes.Length);

        StoryNode selectedNode = validNodes[randomIndex];

        Debug.Log(
            $"랜덤 스토리 선택: {selectedNode.nodeId}"
        );

        return selectedNode;
    }
}