using UnityEngine;

namespace Golf.ScriptsGolf
{
    public class GameSettingsStorage
    {
        private const string MusicKey = "MusicOn";
        private const string LightKey = "LightOn";

        public bool MusicOn
        {
            get => PlayerPrefs.GetInt(MusicKey, 1) == 1;
            set => Write(MusicKey, value);
        }

        public bool LightOn
        {
            get => PlayerPrefs.GetInt(LightKey, 1) == 1;
            set => Write(LightKey, value);
        }

        private static void Write(string key, bool value)
        {
            PlayerPrefs.SetInt(key, value ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
