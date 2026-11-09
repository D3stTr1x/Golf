using System.Collections;
using UnityEngine;

namespace Golf.ScriptsGolf.ShootScripts
{
    public class GolfShootAnimation
    {
        private readonly Animator m_animator;
        private readonly Transform m_player;
        private readonly MonoBehaviour m_coroutineRunner;
        private readonly string m_shootStateName;
        private readonly string m_idleStateName;

        private Coroutine m_routine;
        private Vector3 m_originalPosition;
        private Quaternion m_originalRotation;
        private float m_originalAnimatorSpeed;
        private bool m_buttonHeld;

        public bool IsPlaying { get; private set; }

        public GolfShootAnimation(
            Animator animator,
            Transform player,
            MonoBehaviour coroutineRunner,
            string shootStateName = "Shoot",
            string idleStateName = "Idle")
        {
            m_animator = animator;
            m_player = player;
            m_coroutineRunner = coroutineRunner;
            m_shootStateName = shootStateName;
            m_idleStateName = idleStateName;
        }

        public void Press()
        {
            m_buttonHeld = true;

            if (IsPlaying)
            {
                return;
            }

            IsPlaying = true;
            m_originalPosition = m_player.position;
            m_originalRotation = m_player.rotation;
            m_originalAnimatorSpeed = m_animator.speed;

            m_animator.ResetTrigger("Shoot");
            m_animator.ResetTrigger("Return");
            m_animator.SetTrigger("Shoot");

            m_routine = m_coroutineRunner.StartCoroutine(PlayShot());
        }

        public void Release()
        {
            m_buttonHeld = false;

            if (m_animator.speed == 0f)
            {
                m_animator.speed = m_originalAnimatorSpeed;
            }
        }

        private IEnumerator PlayShot()
        {
            while (!m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_shootStateName))
            {
                yield return null;
            }

            AnimatorStateInfo state = m_animator.GetCurrentAnimatorStateInfo(0);

            while (state.IsName(m_shootStateName) && state.normalizedTime < 1f)
            {
                yield return null;
                state = m_animator.GetCurrentAnimatorStateInfo(0);
            }

            if (m_buttonHeld)
            {
                m_animator.speed = 0f;

                while (m_buttonHeld)
                {
                    yield return null;
                }

                m_animator.speed = m_originalAnimatorSpeed;
            }

            m_animator.ResetTrigger("Return");
            m_animator.SetTrigger("Return");

            while (!m_animator.GetCurrentAnimatorStateInfo(0).IsName(m_idleStateName))
            {
                yield return null;
            }

            m_player.SetPositionAndRotation(m_originalPosition, m_originalRotation);

            m_animator.speed = m_originalAnimatorSpeed;
            IsPlaying = false;
            m_routine = null;
        }
        
        public bool IsInHitWindow(float start, float end)
        {
            AnimatorStateInfo state = m_animator.GetCurrentAnimatorStateInfo(0);

            return state.IsName(m_shootStateName) &&
                state.normalizedTime >= start &&
                state.normalizedTime <= end;
        }
    }
}