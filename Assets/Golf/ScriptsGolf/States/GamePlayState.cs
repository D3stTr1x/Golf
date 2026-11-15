using Golf.ScriptsGolf.ShootScripts;
using Golf.ScriptsGolf.SpawnRocks;
using Golf.ScriptsGolf.States.Interfaces;
using Golf.ScriptsGolf.UI;
using UnityEngine;

namespace Golf.ScriptsGolf.States
{
    public class GamePlayState : IGameState, IActivate, IUpdatable, IFixedUpdatable, ILateUpdatable
    {
        private const int c_PointsPerDifficultyStep = 10;

        private readonly StateMachine m_stateMachine;
        private readonly RockSpawner m_spawner;
        private readonly Pinata m_pinata;
        private readonly ScoreService m_score;
        private readonly BallIndicator m_indicator;
        private readonly ShootInput m_input;
        private readonly GolfShootAnimation m_animation;
        private readonly HoeHitHandler m_hoe;

        public GamePlayState(
            StateMachine stateMachine,
            RockSpawner spawner,
            Pinata pinata,
            ScoreService score,
            BallIndicator indicator,
            ShootInput input,
            GolfShootAnimation animation,
            HoeHitHandler hoe)
        {
            m_stateMachine = stateMachine;
            m_spawner = spawner;
            m_pinata = pinata;
            m_score = score;
            m_indicator = indicator;
            m_input = input;
            m_animation = animation;
            m_hoe = hoe;
        }

        public void Enter()
        {
            m_score.Reset();
            m_spawner.StartSpawning();
        }

        public void Exit()
        {
            m_spawner.StopSpawning();
            m_indicator.Clear();
        }

        public void Activate()
        {
            m_pinata.Hit += OnPinataHit;
            m_spawner.RocksCollided += OnRocksCollided;
            m_spawner.RockSpawned += OnRockSpawned;
            m_input.Pressed += OnShootPressed;
            m_input.Released += OnShootReleased;

            m_input.Activate();
        }

        public void Deactivate()
        {
            m_input.Deactivate();

            m_pinata.Hit -= OnPinataHit;
            m_spawner.RocksCollided -= OnRocksCollided;
            m_spawner.RockSpawned -= OnRockSpawned;
            m_input.Pressed -= OnShootPressed;
            m_input.Released -= OnShootReleased;
        }

        public void Update()
        {
            m_input.Update();
            m_animation.Update();
            m_indicator.Tick();
        }

        public void FixedUpdate() => m_hoe.FixedUpdate();

        public void LateUpdate() => m_hoe.LateUpdate();

        private void OnShootPressed()
        {
            m_indicator.Freeze();
            m_hoe.BeginSwing(m_indicator.CurrentZone);
            m_animation.Press();
        }

        private void OnShootReleased() => m_animation.Release();

        private void OnPinataHit()
        {
            m_score.Add();

            if (m_score.Current % c_PointsPerDifficultyStep == 0)
            {
                m_spawner.IncreaseDifficulty();
            }
        }

        private void OnRockSpawned(Transform rock) => m_indicator.Track(rock);

        private void OnRocksCollided() => m_stateMachine.ChangeState<GameOverState>();
    }
}
