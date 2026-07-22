using UnityEngine;

[CreateAssetMenu(
    fileName = "New Story",
    menuName = "Text Game/Story Node"
)]
public class StoryNode : ScriptableObject
{
    [Header("Save ID")]
    public string nodeId;

    [Header("Story")]
    [TextArea(5, 10)]
    public string storyText;

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
}

[System.Serializable]
public class Choice
{
    public string buttonText;
    public StoryNode nextNode;

    [Header("Requirements")]
    public string requiredItem;
    public int requiredGold;

    [Header("Effects")]
    public int hpChange;
    public int goldChange;
    public string itemToGive;
    public string itemToRemove;
}