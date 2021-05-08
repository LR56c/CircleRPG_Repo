using UnityEngine;
using UnityEngine.SceneManagement;

namespace Code
{
    public class World
    {
        private int _currentLevel = 1;
        private int _maxScenes    = 0;
        private int _sceneOffset  = 2;
        
        public World(int currentLevel)
        {
            //esto se genera en bootScene
            _currentLevel = currentLevel;
            _maxScenes = SceneManager.sceneCountInBuildSettings;
        }

        public int GetCurrentLevel() => _currentLevel;

        public void CheckCanAddNextLevel()
        {
            //aqui se podria resetear current level para volver a jugar desde el tutorial
            if(_currentLevel + _sceneOffset >= _maxScenes) return;
            PlayerPrefs.SetInt("Level", _currentLevel);
            _currentLevel++;
        }
    }
}