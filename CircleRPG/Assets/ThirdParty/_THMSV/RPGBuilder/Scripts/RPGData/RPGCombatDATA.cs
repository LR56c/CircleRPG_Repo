using UnityEngine;

//[CreateAssetMenu(fileName = "New Combat Settings", menuName = "RPG BUILDER/COMBAT SETTINGS")]
public class RPGCombatDATA : ScriptableObject
{
    public string[] StatFunctions;
    public string[] States;
    public string[] UIStatsCategories;

    public float CriticalDamageBonus = 2;

    public int actionBarSlots = 10;

    public enum TARGET_TYPE
    {
        Target,
        Caster
    }
    
    public void updateThis(RPGCombatDATA newData)
    {
        StatFunctions = newData.StatFunctions;
        States = newData.States;
        UIStatsCategories = newData.UIStatsCategories;
        CriticalDamageBonus = newData.CriticalDamageBonus;
        actionBarSlots = newData.actionBarSlots;
    }
}