using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TestDebugLog : MonoBehaviour
{
    [SerializeField] TMP_Text text;
    [SerializeField] GameObject debugConsoleObject;

    void Start()
    {
        Application.logMessageReceived += application_logMessageReceived;
    }

    void application_logMessageReceived(string condition, string stackTrace, LogType type)
    {
        string msg;
        string crlf = System.Environment.NewLine;
        if (type == LogType.Error)
        {
            msg = $"<color=#ee4444>{condition}{crlf}{stackTrace}</color>{crlf}";
        }
        else
        if (type == LogType.Warning)
        {
            msg = $"<color=orange>{condition}</color>{crlf}{crlf}";
        }
        else
        {
            msg = $"{condition}{crlf}{crlf}";
        }
        msg = text.text.Insert(0, msg);
        if (msg.Length >= 1000)
        {
            msg = msg.Substring(0, 1000);
        }
        text.SetText(msg);
    }

    public void OnChanged(Toggle toggle)
    {
        debugConsoleObject.SetActive(toggle.isOn);
    }
}

