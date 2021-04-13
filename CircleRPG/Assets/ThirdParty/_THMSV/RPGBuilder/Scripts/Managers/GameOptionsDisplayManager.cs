using THMSV.RPGBuilder.LogicMono;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace THMSV.RPGBuilder.Managers
{
    public class GameOptionsDisplayManager : MonoBehaviour
    {
        public CanvasGroup thisCG;
        private bool showing;

        private void Start()
        {
            if (Instance != null) return;
            Instance = this;
        }

        public static GameOptionsDisplayManager Instance { get; private set; }

        public void BackToMainMenu()
        {
            RPGBuilderJsonSaver.SaveCharacterData(CharacterData.Instance.CharacterName, CharacterData.Instance);
            RPGBuilderJsonSaver.SaveRandomItemsData(RandomizedItemsData.Instance);
            Hide();
            RPGBuilderEssentials.Instance.mainGameCanvas.enabled = false;
            RPGBuilderEssentials.Instance.HandleDATAReset();
            CharacterData.Instance.RESET_CHARACTER_DATA(true);
            SceneManager.LoadScene("MainMenu");
        }
        public void QuitGame()
        {
            RPGBuilderJsonSaver.SaveCharacterData(CharacterData.Instance.CharacterName, CharacterData.Instance);
            RPGBuilderJsonSaver.SaveRandomItemsData(RandomizedItemsData.Instance);
            Application.Quit();
        }

        private void Show()
        {
            showing = true;
            RPGBuilderUtilities.EnableCG(thisCG);
            transform.SetAsLastSibling();
        }

        public void Hide()
        {
            gameObject.transform.SetAsFirstSibling();

            showing = false;
            RPGBuilderUtilities.DisableCG(thisCG);
        }

        private void Awake()
        {
            Hide();
        }

        public void Toggle()
        {
            if (showing)
                Hide();
            else
                Show();
        }
    }
}