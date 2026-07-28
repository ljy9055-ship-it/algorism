using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

public class StoryDatabase : ScriptableObject
{
    [SerializeField]
    private List<StoryNode> nodes = new List<StoryNode>();

    public StoryNode GetNode(string nodeId)
    {
        if (string.IsNullOrWhiteSpace(nodeId))
            return null;

        nodeId = nodeId.Trim();

        StoryNode foundNode = nodes.Find(node =>
            node != null &&
            !string.IsNullOrWhiteSpace(node.nodeId) &&
            node.nodeId.Trim() == nodeId
        );

        if (foundNode == null)
        {
            Debug.LogError(
                $"StoryDatabase에서 ID가 '{nodeId}'인 노드를 찾지 못했습니다."
            );
        }

        return foundNode;
    }

#if UNITY_EDITOR
    [ContextMenu("StoryNode 전체 자동 등록")]
    public void RefreshNodesFromProject()
    {
        string[] nodeGuids =
            AssetDatabase.FindAssets("t:StoryNode");

        List<StoryNode> foundNodes =
            nodeGuids
                .Select(guid =>
                {
                    string path =
                        AssetDatabase.GUIDToAssetPath(guid);

                    return AssetDatabase.LoadAssetAtPath<StoryNode>(
                        path
                    );
                })
                .Where(node => node != null)
                .OrderBy(node => node.nodeId)
                .ToList();

        // ID 중복 검사
        var duplicateIds = foundNodes
            .Where(node =>
                !string.IsNullOrWhiteSpace(node.nodeId))
            .GroupBy(node => node.nodeId.Trim())
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        foreach (string duplicateId in duplicateIds)
        {
            Debug.LogError(
                $"중복된 StoryNode ID가 있습니다: {duplicateId}"
            );
        }

        Undo.RecordObject(
            this,
            "StoryDatabase 자동 등록"
        );

        nodes = foundNodes;

        EditorUtility.SetDirty(this);

        Debug.Log(
            $"StoryDatabase에 StoryNode {nodes.Count}개를 자동 등록했습니다."
        );
    }
#endif
}