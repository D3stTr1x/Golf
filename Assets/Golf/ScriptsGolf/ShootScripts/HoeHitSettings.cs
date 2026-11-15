using System;
using UnityEngine;

namespace Golf.ScriptsGolf.ShootScripts
{
    [Serializable]
    public class HoeHitSettings
    {
        public BoxCollider headCollider;
        public LayerMask rockLayer;
        public float hitForce = 8f;
        public float upwardForce = 1.5f;
        [Tooltip("Мировая ось, вдоль которой мотыга движется вперёд при ударе")]
        public Vector3 swingDirection = Vector3.forward;
        [Tooltip("Минимальная скорость головы вперёд вдоль оси удара")]
        public float minHitSpeed = 2f;
        [Tooltip("0..1, чем больше, тем меньше сглаживание скорости")]
        [Range(0f, 1f)] public float velocitySmoothing = 0.5f;
    }
}
