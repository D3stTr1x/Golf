using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Golf.ScriptsGolf.Effects
{
    public class PinataEffects : MonoBehaviour
    {
        [SerializeField] private AudioSource m_audioSource;

        [Header("Попадание мяча")]
        [SerializeField] private ParticleSystem m_hitParticles;
        [SerializeField] private AudioClip m_hitClip;
        [Tooltip("Сюда можно повесить любые свои эффекты: анимацию качания, вспышку, тряску камеры")]
        [SerializeField] private UnityEvent m_onHit;

        [Header("Победа")]
        [SerializeField] private ParticleSystem m_breakParticles;
        [SerializeField] private AudioClip m_breakClip;
        [SerializeField] private UnityEvent m_onBreak;
        [SerializeField] private GameObject m_target;
        [SerializeField] private float m_hideDelay = 0.3f;

        public void PlayHit()
        {
            if (m_hitParticles != null)
            {
                m_hitParticles.Play();
            }

            PlayClip(m_hitClip);
            m_onHit?.Invoke();
        }

        public void PlayBreak()
        {
            if (m_breakParticles != null)
            {
                m_breakParticles.Play();
            }

            PlayClip(m_breakClip);
            m_onBreak?.Invoke();

            StartCoroutine(RemoveAfterDelay());
        }

        private IEnumerator RemoveAfterDelay()
        {
            yield return new WaitForSeconds(m_hideDelay);

            GameObject target = m_target != null ? m_target : gameObject;

            target.SetActive(false);
        }

        private void PlayClip(AudioClip clip)
        {
            if (m_audioSource != null && clip != null)
            {
                m_audioSource.PlayOneShot(clip);
            }
        }
    }
}
