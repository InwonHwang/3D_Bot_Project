using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

namespace Robot.Singleton
{
    public class EventTriggerAgent : Singleton<EventTriggerAgent>
    {
        #region public method

        public void addEvent(string parnet, string path, EventTriggerType type, UnityAction action)
        {
            var GUIObject = GameObjectAgent.Instance.findChild(parnet, path);

            internalAddEvent(GUIObject, type, action);
        }

        public void addEvent<T>(string parnet, string path, EventTriggerType type, UnityAction<T> action, T data)
        {
            var GUIObject = GameObjectAgent.Instance.findChild(parnet, path);

            internalAddEvent<T>(GUIObject, type, action, data);
        }

        public void addEvent<T0, T1>(string parnet, string path, EventTriggerType type, UnityAction<T0, T1> action, T0 data0, T1 data1)
        {
            var GUIObject = GameObjectAgent.Instance.findChild(parnet, path);

            internalAddEvent<T0, T1>(GUIObject, type, action, data0, data1);
        }

        public void addEvent(GameObject gameobject, EventTriggerType type, UnityAction action)
        {
            internalAddEvent(gameobject, type, action);
        }        

        public void addEvent<T>(GameObject gameobject, EventTriggerType type, UnityAction<T> action, T data)
        {
            internalAddEvent<T>(gameobject, type, action, data);
        }

        public void addEvent<T0, T1>(GameObject gameobject, EventTriggerType type, UnityAction<T0, T1> action, T0 data0, T1 data1)
        {
            internalAddEvent<T0, T1>(gameobject, type, action, data0, data1);
        }

        #endregion public method

        #region private method

        void internalAddEvent(GameObject gameobject, EventTriggerType type, UnityAction action)
        {
            if (!gameobject) return;

            EventTrigger trigger = gameobject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = gameobject.gameObject.AddComponent<EventTrigger>();

            if (action == null) return;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(delegate { action(); });
            trigger.triggers.Add(entry);
        }

        void internalAddEvent<T>(GameObject gameobject, EventTriggerType type, UnityAction<T> action, T data)
        {
            if (!gameobject) return;

            EventTrigger trigger = gameobject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = gameobject.gameObject.AddComponent<EventTrigger>();

            if (action == null) return;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(delegate { action(data); });
            trigger.triggers.Add(entry);
        }

        void internalAddEvent<T0, T1>(GameObject gameobject, EventTriggerType type, UnityAction<T0, T1> action, T0 data0, T1 data1)
        {
            if (!gameobject) return;

            EventTrigger trigger = gameobject.GetComponent<EventTrigger>();
            if (trigger == null)
                trigger = gameobject.gameObject.AddComponent<EventTrigger>();

            if (action == null) return;

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(delegate { action(data0, data1); });
            trigger.triggers.Add(entry);
        }

        #endregion private method

    }   // class EventTriggetAgner

} // namespace Robot.Singleton
