using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.Localization.Settings;
using System.Collections.Generic;

public class LocalizLangChange : MonoBehaviour
{
    [SerializeField] List<string> langStrs;
    [SerializeField] TMP_Dropdown langDr;
    void Update()
    {
        var clang = LocalizationSettings.SelectedLocale.Identifier.Code;
        var langID = langStrs.FindIndex(x => x == clang);
        if(langID >= 0)langDr.value = langID;
    }
    public void langChangeDr()
    {
        var langStr = langStrs[langDr.value];
        var locale = LocalizationSettings.AvailableLocales.GetLocale(langStr);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
        else
        {
            Debug.LogWarning($"Locale '{langStr}' が見つかりません。");
        }
        LocalizSystem.LocailzLangChange();
    }
}
