using TMPro;
using UnityEngine;

namespace Golf.ScriptsGolf.Buttons
{
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField] private TMP_Text m_scoreText;

        private void Awake()
        {
            if (m_scoreText == null)
            {
                return;
            }

            int best = PlayerPrefs.GetInt(ScoreService.BestScoreKey, 0);
            int last = PlayerPrefs.GetInt(ScoreService.LastScoreKey, 0);
            m_scoreText.text = $"Рекорд: {best}\nПоследний: {last}";
        }
    }
}
