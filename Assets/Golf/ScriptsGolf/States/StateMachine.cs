using System;
using System.Collections.Generic;
using Golf.ScriptsGolf.States.Interfaces;

namespace Golf.ScriptsGolf.States
{
    public class StateMachine
    {
        private readonly Dictionary<Type, IGameState> m_states = new Dictionary<Type, IGameState>();
        private IGameState m_current;

        public void Register(IGameState state)
        {
            m_states.Add(state.GetType(), state);
        }

        public void ChangeState<TState>() where TState : IGameState
        {
            if (!m_states.TryGetValue(typeof(TState), out IGameState next))
            {
                throw new InvalidOperationException($"State {typeof(TState).Name} is not registered");
            }

            if (m_current == next)
            {
                return;
            }

            if (m_current is IActivate oldActivate)
            {
                oldActivate.Deactivate();
            }

            m_current?.Exit();

            m_current = next;

            m_current.Enter();
            if (m_current is IActivate newActivate)
            {
                newActivate.Activate();
            }
        }

        public void Update() => (m_current as IUpdatable)?.Update();

        public void FixedUpdate() => (m_current as IFixedUpdatable)?.FixedUpdate();

        public void LateUpdate() => (m_current as ILateUpdatable)?.LateUpdate();
    }
}
