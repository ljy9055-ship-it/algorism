using UnityEngine;

public class PlayerData : MonoBehaviour
{
    [Header("Player Stats")]
    public int maxHp = 100;
    public int currentHp = 70;
    public int gold = 100;

    public void Heal(int amount)
    {
        currentHp = Mathf.Min(currentHp + amount, maxHp);
    }
}