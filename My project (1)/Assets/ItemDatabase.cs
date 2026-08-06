using System.Collections.Generic;
using UnityEngine;

public class ItemDatabase : MonoBehaviour
{
    [SerializeField]
    private List<ItemData> items = new List<ItemData>();

    public ItemData GetItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
            return null;

        itemId = itemId.Trim();

        ItemData item = items.Find(data =>
            data != null &&
            !string.IsNullOrWhiteSpace(data.itemId) &&
            data.itemId.Trim() == itemId
        );

        if (item == null)
        {
            Debug.LogWarning(
                $"ItemDatabase에서 '{itemId}' 아이템을 찾지 못했습니다."
            );
        }

        return item;
    }
}