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
        [SerializeField] private GameObject m_menuBackgroud;

        [SerializeField] private GameObject m_pauseButton;
        [SerializeField] private GameObject m_backButton;

        [SerializeField] private GameObject[] m_menuButtons;
        [SerializeField] private GameObject[] m_settingsButton;

        [SerializeField] private GameObject[] m_lights;
        [SerializeField] private Light m_addLight;


        private string scene;

        private const string BestScoreKey = "BestScore";
        private const string LastScoreKey = "LastScore";

        private bool checkMenu = false;

        private void Awake()
        {
            int best = PlayerPrefs.GetInt(BestScoreKey, 0);
            int last = PlayerPrefs.GetInt(LastScoreKey, 0);
            if (m_scoreText != null)
            {
                m_scoreText.text = $"Рекорд: {best}\nПоследний: {last}";
            }
        }

        private void Update()
        {
            scene = SceneManager.GetActiveScene().name;
            OpenMenuInteractive();
        }

        public void ContnueBut()
        {
            if (m_menuBackgroud != null)
            {
                m_menuBackgroud.gameObject.SetActive(false);
            }
            
            CloseSettings();
            m_pauseButton.gameObject.SetActive(true);

            Time.timeScale = 1f;

            checkMenu = false;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void ToMenu()
        {
            if (m_menuBackgroud != null)
            {
                m_menuBackgroud.gameObject.SetActive(false);
            }

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
        
        public void OpenMenu()
        {
            if ((scene == m_sceneName) && (!checkMenu))
            {
                Time.timeScale = 0f;
                m_pauseButton.gameObject.SetActive(false);
                m_menuBackgroud.gameObject.SetActive(true);

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                checkMenu = true;
            }
            else if (checkMenu)
            {
                m_pauseButton.gameObject.SetActive(true);
                ContnueBut();
            }
        }

        private void OpenMenuInteractive()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OpenMenu();
            }
        }

        public void OpenSettings()
        {
            m_backButton.gameObject.SetActive(true);
            foreach (GameObject button in m_menuButtons)
            {
                button.gameObject.SetActive(false);
            }
            foreach (GameObject button in m_settingsButton)
            {
                button.gameObject.SetActive(true);
            }
        }

        public void CloseSettings()
        {
            m_backButton.gameObject.SetActive(false);
            foreach (GameObject button in m_settingsButton)
            {
                button.gameObject.SetActive(false);
            }
            foreach (GameObject button in m_menuButtons)
            {
                button.gameObject.SetActive(true);
            }
            
        }

        public void LightSettings()
        {
            foreach (GameObject light in m_lights)
            {
                if (light.gameObject.activeSelf)
                {
                    light.gameObject.SetActive(false);
                    m_addLight.range = 5f;
                }
                else
                {
                    light.gameObject.SetActive(true);
                    m_addLight.range = 10f;
                }
            }
        }
    }
}
