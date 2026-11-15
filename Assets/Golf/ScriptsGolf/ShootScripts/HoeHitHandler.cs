using System.Collections.Generic;
using Golf.ScriptsGolf.SpawnRocks;
using Golf.ScriptsGolf.States.Interfaces;
using UnityEngine;

namespace Golf.ScriptsGolf.ShootScripts
{
    public class HoeHitHandler : IFixedUpdatable, ILateUpdatable
    {
        public event System.Action StoneHit;

        private readonly HoeHitSettings m_settings;
        private readonly GolfShootAnimation m_animation;
        private readonly HashSet<int> m_hitRocks = new HashSet<int>();

        private Vector3 m_prevHeadPos;
        private Vector3 m_headVelocity;
        private bool m_hasPrevPos;



        private Rigidbody m_lastHit;

        public HoeHitHandler(HoeHitSettings settings, GolfShootAnimation animation)
        {
            m_settings = settings;
            m_animation = animation;
        }

        public void ResetHits() => m_hitRocks.Clear();

        public void LateUpdate()
        {
            if (!m_animation.IsPlaying)
            {
                m_hasPrevPos = false;
                m_headVelocity = Vector3.zero;
                return;
            }

            Vector3 headPos = m_settings.headCollider.transform.position;

            Vector3 rawVelocity = (m_hasPrevPos && Time.deltaTime > 0f)
                ? (headPos - m_prevHeadPos) / Time.deltaTime
                : Vector3.zero;

            m_headVelocity = Vector3.Lerp(m_headVelocity, rawVelocity, m_settings.velocitySmoothing);

            m_prevHeadPos = headPos;
            m_hasPrevPos = true;
        }

        public void FixedUpdate()
        {


            if (m_lastHit != null)
            {
                Debug.Log($"after: vel={m_lastHit.linearVelocity}");
                m_lastHit = null;
            }



            Bounds bounds = m_settings.headCollider.bounds;
            Collider[] hits = Physics.OverlapBox(
                bounds.center,
                bounds.extents,
                Quaternion.identity,
                m_settings.rockLayer,
                QueryTriggerInteraction.Ignore);

            foreach (Collider hit in hits)
            {
                RockSpawnSettings rock = hit.GetComponentInParent<RockSpawnSettings>();
                if (rock != null)
                {
                    rock.isTouch = true;
                }
            }

            if (!m_animation.IsPlaying)
            {
                return;
            }

            Vector3 swingDir = m_settings.swingDirection.normalized;

            if (Vector3.Dot(m_headVelocity, swingDir) < m_settings.minHitSpeed)
            {
                return;
            }

            Vector3 hitDir = (swingDir + Vector3.up * m_settings.upwardForce).normalized;

            foreach (Collider hit in hits)
            {
                Rigidbody body = hit.attachedRigidbody;

                if (body == null || body.isKinematic)
                {
                    continue;
                }

                if (!m_hitRocks.Add(body.GetInstanceID()))
                {
                    continue;
                }

                Debug.Log($"hit: ballPos={body.position} headCenter={bounds.center} ballVel={body.linearVelocity}");
                body.AddForce(hitDir * m_settings.hitForce, ForceMode.Impulse);
                m_lastHit = body;
                StoneHit?.Invoke();
            }
        }
    }
}
