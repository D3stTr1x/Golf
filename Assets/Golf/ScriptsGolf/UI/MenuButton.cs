using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

namespace Golf.ScriptsGolf.Buttons
{
    public class MenuButton : MonoBehaviour
    {
        [SerializeField] private string m_sceneName = "GameScene";
        [SerializeField] private string m_menuSceneName = "Menu";
        [SerializeField] private AudioSource m_music;
        
        private const string MusicKey = "MusicOn";
        private const string LightKey = "LightOn";

        private bool m_musicOn = true;
        private bool m_lightOn = true;

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

            m_musicOn = PlayerPrefs.GetInt(MusicKey, 1) == 1;
            m_lightOn = PlayerPrefs.GetInt(LightKey, 1) == 1;

            ApplyMusic();
            ApplyLight();
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

        public void MusicSettings()
        {
            m_musicOn = !m_musicOn;
            PlayerPrefs.SetInt(MusicKey, m_musicOn ? 1 : 0);
            PlayerPrefs.Save();
            ApplyMusic();
        }

        public void LightSettings()
        {
            m_lightOn = !m_lightOn;
            PlayerPrefs.SetInt(LightKey, m_lightOn ? 1 : 0);
            PlayerPrefs.Save();
            ApplyLight();
        }

        private void ApplyMusic()
        {
            if (m_music != null)
            {
                m_music.mute = !m_musicOn;
            }
        }

        private void ApplyLight()
        {
            foreach (GameObject lamp in m_lights)
            {
                if (lamp != null)
                {
                    lamp.SetActive(m_lightOn);
                }
            }

            if (m_addLight != null)
            {
                m_addLight.range = m_lightOn ? 10f : 5f;
            }
        }
    }
}
