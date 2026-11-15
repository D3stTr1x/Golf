using System.Collections;
using Golf.ScriptsGolf.States.Interfaces;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Golf.ScriptsGolf.States
{
    public class GameOverState : IGameState
    {
        private readonly MonoBehaviour m_runner;
        private readonly ScoreService m_score;
        private readonly string m_menuSceneName;
        private readonly float m_delay;

        public GameOverState(MonoBehaviour runner, ScoreService score, string menuSceneName, float delay)
        {
            m_runner = runner;
            m_score = score;
            m_menuSceneName = menuSceneName;
            m_delay = delay;
        }

        public void Enter()
        {
            m_score.Save();
            Time.timeScale = 0f;
            m_runner.StartCoroutine(LoadMenu());
        }

        public void Exit()
        {
            Time.timeScale = 1f;
        }

        private IEnumerator LoadMenu()
        {
            yield return new WaitForSecondsRealtime(m_delay);

            Time.timeScale = 1f;
            SceneManager.LoadScene(m_menuSceneName);
        }
    }
}
