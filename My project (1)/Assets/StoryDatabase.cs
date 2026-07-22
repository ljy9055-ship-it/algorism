using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(
    fileName = "Story Database",
    menuName = "Text Game/Story Database"
)]
public class StoryDatabase : ScriptableObject
{
    public List<StoryNode> storyNodes = new List<StoryNode>();

    public StoryNode GetNode(string nodeId)
    {
        foreach (StoryNode node in storyNodes)
        {
            if (node != null && node.nodeId == nodeId)
            {
                return node;
            }
        }

        Debug.LogError(
            $"StoryDatabase에서 ID가 '{nodeId}'인 노드를 찾지 못했습니다."
        );

        return null;
    }
}