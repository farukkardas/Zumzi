using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace FirstScene
{
    public class LevelButton : MonoBehaviour
    {
        public int levelIndex;
        [SerializeField] private TMP_Text levelText;

        void Start()
        {
            levelText.text = levelIndex.ToString();
        }

        public void DoLevelChange()
        {
            SceneManager.LoadSceneAsync($"Level{levelIndex}");
        }
    }
}