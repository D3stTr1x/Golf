using UnityEngine;

namespace Golf.ScriptsGolf.SpawnRocks
{
    public class RockSpawnSettings : MonoBehaviour
    {
        [Min(0.3f)] public float minIntervalSeconds = 0.3f;
        [Min(0.4f)] public float maxIntervalSeconds = 5f;
        [Min(5f)] public float maxIntervalDecreaseMilliseconds = 100f;
        public bool isTouch = false;
        public bool isScored = false;

        private void OnCollisionEnter(Collision collision)
        {
            RockSpawnSettings other = collision.collider.GetComponentInParent<RockSpawnSettings>();

            if (other == null || other == this)
            {
                return;
            }

            if (!isTouch && !other.isTouch)
            {
                GameEvents.RaiseGameOver();
            }
        }

        public void ChangeDifficulty()
        {
            minIntervalSeconds -= 0.2f;
        }


        private void Start()
        {
            GameEvents.RaiseRockSpawned(transform);
        }
    }
}