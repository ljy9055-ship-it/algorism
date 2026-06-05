using UnityEngine;

public class graphgizmovisualizer : MonoBehaviour
{
    [Header("Nodes")]
    [Tooltip("그래프 노드들의 위치입니다. 각 원소가 Scene 뷰의 점 하나가 됩니다.")]
    [SerializeField]
    private Vector3[] nodePositions =
    {
        new Vector3(0f, 0f, 0f),
        new Vector3(2f, 0f, 1f),
        new Vector3(4f, 0f, 0f),
        new Vector3(2f, 0f, -2f)
    };

    [Header("Edges")]
    [Tooltip("연결할 노드 번호 쌍입니다. 예: (0, 1)은 0번 노드와 1번 노드를 연결합니다.")]
    [SerializeField]
    private Vector2Int[] edges =
    {
        new Vector2Int(0, 1),
        new Vector2Int(1, 2),
        new Vector2Int(1, 3),
        new Vector2Int(3, 2)
    };

    [Tooltip("노드를 Scene 뷰에 그릴 때 사용할 크기입니다.")]
    [SerializeField] private float nodeRadius = 0.2f;

    private void OnDrawGizmos()
    {
        if (nodePositions == null)
        {
            return;
        }

        DrawEdges();
        DrawNodes();
    }

    private void DrawNodes()
    {
        for (int i = 0; i < nodePositions.Length; i++)
        {
            Vector3 worldPosition = transform.position + nodePositions[i];

            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(worldPosition, nodeRadius);

            Gizmos.color = Color.black;
            Gizmos.DrawWireSphere(worldPosition, nodeRadius + 0.04f);
        }
    }

    private void DrawEdges()
    {
        if (edges == null)
        {
            return;
        }

        Gizmos.color = Color.white;

        foreach (Vector2Int edge in edges)
        {
            if (!IsValidNodeIndex(edge.x) || !IsValidNodeIndex(edge.y))
            {
                continue;
            }

            Vector3 from = transform.position + nodePositions[edge.x];
            Vector3 to = transform.position + nodePositions[edge.y];
            Gizmos.DrawLine(from, to);
        }
    }

    private bool IsValidNodeIndex(int index)
    {
        return index >= 0 && index < nodePositions.Length;
    }
}