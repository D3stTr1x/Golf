using System.Collections;
using UnityEngine;

namespace Golf.ScriptsGolf.SpawnRocks
{
    public class RockSpawner : MonoBehaviour
    {
        private const float c_MinIntervalFloor = 0.7f;
        private const float c_DifficultyStep = 0.05f;

        public event System.Action<Transform> RockSpawned;
        public event System.Action RocksCollided;

        [SerializeField] private GameObject[] m_rockPrefabs;
        [SerializeField] private Transform m_spawnPoint;

        private RockSpawnSettings[] m_settings;
        private float[] m_min;
        private float[] m_max;
        private Coroutine m_routine;

        private void Awake()
        {
            int count = m_rockPrefabs.Length;
            m_settings = new RockSpawnSettings[count];
            m_min = new float[count];
            m_max = new float[count];

            for (int i = 0; i < count; i++)
            {
                if (m_rockPrefabs[i] != null && m_rockPrefabs[i].TryGetComponent(out RockSpawnSettings s))
                {
                    m_settings[i] = s;
                    m_min[i] = Mathf.Max(0f, s.minIntervalSeconds);
                    m_max[i] = Mathf.Max(m_min[i], s.maxIntervalSeconds);
                }
            }
        }

        public void StartSpawning()
        {
            if (m_routine == null)
            {
                m_routine = StartCoroutine(SpawnLoop());
            }
        }

        public void StopSpawning()
        {
            if (m_routine != null)
            {
                StopCoroutine(m_routine);
                m_routine = null;
            }
        }

        public void IncreaseDifficulty()
        {
            for (int i = 0; i < m_settings.Length; i++)
            {
                if (m_settings[i] != null && m_min[i] > c_MinIntervalFloor)
                {
                    m_min[i] -= c_DifficultyStep;
                }
            }
        }

        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                for (int i = 0; i < m_rockPrefabs.Length; i++)
                {
                    if (m_settings[i] == null)
                    {
                        continue;
                    }

                    yield return new WaitForSeconds(Random.Range(m_min[i], Mathf.Max(m_min[i], m_max[i])));

                    GameObject rock = Instantiate(m_rockPrefabs[i], m_spawnPoint.position, m_spawnPoint.rotation);

                    if (rock.TryGetComponent(out RockSpawnSettings spawned))
                    {
                        spawned.Collided += OnRockCollided;
                    }

                    RockSpawned?.Invoke(rock.transform);

                    float decrease = Mathf.Max(0f, m_settings[i].maxIntervalDecreaseMilliseconds) / 1000f;
                    m_max[i] = Mathf.Max(m_min[i], m_max[i] - decrease);
                }
            }
        }

        private void OnRockCollided() => RocksCollided?.Invoke();
    }
}
