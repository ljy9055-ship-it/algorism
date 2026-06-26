using System.Collections.Generic;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    public Dictionary<string, int> items = new Dictionary<string, int>();

    public void AddItem(string itemName)
    {
        Debug.Log("AddItem 호출됨 : " + itemName);

        if (items.ContainsKey(itemName))
            items[itemName]++;
        else
            items.Add(itemName, 1);

        Debug.Log("현재 수량 : " + items[itemName]);
    }
}