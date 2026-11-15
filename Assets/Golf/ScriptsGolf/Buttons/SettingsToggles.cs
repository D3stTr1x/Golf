using UnityEngine;

namespace Golf.ScriptsGolf.Buttons
{
    public class SettingsToggles : MonoBehaviour
    {
        private const float c_LightOnRange = 10f;
        private const float c_LightOffRange = 5f;

        [SerializeField] private AudioSource m_music;
        [SerializeField] private GameObject[] m_lights;
        [SerializeField] private Light m_addLight;

        private readonly GameSettingsStorage m_settings = new GameSettingsStorage();

        private void Awake()
        {
            ApplyMusic();
            ApplyLight();
        }

        public void ToggleMusic()
        {
            m_settings.MusicOn = !m_settings.MusicOn;
            ApplyMusic();
        }

        public void ToggleLight()
        {
            m_settings.LightOn = !m_settings.LightOn;
            ApplyLight();
        }

        private void ApplyMusic()
        {
            if (m_music != null)
            {
                m_music.mute = !m_settings.MusicOn;
            }
        }

        private void ApplyLight()
        {
            bool on = m_settings.LightOn;

            foreach (GameObject lamp in m_lights)
            {
                if (lamp != null)
                {
                    lamp.SetActive(on);
                }
            }

            if (m_addLight != null)
            {
                m_addLight.range = on ? c_LightOnRange : c_LightOffRange;
            }
        }
    }
}
