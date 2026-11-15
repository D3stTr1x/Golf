using TMPro;

namespace Golf.ScriptsGolf.UI
{
    public class ScoreView
    {
        private readonly TMP_Text m_text;

        public ScoreView(TMP_Text text, ScoreService score)
        {
            m_text = text;
            score.Changed += Show;
            Show(score.Current);
        }

        private void Show(int value)
        {
            if (m_text != null)
            {
                m_text.text = $"Счёт: {value}";
            }
        }
    }
}
