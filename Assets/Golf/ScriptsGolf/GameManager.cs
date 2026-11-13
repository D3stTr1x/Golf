using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using Golf.ScriptsGolf.SpawnRocks;
using Golf.ScriptsGolf.ShootScripts;
using Golf.ScriptsGolf.UI;
using TMPro;



namespace Golf.ScriptsGolf
{
    public class GameManager : MonoBehaviour
    {
        [Header("Спавн камней")]
        [SerializeField] private GameObject[] m_rockPrefabs;
        [SerializeField] private Transform m_spawnPoint;
        [SerializeField] private RockSpawnSettings[] m_prefabSettings;

        [Header("Игрок")]
        [SerializeField] private Animator m_playerAnimator;
        [SerializeField] private Transform m_player;

        [Header("Удар по камню")]
        [SerializeField] private BoxCollider m_hoeHeadCollider;
        [SerializeField] private LayerMask m_rockLayer;
        [SerializeField] private float m_hitForce = 8f;
        [SerializeField] private float m_upwardForce = 1.5f;
        [SerializeField] private Button m_shootButton;

        [Header("Направление удара")]
        [Tooltip("Мировая ось, вдоль которой мотыга движется вперёд при ударе")]
        [SerializeField] private Vector3 m_swingDirection = Vector3.forward;
        [Tooltip("Минимальная скорость головы вперёд вдоль оси удара")]
        [SerializeField] private float m_minHitSpeed = 2f;
        [Tooltip("0..1, чем больше, тем меньше сглаживание скорости")]
        [SerializeField, Range(0f, 1f)] private float m_velocitySmoothing = 0.5f;

        [Header("Счётчик")]
        [SerializeField] private TMP_Text m_pointCount;
        private int m_point = 0;

        [Header("Конец игры")]
        [SerializeField] private string m_menuSceneName = "Menu";
        [SerializeField] private float m_gameOverDelay = 1.5f;

        [Header("Индикатор мяча")]
        [SerializeField] private Slider m_indicatorSlider;
        [SerializeField] private float m_indicatorTopY = 4.49f;
        [SerializeField] private float m_indicatorBottomY = 0.5f;
        [SerializeField] private BallIndicatorZone[] m_indicatorZones;

        private BallIndicator m_indicator;

        private const string BestScoreKey = "BestScore";
        private bool m_isGameOver;

        private readonly HashSet<int> m_hitRocks = new HashSet<int>();

        private IGameState m_currentState;
        private GolfShootAnimation m_golfShootAnimation;

        private Vector3 m_prevHeadPos;
        private Vector3 m_headVelocity;
        private bool m_hasPrevPos;
        

        private void Awake()
        {
            Time.timeScale = 1f;
            m_prefabSettings = new RockSpawnSettings[m_rockPrefabs.Length];

            for (int i = 0; i < m_rockPrefabs.Length; i++)
            {
                if (m_rockPrefabs[i] == null)
                {
                    continue;
                }

                m_prefabSettings[i] = m_rockPrefabs[i].GetComponent<RockSpawnSettings>();
            }

            if (m_pointCount == null)
            {
                Debug.LogError(
                    "GameManager: назначьте TMP_Text(Очки) в Inspector."
                );
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

            SetupShootButton();

            if (m_indicatorSlider != null)
            {
                m_indicator = new BallIndicator(
                    m_indicatorSlider, m_indicatorTopY, m_indicatorBottomY, m_indicatorZones);
            }

            ChangeState(new GamePlayState(this));
        }

        private void Update()
        {
            m_indicator?.Tick();

            if (m_golfShootAnimation == null)
            {
                return;
            }

        #if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            {
                OnShootPressed();
            }

            if (Input.GetMouseButtonUp(0))
            {
                OnShootReleased();
            }
        #endif
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }

        private void SetupShootButton()
        {
            if (m_shootButton == null)
            {
                return;
            }

            EventTrigger trigger = m_shootButton.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = m_shootButton.gameObject.AddComponent<EventTrigger>();
            }

