using UnityEngine;

namespace Golf.ScriptsGolf.SpawnRocks
{
    public class RockSpawnSettings : MonoBehaviour
    {
        [Min(0.3f)] public float minIntervalSeconds = 0.3f;
        [Min(0.4f)] public float maxIntervalSeconds = 5f;
        [Min(5f)] public float maxIntervalDecreaseMilliseconds = 100f;
    }
}