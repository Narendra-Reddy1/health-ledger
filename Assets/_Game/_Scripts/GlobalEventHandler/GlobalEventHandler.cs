using System.Collections.Generic;


namespace BenStudios.EventSystem
{
    public static class GlobalEventHandler
    {

        private static Dictionary<EventID, System.Action<object>> _eventsDict = new Dictionary<EventID, System.Action<object>>();

        public static void AddListener(EventID eventID, System.Action<object> callback)
        {
            if (!_eventsDict.ContainsKey(eventID))
                _eventsDict.Add(eventID, null);
            _eventsDict[eventID] = _eventsDict[eventID] + callback;
        }
        public static void RemoveListener(EventID eventID, System.Action<object> callback)
        {
            if (_eventsDict.ContainsKey(eventID))
            {
                _eventsDict[eventID] = _eventsDict[eventID] - callback;

                if (_eventsDict[eventID] == null)
                    _eventsDict.Remove(eventID);
            }
        }
        public static void TriggerEvent(EventID eventID, object args = null)
        {
            System.Action<object> callback;
            _eventsDict.TryGetValue(eventID, out callback);
            callback?.DynamicInvoke(args);
        }
        public static void CleanAllCallbacks(EventID eventID)
        {
            if (_eventsDict.ContainsKey(eventID))
                _eventsDict[eventID] = null;
        }
        public static void CleanTable()
        {
            _eventsDict.Clear();
        }

    }
}