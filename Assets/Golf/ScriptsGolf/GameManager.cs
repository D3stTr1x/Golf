using UnityEngine;
using System.Collections.Generic;
using Golf.ScriptsGolf.SpawnRocks;
using Golf.ScriptsGolf.ShootScripts;

namespace Golf.ScriptsGolf
{
    public class GameManager : MonoBehaviour
    {
        [Header("Спавн камней")]
        [SerializeField] private GameObject[] m_rockPrefabs;
        [SerializeField] private Transform m_spawnPoint;

        [Header("Игрок")]
        [SerializeField] private Animator m_playerAnimator;
        [SerializeField] private Transform m_player;

        [Header("Удар по камню")]
        [SerializeField] private BoxCollider m_hoeHeadCollider;
        [SerializeField] private LayerMask m_rockLayer;
        [SerializeField] private float m_hitForce = 8f;
        [SerializeField] private float m_upwardForce = 1.5f;

        [Header("Направление удара")]
        [Tooltip("Мировая ось, вдоль которой мотыга движется вперёд при ударе")]
        [SerializeField] private Vector3 m_swingDirection = Vector3.forward;
        [Tooltip("Минимальная скорость головы вперёд вдоль оси удара")]
        [SerializeField] private float m_minHitSpeed = 2f;
        [Tooltip("0..1, чем больше, тем меньше сглаживание скорости")]
        [SerializeField, Range(0f, 1f)] private float m_velocitySmoothing = 0.5f;

        private readonly HashSet<int> m_hitRocks = new HashSet<int>();

        private IGameState m_currentState;
        private GolfShootAnimation m_golfShootAnimation;

        private Vector3 m_prevHeadPos;
        private Vector3 m_headVelocity;
        private bool m_hasPrevPos;

        private void Awake()
        {
            if (m_player == null && m_playerAnimator != null)
            {
                m_player = m_playerAnimator.transform;
            }

            if (m_playerAnimator != null && m_player != null)
            {
                m_golfShootAnimation = new GolfShootAnimation(
                    m_playerAnimator,
                    m_player,
                    this,
                    "Shoot",
                    "Idle"
                );
            }
            else
            {
                Debug.LogError(
                    "GameManager: назначьте Player Animator и Player Transform в Inspector."
                );
            }

            ChangeState(new GamePlayState(this));
        }

        private void Update()
        {
            if (m_golfShootAnimation == null)
            {
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                m_hitRocks.Clear();
                m_golfShootAnimation.Press();
            }

            if (Input.GetMouseButtonUp(0))
            {
                m_golfShootAnimation.Release();
            }
        }

        private void LateUpdate()
        {
            if (m_golfShootAnimation == null || m_hoeHeadCollider == null)
            {
                return;
            }

            if (!m_golfShootAnimation.IsPlaying)
            {
                m_hasPrevPos = false;
                m_headVelocity = Vector3.zero;
                return;
            }

            Vector3 headPos = m_hoeHeadCollider.transform.position;

            Vector3 rawVelocity = (m_hasPrevPos && Time.deltaTime > 0f)
                ? (headPos - m_prevHeadPos) / Time.deltaTime
                : Vector3.zero;

            m_headVelocity = Vector3.Lerp(m_headVelocity, rawVelocity, m_velocitySmoothing);

            m_prevHeadPos = headPos;
            m_hasPrevPos = true;
        }

        private void FixedUpdate()
        {
            if (m_golfShootAnimation == null ||
                !m_golfShootAnimation.IsPlaying ||
                m_hoeHeadCollider == null)
            {
                return;
            }

            Vector3 swingDir = m_swingDirection.normalized;

            // Бьём только при движении вперёд: назад и в паузе скорость меньше порога.
            if (Vector3.Dot(m_headVelocity, swingDir) < m_minHitSpeed)
            {
                return;
            }

            Bounds bounds = m_hoeHeadCollider.bounds;
            Collider[] hits = Physics.OverlapBox(
                bounds.center,
                bounds.extents,
                Quaternion.identity,
                m_rockLayer,
                QueryTriggerInteraction.Ignore
            );

            Vector3 hitDir = (swingDir + Vector3.up * m_upwardForce).normalized;

            foreach (Collider hit in hits)
            {
                Rigidbody rockBody = hit.attachedRigidbody;

                if (rockBody == null || rockBody.isKinematic)
                {
                    continue;
                }

                if (!m_hitRocks.Add(rockBody.GetInstanceID()))
                {
                    continue;
                }

                rockBody.AddForce(hitDir * m_hitForce, ForceMode.Impulse);
            }
        }

        private void ChangeState(IGameState newState)
        {
            m_currentState?.Exit();
            m_currentState = newState;
            m_currentState.Enter();
        }

        private interface IGameState
        {
            void Enter();
            void Exit();
        }

        private class GamePlayState : IGameState
        {
            private readonly GameManager m_manager;
            private Coroutine m_spawnRoutine;

            public GamePlayState(GameManager manager)
            {
                m_manager = manager;
            }

            public void Enter()
            {
                m_spawnRoutine = m_manager.StartCoroutine(
                    RockSpawner.SpawnRocks(
                        m_manager.m_rockPrefabs,
                        m_manager.m_spawnPoint
                    )
                );
            }

            public void Exit()
            {
                if (m_spawnRoutine != null)
                {
                    m_manager.StopCoroutine(m_spawnRoutine);
                }
            }
        }
    }
}