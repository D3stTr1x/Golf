using UnityEngine;

namespace Golf.ScriptsGolf
{
    public class ScoreService
    {
        public const string BestScoreKey = "BestScore";
        public const string LastScoreKey = "LastScore";

        public event System.Action<int> Changed;

        public int Current { get; private set; }

        public void Reset()
        {
            Current = 0;
            Changed?.Invoke(Current);
        }

        public void Add(int value = 1)
        {
            Current += value;
            Changed?.Invoke(Current);
        }

        public void Save()
        {
            int best = PlayerPrefs.GetInt(BestScoreKey, 0);

            if (Current > best)
            {
                PlayerPrefs.SetInt(BestScoreKey, Current);
            }

            PlayerPrefs.SetInt(LastScoreKey, Current);
            PlayerPrefs.Save();
        }
    }
}
