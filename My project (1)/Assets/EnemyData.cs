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

    [Header("½Â¸® º¸»ó")]
    public int experienceReward;
    public int goldReward;
    public string itemReward;
}