using UnityEngine;

[CreateAssetMenu(fileName = "New Item Settings", menuName = "RPG BUILDER/ITEM SETTINGS")]
public class RPGGeneralDATA : ScriptableObject
{
    public bool automaticSave;
    public float automaticSaveDelay;
    public bool automaticSaveOnQuit;

    public void updateThis(RPGGeneralDATA newData)
    {
        automaticSave = newData.automaticSave;
        automaticSaveDelay = newData.automaticSaveDelay;
        automaticSaveOnQuit = newData.automaticSaveOnQuit;
    }
}