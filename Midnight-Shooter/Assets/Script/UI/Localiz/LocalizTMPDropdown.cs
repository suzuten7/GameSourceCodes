using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizTMPDropdown : MonoBehaviour
{
    [SerializeField] string tableStr = "UI";
    [SerializeField] string keyStr;
    string[] defStr;
    TMP_Dropdown dr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dr = GetComponent<TMP_Dropdown>();
        if (dr == null) enabled = false;
        defStr = new string[dr.options.Count];
        for(int i = 0; i < dr.options.Count; i++)
        {
            defStr[i] = dr.options[i].text;
        }
        Refresh();
    }
    private void OnEnable()
    {
        LocalizationSettings.SelectedLocaleChanged += OnLocaleChanged;
        Refresh();
    }

    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= OnLocaleChanged;
    }
    private void OnLocaleChanged(UnityEngine.Localization.Locale locale)
    {
        Refresh();
    }
    void Refresh()
    {
        if (dr == null) return;
        if (LocalizSystem.LocailzCheck(tableStr, keyStr))
        {
            var lstr = LocalizSystem.LocailzString(tableStr, keyStr);
            var lstrs = lstr.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
            var cstr = "";
            for (int i = 0; i < dr.options.Count; i++)
            {
                var op = dr.options[i];
                op.text = i < lstrs.Length ? lstrs[i] : $"[{i}]";
                if (dr.value == i) cstr = op.text;
            }
            dr.captionText.text = cstr;
        }
        else
        {
            var cstr = "";
            for (int i = 0; i < dr.options.Count; i++)
            {
                var op = dr.options[i];
                op.text = i < defStr.Length ? LocalizSystem.StrEx(defStr[i]) : $"[{i}]";
                if (dr.value == i) cstr = op.text;
            }
            dr.captionText.text = cstr;
        }
    }
}
