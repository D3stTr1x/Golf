using UnityEngine;

namespace Golf.ScriptsGolf.Buttons
{
    public class PauseMenu : MonoBehaviour
    {
        [SerializeField] private GameObject m_menuBackground;
        [SerializeField] private GameObject m_pauseButton;
        [SerializeField] private SettingsMenu m_settingsMenu;

        private bool m_isPaused;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Toggle();
            }
        }

        public void Toggle()
        {
            if (m_isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }

        public void Pause()
        {
            if (m_isPaused)
            {
                return;
            }

            m_isPaused = true;
            Time.timeScale = 0f;

            m_pauseButton.SetActive(false);
            m_menuBackground.SetActive(true);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void Resume()
        {
            m_isPaused = false;
            Time.timeScale = 1f;

            m_menuBackground.SetActive(false);
            m_settingsMenu.Close();
            m_pauseButton.SetActive(true);

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
