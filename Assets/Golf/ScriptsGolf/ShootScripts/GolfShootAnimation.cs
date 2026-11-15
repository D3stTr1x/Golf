using Golf.ScriptsGolf.States.Interfaces;
using UnityEngine;

namespace Golf.ScriptsGolf.ShootScripts
{
    public class GolfShootAnimation : IUpdatable
    {
        private static readonly int ShootTrigger = Animator.StringToHash("Shoot");

        private enum Phase
        {
            None,
            WaitingForShootState,
            Shooting,
            Holding,
            WaitingForIdleState
        }

        private readonly Animator m_animator;
        private readonly Transform m_player;
        private readonly int m_shootStateHash;
        private readonly int m_idleStateHash;
        private readonly float m_defaultAnimatorSpeed;

        private Phase m_phase = Phase.None;
        private Vector3 m_originalPosition;
        private Quaternion m_originalRotation;
        private bool m_buttonHeld;

        public bool IsPlaying => m_phase != Phase.None;

        public GolfShootAnimation(
            Animator animator,
            Transform player,
            string shootStateName = "Shoot",
            string idleStateName = "Idle")
        {
            m_animator = animator;
            m_player = player;
            m_shootStateHash = Animator.StringToHash(shootStateName);
            m_idleStateHash = Animator.StringToHash(idleStateName);
            m_defaultAnimatorSpeed = animator.speed;
        }

        public void Press()
        {
            m_buttonHeld = true;

            if (IsPlaying)
            {
                return;
            }

            m_originalPosition = m_player.position;
            m_originalRotation = m_player.rotation;

            m_animator.ResetTrigger(ShootTrigger);
            m_animator.SetTrigger(ShootTrigger);

            m_phase = Phase.WaitingForShootState;
        }

        public void Release()
        {
            m_buttonHeld = false;

            if (m_animator.speed == 0f)
            {
                m_animator.speed = m_defaultAnimatorSpeed;
            }
        }

        public void Update()
        {
            if (m_phase == Phase.WaitingForShootState && IsInState(m_shootStateHash))
            {
                m_phase = Phase.Shooting;
            }

            if (m_phase == Phase.Shooting)
            {
                AnimatorStateInfo state = m_animator.GetCurrentAnimatorStateInfo(0);
                bool shootFinished = state.shortNameHash != m_shootStateHash || state.normalizedTime >= 1f;

                if (shootFinished)
                {
                    if (m_buttonHeld)
                    {
                        m_animator.speed = 0f;
                        m_phase = Phase.Holding;
                    }
                    else
                    {
                        StartReturn();
                    }
                }
            }
            else if (m_phase == Phase.Holding && !m_buttonHeld)
            {
                m_animator.speed = m_defaultAnimatorSpeed;
                StartReturn();
            }

            if (m_phase == Phase.WaitingForIdleState && IsInState(m_idleStateHash))
            {
                m_player.SetPositionAndRotation(m_originalPosition, m_originalRotation);
                m_animator.speed = m_defaultAnimatorSpeed;
                m_phase = Phase.None;
            }
        }

        public bool IsInHitWindow(float start, float end)
        {
            AnimatorStateInfo state = m_animator.GetCurrentAnimatorStateInfo(0);

            return state.shortNameHash == m_shootStateHash &&
                state.normalizedTime >= start &&
                state.normalizedTime <= end;
        }

        private void StartReturn()
        {
            m_phase = Phase.WaitingForIdleState;
        }

        private bool IsInState(int shortNameHash)
        {
            return m_animator.GetCurrentAnimatorStateInfo(0).shortNameHash == shortNameHash;
        }
    }
}
