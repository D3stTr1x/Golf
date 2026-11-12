using System;
using UnityEngine;
using UnityEngine.UI;

namespace Golf.ScriptsGolf.UI
{
    [Serializable]
    public class BallIndicatorZone
    {
        public string name;
        [Range(0f, 1f)] public float from;
        [Range(0f, 1f)] public float to;
        public Color color = Color.white;
    }

    public class BallIndicator
    {
        private Slider m_slider;
        private float m_topY;
        private float m_bottomY;
        private bool m_isFrozen;
        private Transform m_target;
        private RectTransform m_marker;

        public BallIndicator(Slider slider, float topY, float bottomY, BallIndicatorZone[] zones)
        {
            m_slider = slider;
            m_topY = topY;
            m_bottomY = bottomY;

            m_slider.minValue = 0f;
            m_slider.maxValue = 1f;
            m_slider.wholeNumbers = false;
            m_slider.direction = Slider.Direction.BottomToTop;
            m_slider.interactable = false;
            m_slider.transition = Selectable.Transition.None;
            m_slider.value = 0f;

            BuildZones(zones != null && zones.Length > 0 ? zones : DefaultZones());

            SetValue(0f);
        }

        public void Track(Transform ball)
        {
            m_isFrozen = false;
            m_target = ball;
            SetValue(1f);
        }

        public void Clear()
        {
            m_target = null;
            SetValue(0f);
        }

        public void Tick()
        {
            if (m_isFrozen)
            {
                return;
            }

            if (m_target == null)
            {
                return;
            }

            float y = m_target.position.y;

            if (y <= m_bottomY)
            {
                Clear();
                return;
            }

            SetValue(Mathf.InverseLerp(m_bottomY, m_topY, y));
        }

        public void Freeze()
        {
            if (m_target == null)
            {
                return;
            }

            m_isFrozen = true;
        }

        private void SetValue(float value)
        {
            m_slider.value = value;

            if (m_marker != null)
            {
                m_marker.anchorMin = new Vector2(0f, value);
                m_marker.anchorMax = new Vector2(1f, value);
                m_marker.anchoredPosition = Vector2.zero;
            }
        }

        private void BuildZones(BallIndicatorZone[] zones)
        {
            GameObject root = new GameObject("Zones", typeof(RectTransform));
            RectTransform rootRect = (RectTransform)root.transform;
            rootRect.SetParent(m_slider.transform, false);
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMin = Vector2.zero;
            rootRect.offsetMax = Vector2.zero;
            rootRect.SetSiblingIndex(1);

            foreach (BallIndicatorZone zone in zones)
            {
                GameObject go = new GameObject(zone.name, typeof(RectTransform), typeof(Image));
                RectTransform rect = (RectTransform)go.transform;
                rect.SetParent(rootRect, false);
                rect.anchorMin = new Vector2(0f, Mathf.Min(zone.from, zone.to));
                rect.anchorMax = new Vector2(1f, Mathf.Max(zone.from, zone.to));
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;

                Image image = go.GetComponent<Image>();
                image.color = zone.color;
                image.raycastTarget = false;
            }

            GameObject markerGo = new GameObject("Marker", typeof(RectTransform), typeof(Image));
            m_marker = (RectTransform)markerGo.transform;
            m_marker.SetParent(rootRect, false);
            m_marker.anchorMin = new Vector2(0f, 1f);
            m_marker.anchorMax = new Vector2(1f, 1f);
            m_marker.sizeDelta = new Vector2(12f, 6f);
            m_marker.anchoredPosition = Vector2.zero;

            Image markerImage = markerGo.GetComponent<Image>();
            markerImage.color = Color.white;
            markerImage.raycastTarget = false;
        }

        private static BallIndicatorZone[] DefaultZones()
        {
            return new[]
            {
                new BallIndicatorZone { name = "Miss Top", from = 0.75f, to = 1.00f, color = Color.red },
                new BallIndicatorZone { name = "Ball Top", from = 0.67f, to = 0.75f, color = Color.blue },
                new BallIndicatorZone { name = "Pinata", from = 0.58f, to = 0.67f, color = Color.green },
                new BallIndicatorZone { name = "Ball Bottom", from = 0.50f, to = 0.58f, color = Color.blue },
                new BallIndicatorZone { name = "Miss Bottom", from = 0.00f, to = 0.50f, color = Color.red },
            };
        }
    }
}