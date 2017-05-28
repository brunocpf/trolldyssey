/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{
    public int this[StatType s]
    {
        get { return _data[(int)s]; }
        set { SetValue(s, value, true); }
    }
    int[] _data = new int[(int)StatType.Count];

    public static string WillChangeNotification(StatType type)
    {
        if (!_willChangeNotifications.ContainsKey(type))
            _willChangeNotifications.Add(type, string.Format("Stats.{0}WillChange", type.ToString()));
        return _willChangeNotifications[type];
    }

    public static string DidChangeNotification(StatType type)
    {
        if (!_didChangeNotifications.ContainsKey(type))
            _didChangeNotifications.Add(type, string.Format("Stats.{0}DidChange", type.ToString()));
        return _didChangeNotifications[type];
    }

    static Dictionary<StatType, string> _willChangeNotifications = new Dictionary<StatType, string>();
    static Dictionary<StatType, string> _didChangeNotifications = new Dictionary<StatType, string>();

    public void SetValue(StatType type, int value, bool allowExceptions)
    {
        int oldValue = this[type];
        if (oldValue == value)
            return;

        if (allowExceptions)
        {
            // Allow exceptions to the rule here
            ValueChangeException exc = new ValueChangeException(oldValue, value);

            // The notification is unique per stat type
            this.PostNotification(WillChangeNotification(type), exc);

            // Did anything modify the value?
            value = Mathf.FloorToInt(exc.GetModifiedValue());

            // Did something nullify the change?
            if (exc.toggle == false || value == oldValue)
                return;
        }

        _data[(int)type] = value;
        this.PostNotification(DidChangeNotification(type), oldValue);
    }
}
*/