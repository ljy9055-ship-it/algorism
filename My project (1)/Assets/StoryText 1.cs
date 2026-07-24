using UnityEngine;

[CreateAssetMenu(
    fileName = "New Story",
    menuName = "Text Game/Story Node"
)]
public class StoryNode : ScriptableObject
{
    public enum StoryNodeType
    {
        Dialogue,   // 일반 대화
        Choice      // 선택지
    }
    [Header("Node Type")]
    public StoryNodeType nodeType;

    [Header("Dialogue Next Node")]
    public StoryNode nextNode;
    [Header("Save ID")]
    public string nodeId;

    [Header("Story")]
    [TextArea(5, 10)]
    public string storyText;

    [Header("Background")]
    public Sprite backgroundImage;

    [Header("Node Effects")]
    public int hpChange;
    public int goldChange;
    public string itemToGive;

    [Header("Battle")]
    public EnemyData enemy;
    public StoryNode victoryNode;
    public StoryNode defeatNode;
    public StoryNode escapeNode;

    [Header("Choices")]
    public Choice[] choices;

    [Header("One Time Event")]
    public string eventId;
    public bool runOnlyOnce;

    public Sprite characterSprite;

}

[System.Serializable]
public class Choice
{
    public string buttonText;

    [Header("다음 스토리")]
    public StoryNode nextNode;

    public bool useRandomNextNode;
    public StoryNode[] randomNextNodes;

    [Header("선택 조건")]
    public string requiredItem;
    public int requiredGold;

    public string requiredEventId;

    [Header("선택 결과")]
    public int hpChange;
    public int goldChange;
    public string itemToGive;
    public string itemToRemove;
    [Header("장비")]
    public EquipmentData equipmentToGive;
    public bool equipImmediately;
}