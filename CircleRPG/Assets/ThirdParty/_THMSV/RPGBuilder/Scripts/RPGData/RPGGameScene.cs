using UnityEngine;

public class RPGGameScene : ScriptableObject
{
    public int ID = -1;
    public string _name;
    public string _fileName;
    public string displayName;
    public string description;

    public Sprite loadingBG;
    public Sprite minimapImage;
    public Bounds mapBounds;
    public Vector2 mapSize;

    public int startPositionID = -1;
    public RPGWorldPosition startPositionREF;
    

    public void updateThis(RPGGameScene newData)
    {
        ID = newData.ID;
        _name = newData._name;
        _fileName = newData._fileName;
        description = newData.description;
        displayName = newData.displayName;
        loadingBG = newData.loadingBG;
        minimapImage = newData.minimapImage;
        mapBounds = newData.mapBounds;
        mapSize = newData.mapSize;
        startPositionID = newData.startPositionID;
    }
}