using System.Collections;
using Golf.ScriptsGolf.Effects;
using Golf.ScriptsGolf.States.Interfaces;
using Golf.ScriptsGolf.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Golf.ScriptsGolf.States
{
    public class GameWinState : IGameState
    {
        private readonly MonoBehaviour m_runner;
        private readonly ScoreService m_score;
        private readonly PinataEffects m_effects;
        private readonly WinMessage m_message;
        private readonly string m_menuSceneName;
        private readonly float m_menuDelay;

        public GameWinState(
            MonoBehaviour runner,
            ScoreService score,
            PinataEffects effects,
            WinMessage message,
            string menuSceneName,
            float menuDelay)
        {
            m_runner = runner;
            m_score = score;
            m_effects = effects;
            m_message = message;
            m_menuSceneName = menuSceneName;
            m_menuDelay = menuDelay;
        }

        public void Enter()
        {
            m_score.Save();
            m_effects.PlayBreak();
            m_message.Show();

            if (m_menuDelay > 0f)
            {
                m_runner.StartCoroutine(LoadMenu());
            }
        }

        public void Exit() { }

        private IEnumerator LoadMenu()
        {
            yield return new WaitForSecondsRealtime(m_menuDelay);

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            SceneManager.LoadScene(m_menuSceneName);
        }
    }
}