            AddTrigger(trigger, EventTriggerType.PointerDown, OnShootPressed);
            AddTrigger(trigger, EventTriggerType.PointerUp, OnShootReleased);
        }

        private static void AddTrigger(EventTrigger trigger, EventTriggerType type, UnityEngine.Events.UnityAction action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(_ => action());
            trigger.triggers.Add(entry);
        }

        public void OnShootPressed()
        {
            if (m_isGameOver || m_golfShootAnimation == null)
            {
                return;
            }

            m_indicator?.Freeze();
            m_hitRocks.Clear();
            m_golfShootAnimation.Press();
        }

        public void OnShootReleased()
        {
            if (m_golfShootAnimation == null)
            {
                return;
            }

            m_golfShootAnimation.Release();
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
            if (m_isGameOver || m_hoeHeadCollider == null)
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

            foreach (Collider hit in hits)
            {
                RockSpawnSettings rock = hit.GetComponentInParent<RockSpawnSettings>();
                if (rock != null)
                {
                    rock.isTouch = true;
                }
            }

            if (m_golfShootAnimation == null ||
                !m_golfShootAnimation.IsPlaying)
            {
                return;
            }

            Vector3 swingDir = m_swingDirection.normalized;

            if (Vector3.Dot(m_headVelocity, swingDir) < m_minHitSpeed)
            {
                return;
            }

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

                RockSpawnSettings isTouchBall = rockBody.GetComponentInParent<RockSpawnSettings>();

                if (isTouchBall != null)
                {
                    isTouchBall.isTouch = true;
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

        private class GameOverState : IGameState
        {
            private readonly GameManager m_manager;

            public GameOverState(GameManager manager)
            {
                m_manager = manager;
            }

            public void Enter()
            {
                m_manager.m_isGameOver = true;
                m_manager.SaveScore();

                if (m_manager.m_shootButton != null)
                {
                    m_manager.m_shootButton.interactable = false;
                }

                Time.timeScale = 0f;
                m_manager.m_indicator?.Clear();
                m_manager.StartCoroutine(LoadMenu());
            }

            public void Exit() { }

            private IEnumerator LoadMenu()
            {
                yield return new WaitForSecondsRealtime(m_manager.m_gameOverDelay);

                Time.timeScale = 1f;
                SceneManager.LoadScene(m_manager.m_menuSceneName);
            }
        }

        private void OnEnable()
        {
            GameEvents.GameOver += HandleGameOver;
            GameEvents.PinataHit += AddPoint;
            GameEvents.RockSpawned += OnRockSpawned;
        }

        private void OnDisable()
        {
            GameEvents.GameOver -= HandleGameOver;
            GameEvents.PinataHit -= AddPoint;
            GameEvents.RockSpawned -= OnRockSpawned;
        }

        private void OnRockSpawned(Transform rock)
        {
            m_indicator?.Track(rock);
        }

        private void HandleGameOver()
        {
            if (m_isGameOver)
            {
                return;
            }

            ChangeState(new GameOverState(this));
        }

        private void AddPoint()
        {
            if (m_isGameOver)
            {
                return;
            }

            m_point++;
            
            if (m_pointCount != null)
            {
                m_pointCount.text = $"Счёт: {m_point}";
            }
            
            if (m_point % 10 == 0)
            {
                foreach (RockSpawnSettings settings in m_prefabSettings)
                {
                    if (settings != null && settings.minIntervalSeconds > 0.5)
                    {
                        settings.ChangeDifficulty();
                    }
                }
            }
        }

        private void SaveScore()
        {
            int best = PlayerPrefs.GetInt(BestScoreKey, 0);

            if (m_point > best)
            {
                PlayerPrefs.SetInt(BestScoreKey, m_point);
            }

            PlayerPrefs.SetInt("LastScore", m_point);
            PlayerPrefs.Save();
        }
    }
}