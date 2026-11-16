using System.Collections.Generic;
using Golf.ScriptsGolf.SpawnRocks;
using Golf.ScriptsGolf.States.Interfaces;
using Golf.ScriptsGolf.UI;
using UnityEngine;

namespace Golf.ScriptsGolf.ShootScripts
{
    public class HoeHitHandler : IFixedUpdatable, ILateUpdatable
    {
        private const float MaxSweepDistance = 1.5f;

        public event System.Action StoneHit;

        private readonly HoeHitSettings m_settings;
        private readonly GolfShootAnimation m_animation;
        private readonly Transform m_target;
        private readonly HashSet<int> m_hitRocks = new HashSet<int>();
        private readonly List<Collider> m_ignored = new List<Collider>();
        private readonly HashSet<Collider> m_candidates = new HashSet<Collider>();

        private Vector3 m_prevHeadPos;
        private Vector3 m_headVelocity;
        private bool m_hasPrevPos;

        private Vector3 m_prevCenter;
        private bool m_hasPrevCenter;

        private ZoneType m_zone = ZoneType.Miss;

        public HoeHitHandler(HoeHitSettings settings, GolfShootAnimation animation, Transform target)
        {
            m_settings = settings;
            m_animation = animation;
            m_target = target;
        }

        public void BeginSwing(ZoneType zone)
        {
            RestoreCollisions();
            m_hitRocks.Clear();
            m_zone = zone;
        }

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
            FindCandidates();

            foreach (Collider hit in m_candidates)
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

            foreach (Collider hit in m_candidates)
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

                Hit(body, swingDir);
                StoneHit?.Invoke();
            }
        }

        private void FindCandidates()
        {
            m_candidates.Clear();

            BoxCollider head = m_settings.headCollider;
            Transform headTransform = head.transform;
            Vector3 scale = headTransform.lossyScale;
            Vector3 halfExtents = Vector3.Scale(
                head.size,
                new Vector3(Mathf.Abs(scale.x), Mathf.Abs(scale.y), Mathf.Abs(scale.z))) * 0.5f;
            halfExtents += Vector3.one * m_settings.hitBoxPadding;

            Vector3 center = headTransform.TransformPoint(head.center);
            Quaternion rotation = headTransform.rotation;

            foreach (Collider c in Physics.OverlapBox(
                center, halfExtents, rotation, m_settings.rockLayer, QueryTriggerInteraction.Ignore))
            {
                m_candidates.Add(c);
            }

            if (m_animation.IsPlaying && m_hasPrevCenter)
            {
                Vector3 delta = center - m_prevCenter;
                float distance = delta.magnitude;

                if (distance > 0.001f && distance < MaxSweepDistance)
                {
                    RaycastHit[] swept = Physics.BoxCastAll(
                        m_prevCenter,
                        halfExtents,
                        delta / distance,
                        rotation,
                        distance,
                        m_settings.rockLayer,
                        QueryTriggerInteraction.Ignore);

                    foreach (RaycastHit h in swept)
                    {
                        m_candidates.Add(h.collider);
                    }
                }
            }

            m_prevCenter = center;
            m_hasPrevCenter = m_animation.IsPlaying;
        }

        private void Hit(Rigidbody body, Vector3 swingDir)
        {
            switch (m_zone)
            {
                case ZoneType.Pinata:
                    LaunchToTarget(body);
                    break;
                case ZoneType.BallHigh:
                    Impulse(body, swingDir, m_settings.upwardForce * m_settings.highZoneUpwardMultiplier);
                    break;
                case ZoneType.BallLow:
                    Impulse(body, swingDir, m_settings.upwardForce * m_settings.lowZoneUpwardMultiplier);
                    break;
                default:
                    Impulse(body, swingDir, m_settings.upwardForce);
                    break;
            }
        }

        private void Impulse(Rigidbody body, Vector3 swingDir, float upward)
        {
            Vector3 dir = (swingDir + Vector3.up * upward).normalized;
            body.AddForce(dir * m_settings.hitForce, ForceMode.Impulse);
        }

        private void LaunchToTarget(Rigidbody body)
        {
            Vector3 aim = m_target.position + m_settings.pinataAimOffset;
            float time = Mathf.Max(0.05f, m_settings.pinataFlightTime);

            Vector3 velocity = (aim - body.position) / time - 0.5f * Physics.gravity * time;

            body.linearVelocity = velocity;
            body.angularVelocity = Vector3.zero;

            foreach (Collider c in body.GetComponentsInChildren<Collider>())
            {
                Physics.IgnoreCollision(m_settings.headCollider, c, true);
                m_ignored.Add(c);
            }
        }

        private void RestoreCollisions()
        {
            foreach (Collider c in m_ignored)
            {
                if (c != null)
                {
                    Physics.IgnoreCollision(m_settings.headCollider, c, false);
                }
            }

            m_ignored.Clear();
        }
    }
}
