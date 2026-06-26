using UnityEngine;

public class AutoItemCollector : MonoBehaviour
{
    private PlayerInventory inventory;

    void Awake()
    {
        inventory = GetComponent<PlayerInventory>();
    }

    public void CollectFromMonster(GameObject monster)
    {
        if (monster == null) return;

        MonsterDropper dropper = monster.GetComponent<MonsterDropper>();

        if (dropper != null)
        {
            dropper.DropTo(inventory);
        }
    }
}