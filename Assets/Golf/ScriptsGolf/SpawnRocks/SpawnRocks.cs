using System.Collections;
using UnityEngine;

namespace Golf.ScriptsGolf.SpawnRocks
{
    public static class RockSpawner
    {
        public static IEnumerator SpawnRocks(
            GameObject[] rockPrefabs,
            Transform spawnPoint)
        {
            if (rockPrefabs == null || spawnPoint == null)
            {
                yield break;
            }

            float[] currentMax = new float[rockPrefabs.Length];

            for (int i = 0; i < rockPrefabs.Length; i++)
            {
                if (rockPrefabs[i] == null)
                {
                    continue;
                }

                RockSpawnSettings settings = rockPrefabs[i].GetComponent<RockSpawnSettings>();

                if (settings != null)
                {    
                    currentMax[i] = Mathf.Max(settings.minIntervalSeconds, settings.maxIntervalSeconds);
                }
            }

            while (true)
            {
                for (int i = 0; i < rockPrefabs.Length; i++)
                {
                    GameObject prefab = rockPrefabs[i];
                    if (prefab == null)
                    {
                        continue;
                    }

                    RockSpawnSettings settings = prefab.GetComponent<RockSpawnSettings>();
                    if (settings == null)
                    {
                        continue;
                    }

                    float min = Mathf.Max(0f, settings.minIntervalSeconds);
                    float interval = Random.Range(min, Mathf.Max(min, currentMax[i]));

                    yield return new WaitForSeconds(interval);
                    Object.Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);

                    float decrease =
                        Mathf.Max(0f, settings.maxIntervalDecreaseMilliseconds) / 1000f;

                    currentMax[i] = Mathf.Max(min, currentMax[i] - decrease);
                }
            }
        }
    }
}