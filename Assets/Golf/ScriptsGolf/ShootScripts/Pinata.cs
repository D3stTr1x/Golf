using UnityEngine;
using Golf.ScriptsGolf.SpawnRocks;

namespace Golf.ScriptsGolf.ShootScripts
{
    public class Pinata : MonoBehaviour
    {
        public event System.Action Hit;

        private void OnCollisionEnter(Collision collision)
        {
            RockSpawnSettings ball = collision.collider.GetComponentInParent<RockSpawnSettings>();

            if (ball == null || ball.isScored)
            {
                return;
            }

            ball.isScored = true;
            Hit?.Invoke();
        }
    }
}
