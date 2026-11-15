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
        public Vector3 swingDirection = Vector3.forward;
        public float minHitSpeed = 2f;
        [Range(0f, 1f)] public float velocitySmoothing = 0.5f;

        public float hitBoxPadding = 0f;

        [Header("Зелёная зона: мяч летит точно в пиньяту")]
        public float pinataFlightTime = 0.8f;
        public Vector3 pinataAimOffset = Vector3.zero;

        [Header("Синие зоны: обычный физический удар")]
        public float highZoneUpwardMultiplier = 20f;
        public float lowZoneUpwardMultiplier = 0.3f;
    }
}
