using UnityEngine;
using UnityEngine.SceneManagement;

namespace Golf.ScriptsGolf.Buttons
{
    public class SceneLoader : MonoBehaviour
    {
        [SerializeField] private string m_gameSceneName = "GameScene";
        [SerializeField] private string m_menuSceneName = "Menu";

        public void LoadGame()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            SceneManager.LoadScene(m_gameSceneName);
        }

        public void LoadMenu()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(m_menuSceneName);
        }

        public void ExitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
