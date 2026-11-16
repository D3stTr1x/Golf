using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Golf.ScriptsGolf.UI
{
    public class WinMessage : MonoBehaviour
    {
        [SerializeField] private GameObject m_root;
        [SerializeField] private TMP_Text m_text;
        [SerializeField] private string m_message = "Mission passed +RESPECT";
        [Tooltip("Анимация появления и т. п.")]
        [SerializeField] private UnityEvent m_onShown;

        [Header("Победная озвучка")]
        [Tooltip("Основная музыка: на время победного звука ставится на паузу")]
        [SerializeField] private AudioSource m_music;
        [Tooltip("Источник для победного звука (не тот же, что у музыки)")]
        [SerializeField] private AudioSource m_winAudio;
        [SerializeField] private AudioClip m_winClip;
        [Tooltip("Включить музыку снова, когда победный звук закончится")]
        [SerializeField] private bool m_resumeMusicAfter = false;

        public void Hide()
        {
            if (m_root != null)
            {
                m_root.SetActive(false);
            }
        }

        public void Show()
        {
            if (m_text != null)
            {
                m_text.text = m_message;
            }

            if (m_root != null)
            {
                m_root.SetActive(true);
            }

            PlayWinSound();
            m_onShown?.Invoke();
        }

        private void PlayWinSound()
        {
            if (m_music != null)
            {
                m_music.Pause();
            }

            if (m_winAudio == null || m_winClip == null)
            {
                return;
            }

            m_winAudio.PlayOneShot(m_winClip);

            if (m_resumeMusicAfter && m_music != null)
            {
                StartCoroutine(ResumeMusic(m_winClip.length));
            }
        }

        private IEnumerator ResumeMusic(float delay)
        {
            yield return new WaitForSecondsRealtime(delay);
            m_music.UnPause();
        }
    }
}
