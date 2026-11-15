using UnityEngine;

namespace Golf.ScriptsGolf.Buttons
{
    public class SettingsMenu : MonoBehaviour
    {
        [SerializeField] private GameObject m_backButton;
        [SerializeField] private GameObject[] m_menuButtons;
        [SerializeField] private GameObject[] m_settingsButtons;

        public void Open()
        {
            m_backButton.SetActive(true);
            SetActive(m_menuButtons, false);
            SetActive(m_settingsButtons, true);
        }

        public void Close()
        {
            m_backButton.SetActive(false);
            SetActive(m_settingsButtons, false);
            SetActive(m_menuButtons, true);
        }

        private static void SetActive(GameObject[] objects, bool active)
        {
            foreach (GameObject go in objects)
            {
                go.SetActive(active);
            }
        }
    }
}
