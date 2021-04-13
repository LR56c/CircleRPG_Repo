using THMSV.RPGBuilder.Managers;
using TMPro;
using UnityEngine;

namespace THMSV.RPGBuilder.UIElements
{
    public class CharacterSlotHolder : MonoBehaviour
    {
        public TextMeshProUGUI CharacterNameText, LevelText, RaceText, ClassText;

        public void Init(CharacterData charData)
        {
            CharacterNameText.text = charData.CharacterName;
            LevelText.text = "LvL. " + charData.classDATA.currentClassLevel;
            RaceText.text = RPGBuilderUtilities.GetRaceFromID(charData.raceID).displayName;
            ClassText.text = RPGBuilderUtilities.GetClassFromID(charData.classDATA.classID).displayName;
        }

        public void SelectCharacter()
        {
            MainMenuManager.Instance.SelectCharacter(CharacterNameText.text);
        }
    }
}