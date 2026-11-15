using Golf.ScriptsGolf.ShootScripts;
using Golf.ScriptsGolf.SpawnRocks;
using Golf.ScriptsGolf.States;
using Golf.ScriptsGolf.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Golf.ScriptsGolf.Source
{
    public class Bootstrap : MonoBehaviour
    {
        [Header("Спавн камней")]
        [SerializeField] private RockSpawner m_rockSpawner;
        [SerializeField] private Pinata m_pinata;

        [Header("Игрок")]
        [SerializeField] private Animator m_playerAnimator;
        [SerializeField] private Transform m_player;

        [Header("Удар по камню")]
        [SerializeField] private HoeHitSettings m_hoe;
        [SerializeField] private Button m_shootButton;

        [Header("Счётчик")]
        [SerializeField] private TMP_Text m_pointCount;

        [Header("Индикатор мяча")]
        [SerializeField] private Slider m_indicatorSlider;
        [SerializeField] private float m_indicatorTopY = 4.49f;
        [SerializeField] private float m_indicatorBottomY = 0.5f;
        [SerializeField] private BallIndicatorZone[] m_indicatorZones;

        [Header("Конец игры")]
        [SerializeField] private string m_menuSceneName = "Menu";
        [SerializeField] private float m_gameOverDelay = 1.5f;

        private StateMachine m_stateMachine;

        private void Awake()
        {
            if (m_playerAnimator == null || m_player == null ||
                m_hoe.headCollider == null || m_indicatorSlider == null ||
                m_rockSpawner == null || m_pinata == null)
            {
                Debug.LogError("Bootstrap: назначьте все обязательные поля в Inspector.");
                enabled = false;
                return;
            }

            Time.timeScale = 1f;

            m_stateMachine = new StateMachine();

            var score = new ScoreService();
            var scoreView = new ScoreView(m_pointCount, score);
            var indicator = new BallIndicator(m_indicatorSlider, m_indicatorTopY, m_indicatorBottomY, m_indicatorZones);
            var animation = new GolfShootAnimation(m_playerAnimator, m_player, "Shoot", "Idle");
            var hoe = new HoeHitHandler(m_hoe, animation);
            var input = new ShootInput(m_shootButton);

            m_stateMachine.Register(new GamePlayState(
                m_stateMachine, m_rockSpawner, m_pinata, score, indicator, input, animation, hoe));
            m_stateMachine.Register(new GameOverState(
                this, score, m_menuSceneName, m_gameOverDelay));
        }

        private void Start() => m_stateMachine.ChangeState<GamePlayState>();

        private void Update() => m_stateMachine.Update();

        private void FixedUpdate() => m_stateMachine.FixedUpdate();

        private void LateUpdate() => m_stateMachine.LateUpdate();
    }
}
