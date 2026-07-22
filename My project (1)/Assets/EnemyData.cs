using UnityEngine;

[CreateAssetMenu(
    fileName = "New Enemy",
    menuName = "Text Game/Enemy"
)]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public string enemyId;

    [TextArea(3, 6)]
    public string description;

    public int maxHp = 30;
    public int attack = 5;

    [Header("Rewards")]
    public int goldReward = 10;
    public string itemReward;
}