using UnityEngine;

public class MonsterDropper : MonoBehaviour
{
    [System.Serializable]
    public class DropItem
    {
        public ItemPickup item;
        public int amount = 1;
        [Range(0f, 1f)] public float dropChance = 1f;
    }

    public DropItem[] dropItems;

    public void DropTo(PlayerInventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogError("PlayerInventory가 없습니다.");
            return;
        }

        foreach (DropItem drop in dropItems)
        {
            if (drop.item == null)
            {
                Debug.LogError("DropItem의 item이 비어 있습니다.");
                continue;
            }

            float randomValue = Random.value;

            if (randomValue <= drop.dropChance)
            {
                for (int i = 0; i < drop.amount; i++)
                {
                    inventory.AddItem(drop.item.itemName);
                }

                Debug.Log(drop.item.itemName + " " + drop.amount + "개 획득");
                
            }
            else
            {
                Debug.Log(drop.item.itemName + " 드롭 실패");
            }
        }
    }
}