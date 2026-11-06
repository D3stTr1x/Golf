using System;
using System.Collections.Generic;
using UnityEngine;

public class Test
{
    public interface IGameState
    {
        void Enter();
        void Exit();
    }

    public interface IUpdatable
    {
        void Update();
    }

    public class GameManager : MonoBehaviour
    {
        private event Action<IGameState> m_onLose;
        private List<IUpdatable> upatables;
        private IGameState m_currentState;

        private void Awake()
        {
            GameOverState gameOverState = new GameOverState();
            //gameOverState.Initialize(new StoneSpawner());

            m_onLose += ChangeState;
        }
        private void Update()
        {
            /*foreach (var variable in m_updatebles)
            {
                variable.Update();
            }
            */
        }
        private void ChangeState(IGameState newState)
        {
            m_currentState?.Exit();
            m_currentState = newState;
            m_currentState.Enter();
        }
    }

    public class GamePlayState : IGameState
    {
        //private StoneSpawner m_stoneSpawner;

        private void Initialize()
        {
            //m_stoneSpawner = m_stoneSpawner;
        }
        public void Enter()
        {
            
        }

        public void Exit()
        {
            
        }
    }

    public class GameOverState : IGameState
    {
        private void Initialize()
        {
        }
        public void Enter()
        {

        }

        public void Exit()
        {

        }
    }
}

