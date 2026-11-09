using System;

namespace Golf.ScriptsGolf
{
    public static class GameEvents
    {
        public static event Action GameOver;
        public static event Action PinataHit;

        public static void RaiseGameOver() => GameOver?.Invoke();
        public static void RaisePinataHit() => PinataHit?.Invoke();
    }
}