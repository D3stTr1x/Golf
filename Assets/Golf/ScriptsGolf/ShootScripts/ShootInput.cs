using System.Collections.Generic;
using Golf.ScriptsGolf.States.Interfaces;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Golf.ScriptsGolf.ShootScripts
{
    public class ShootInput : IActivate, IUpdatable
    {
        public event System.Action Pressed;
        public event System.Action Released;

        private readonly Button m_button;
        private readonly List<EventTrigger.Entry> m_entries = new List<EventTrigger.Entry>();
        private EventTrigger m_trigger;

        public ShootInput(Button button)
        {
            m_button = button;
        }

        public void Activate()
        {
            if (m_button == null)
            {
                return;
            }

            m_trigger = m_button.GetComponent<EventTrigger>();
            if (m_trigger == null)
            {
                m_trigger = m_button.gameObject.AddComponent<EventTrigger>();
            }

            AddEntry(EventTriggerType.PointerDown, () => Pressed?.Invoke());
            AddEntry(EventTriggerType.PointerUp, () => Released?.Invoke());

            m_button.interactable = true;
        }

        public void Deactivate()
        {
            if (m_button == null)
            {
                return;
            }

            foreach (EventTrigger.Entry entry in m_entries)
            {
                m_trigger.triggers.Remove(entry);
            }
            m_entries.Clear();

            m_button.interactable = false;
        }

        public void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUI())
            {
                Pressed?.Invoke();
            }

            if (Input.GetMouseButtonUp(0))
            {
                Released?.Invoke();
            }
#endif
        }

        private void AddEntry(EventTriggerType type, UnityEngine.Events.UnityAction action)
        {
            EventTrigger.Entry entry = new EventTrigger.Entry { eventID = type };
            entry.callback.AddListener(_ => action());
            m_trigger.triggers.Add(entry);
            m_entries.Add(entry);
        }

        private static bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}
