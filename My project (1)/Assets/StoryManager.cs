using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static StoryNode;

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

    [Header("대화 진행")]
    [SerializeField] private Button nextButton;

    [SerializeField] private Image characterImage;

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

        if (characterImage != null)
        {
            characterImage.sprite = currentNode.characterSprite;
            characterImage.enabled = currentNode.characterSprite != null;
        }

        ApplyNodeEffects(currentNode);
        UpdateStatusUI();

        if (currentNode.enemy != null)
        {
            StartBattle();
            return;
        }

        storyPanel.SetActive(true);
        storyText.text = currentNode.storyText;

        UpdateStoryBackground(currentNode.backgroundImage);
        UpdateNodeUI();
    }

    private void UpdateNodeUI()
    {
        if (currentNode == null)
            return;

        // 다음 버튼 이벤트 초기화
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            nextButton.gameObject.SetActive(false);
        }

        HideAllChoiceButtons();

        switch (currentNode.nodeType)
        {
            case StoryNodeType.Dialogue:
                ShowDialogueNode();
                break;

            case StoryNodeType.Choice:
                UpdateChoiceButtons();
                break;
        }
    }

    private void HideAllChoiceButtons()
    {
        foreach (Button button in choiceButtons)
        {
            if (button == null)
                continue;

            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }
    }

    private void ShowDialogueNode()
    {
        if (nextButton == null)
        {
            Debug.LogError("Next Button이 연결되지 않았습니다.");
            return;
        }

        nextButton.gameObject.SetActive(true);
        nextButton.interactable = true;

        TMP_Text nextButtonText =
            nextButton.GetComponentInChildren<TMP_Text>();

        if (nextButtonText != null)
        {
            nextButtonText.text =
                currentNode.nextNode != null ? "다음" : "끝";
        }

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(GoToNextDialogue);
    }

    private void GoToNextDialogue()
    {
        if (currentNode == null)
            return;

        StoryNode nextNode = currentNode.nextNode;

        if (nextNode == null)
        {
            EndStory();
            return;
        }

        currentNode = nextNode;
        ShowNode();
    }

    private void EndStory()
    {
        storyText.text = "이야기가 종료되었습니다.";

        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }

        HideAllChoiceButtons();
        UpdateStatusUI();
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

    private void ApplyNodeEffects(StoryNode node)
    {
        if (node == null)
            return;

        PlayerData player = PlayerData.Instance;

        if (player == null)
        {
            Debug.LogError("PlayerData.Instance를 찾을 수 없습니다.");
            return;
        }

        string eventId = node.eventId?.Trim();

        Debug.Log(
            $"[이벤트 진입] " +
            $"노드={node.nodeId}, " +
            $"이벤트ID=[{eventId}], " +
            $"1회성={node.runOnlyOnce}, " +
            $"완료됨={player.IsEventCompleted(eventId)}"
        );

        if (node.runOnlyOnce)
        {
            if (string.IsNullOrWhiteSpace(eventId))
            {
                Debug.LogError(
                    $"노드 '{node.nodeId}'는 Run Only Once가 켜져 있지만 Event ID가 비어 있습니다."
                );

                return;
            }

            if (player.IsEventCompleted(eventId))
            {
                Debug.Log($"이미 실행된 이벤트이므로 효과를 건너뜁니다: {eventId}");
                return;
            }
        }

        if (node.hpChange != 0)
        {
            player.ChangeHp(node.hpChange);
        }

        if (node.goldChange != 0)
        {
            player.ChangeGold(node.goldChange);
        }

        if (!string.IsNullOrWhiteSpace(node.itemToGive))
        {
            player.AddItem(node.itemToGive);
        }

        if (node.runOnlyOnce)
        {
            player.CompleteEvent(eventId);

            Debug.Log(
                $"이벤트 기록 완료: {eventId}\n" +
                $"현재 완료 이벤트: {string.Join(", ", player.completedEventIds)}"
            );
        }

        UpdateStatusUI();
    }

    private void UpdateChoiceButtons()
    {
        if (currentNode == null)
        {
            Debug.LogError("현재 StoryNode가 없습니다.");
            return;
        }

        PlayerData player = PlayerData.Instance;

        if (player == null)
        {
            Debug.LogError("PlayerData.Instance를 찾을 수 없습니다.");
            return;
        }

        // 기존 버튼 상태와 클릭 이벤트 초기화
        foreach (Button button in choiceButtons)
        {
            if (button == null)
                continue;

            button.onClick.RemoveAllListeners();
            button.gameObject.SetActive(false);
        }

        if (currentNode.choices == null)
            return;

        // 실제 화면에 사용할 버튼 번호
        int buttonIndex = 0;

        // 노드에 들어 있는 모든 선택지를 순서대로 검사
        for (int choiceIndex = 0;
             choiceIndex < currentNode.choices.Length;
             choiceIndex++)
        {
            Choice choice = currentNode.choices[choiceIndex];

            if (choice == null)
                continue;

            // Hide After Complete 선택지가 이미 완료됐다면 건너뜀
            if (choice.hideAfterComplete)
            {
                string completeId = choice.completeEventId?.Trim();

                bool completed =
                    !string.IsNullOrWhiteSpace(completeId) &&
                    player.IsEventCompleted(completeId);

                if (completed)
                {
                    Debug.Log(
                        $"[선택지 숨김] " +
                        $"선택지={choice.buttonText}, " +
                        $"ID=[{completeId}]"
                    );

                    continue;
                }
            }

            // 필요한 이벤트 조건을 만족하지 못하면 건너뜀
            if (!CanShowChoice(choice))
                continue;

            // 사용할 수 있는 버튼을 모두 채웠다면 종료
            if (buttonIndex >= choiceButtons.Length)
                break;

            Button button = choiceButtons[buttonIndex];

            if (button == null)
            {
                buttonIndex++;
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

            // 화면 버튼 번호가 아니라 원래 Choice 배열 번호를 저장
            int capturedChoiceIndex = choiceIndex;

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                SelectChoice(capturedChoiceIndex);
            });

            // 다음 표시 버튼으로 이동
            buttonIndex++;
        }

        // 사용되지 않은 나머지 버튼 숨기기
        for (int i = buttonIndex; i < choiceButtons.Length; i++)
        {
            if (choiceButtons[i] == null)
                continue;

            choiceButtons[i].onClick.RemoveAllListeners();
            choiceButtons[i].gameObject.SetActive(false);
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

        // 선택지를 사용했다는 기록을 먼저 저장
        if (selectedChoice.hideAfterComplete)
        {
            string completeId = selectedChoice.completeEventId?.Trim();

            if (string.IsNullOrWhiteSpace(completeId))
            {
                Debug.LogError(
                    $"'{selectedChoice.buttonText}' 선택지는 Hide After Complete가 체크되어 있지만 " +
                    "Complete Event Id가 비어 있습니다."
                );
            }
            else
            {
                PlayerData.Instance.CompleteEvent(completeId);

                Debug.Log(
                    $"[선택지 완료 저장] " +
                    $"선택지={selectedChoice.buttonText}, " +
                    $"ID=[{completeId}], " +
                    $"저장 결과={PlayerData.Instance.IsEventCompleted(completeId)}"
                );
            }
        }

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

        if (choice.equipmentToGive != null)
        {
            player.AddEquipment(choice.equipmentToGive);

            if (choice.equipImmediately)
            {
                player.Equip(choice.equipmentToGive);
            }
        }
    }

    private void UpdateStatusUI()
    {
        if (statusText == null)
            return;

        PlayerData player = PlayerData.Instance;

        if (player == null)
            return;

        string weaponName =
            player.equippedWeapon != null
                ? player.equippedWeapon.equipmentName
                : "없음";

        string armorName =
            player.equippedArmor != null
                ? player.equippedArmor.equipmentName
                : "없음";

        statusText.text =
            $"체력 : {player.hp}/{player.MaxHp}\n" +
            $"골드 : {player.gold}\n" +
            $"공격력 : {player.Attack}\n" +
            $"방어력 : {player.Defense}\n" +
            $"무기 : {weaponName}\n" +
            $"방어구 : {armorName}";
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
            Debug.LogError("불러온 StoryNode가 없습니다.");
            return;
        }

        currentNode = loadedNode;
        ShowLoadedNode();
    }

    private void ShowLoadedNode()
    {
        if (currentNode == null)
            return;

        if (characterImage != null)
        {
            characterImage.sprite = currentNode.characterSprite;
            characterImage.enabled = currentNode.characterSprite != null;
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

        UpdateStoryBackground(currentNode.backgroundImage);
        UpdateNodeUI();
    }

    private bool CanShowChoice(Choice choice)
    {
        if (choice == null)
            return false;

        if (string.IsNullOrWhiteSpace(choice.requiredEventId))
            return true;

        PlayerData player = PlayerData.Instance;

        if (player == null)
            return false;

        return player.IsEventCompleted(choice.requiredEventId.Trim());
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

        // 배열에 들어 있는 null 노드를 제외하고 계산
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

        Debug.Log($"랜덤 스토리 선택: {selectedNode.nodeId}");

        return selectedNode;
    }
}