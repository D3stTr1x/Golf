using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Golf.ScriptsGolf.Buttons
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField] private string m_sceneName = "GameScene";
        [SerializeField] private string m_menuSceneName = "Menu";

        [SerializeField] private TMP_Text m_scoreText;

        private string scene;

        private const string BestScoreKey = "BestScore";
        private const string LastScoreKey = "LastScore";

        private void Awake()
        {
            if (m_scoreText == null)
            {
                Debug.LogError("Назначьте Score TMP_Text в Inspector");
                return;
            }

            int best = PlayerPrefs.GetInt(BestScoreKey, 0);
            int last = PlayerPrefs.GetInt(LastScoreKey, 0);

            m_scoreText.text = $"Рекорд: {best}\nПоследний: {last}";
        }

        private void Update()
        {
            scene = SceneManager.GetActiveScene().name;
        }

        public void ContnueBut()
        {
            /*menuImg.gameObject.SetActive(false);
            menuButton1.gameObject.SetActive(false);
            menuButton2.gameObject.SetActive(false);
            menuButton3.gameObject.SetActive(false);

            checkMenu = false;
            */

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ToMenu()
        {
            /*
            menuImg.gameObject.SetActive(false);
            menuButton1.gameObject.SetActive(false);
            menuButton2.gameObject.SetActive(false); 
            menuButton3.gameObject.SetActive(false);
            */

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SceneManager.LoadScene(m_menuSceneName);
        }

        public void LoadScene()
        {
            SceneManager.LoadScene(m_sceneName);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ExitGame()
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit();
    #endif
        }
        /*
        public void OpenMenu()
        {
            if ((scene != "MainMenu") && (!checkMenu) && (Input.GetKeyDown(KeyCode.Escape)))
            {
                menuImg.gameObject.SetActive(true);
                menuButton1.gameObject.SetActive(true);
                menuButton2.gameObject.SetActive(true);
                menuButton3.gameObject.SetActive(true);

                menuButton1.interactable = true;
                menuButton2.interactable = true;
                menuButton3.interactable = true;

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                checkMenu = true;
            }
            else if ((checkMenu) && (Input.GetKeyDown(KeyCode.Escape)))
            {
                ContnueBut();
            }
        }
        */
    }
}
