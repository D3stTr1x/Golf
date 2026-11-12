using System;
using UnityEngine;

namespace Golf.ScriptsGolf
{
    public static class GameEvents
    {
        public static event Action GameOver;
        public static event Action PinataHit;
        public static event Action<Transform> RockSpawned;
        
        public static void RaiseRockSpawned(Transform rock) => RockSpawned?.Invoke(rock);
        public static void RaiseGameOver() => GameOver?.Invoke();
        public static void RaisePinataHit() => PinataHit?.Invoke();
    }
}