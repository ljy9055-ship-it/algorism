using System;

[Serializable]
public class BattleSaveData
{
    public bool isInBattle;

    public string enemyId;
    public int enemyCurrentHp;

    public string victoryNodeId;
    public string defeatNodeId;
    public string escapeNodeId;

}