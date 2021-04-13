using UnityEngine;

public class RPGResourceNodeRankData : ScriptableObject
{
    public int ID = -1;
    public int unlockCost;

    public int lootTableID = -1;
    public RPGLootTable lootTableREF;

    public int skillLevelRequired;

    public int Experience;

    public float distanceMax;

    public float gatherTime;
    public float respawnTime;


    public void copyData(RPGResourceNodeRankData original, RPGResourceNodeRankData copied)
    {
        original.unlockCost = copied.unlockCost;
        original.lootTableID = copied.lootTableID;
        original.skillLevelRequired = copied.skillLevelRequired;
        original.Experience = copied.Experience;
        original.distanceMax = copied.distanceMax;
        original.gatherTime = copied.gatherTime;
        original.respawnTime = copied.respawnTime;
    }

    public void updateThis(RPGResourceNodeRankData newDATA)
    {
        ID = newDATA.ID;
        unlockCost = newDATA.unlockCost;
        lootTableID = newDATA.lootTableID;
        skillLevelRequired = newDATA.skillLevelRequired;
        Experience = newDATA.Experience;
        distanceMax = newDATA.distanceMax;
        gatherTime = newDATA.gatherTime;
        respawnTime = newDATA.respawnTime;
    }
}