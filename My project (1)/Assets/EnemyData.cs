using UnityEngine;

[CreateAssetMenu(
    fileName = "New Enemy",
    menuName = "Text Game/Enemy"
)]

public class EnemyData : ScriptableObject
{
    [Header("전투 배경")]
    public Sprite battleBackground;

    public string enemyName;
    public string enemyId;

    [TextArea(3, 6)]
    public string description;

    public int maxHp = 30;
    public int attack = 5;

    [Header("승리 보상")]
    public int experienceReward;
    public int goldReward;
    public string itemReward;
}