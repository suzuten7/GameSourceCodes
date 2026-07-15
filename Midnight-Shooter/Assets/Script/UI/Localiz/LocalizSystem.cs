using System;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
public class LocalizSystem : MonoBehaviour
{
    static bool locLoad = false;
    static public void LocailzLangLoad()
    {
        if (locLoad) return;
        locLoad = true;
        var dflangStr = "en";
        switch (Application.systemLanguage)
        {
            case SystemLanguage.Japanese:dflangStr = "ja-JP";break;
        }
        var langStr = Library_SaveFiles.LoadFileStr("", "Langage", dflangStr);
        var locale = LocalizationSettings.AvailableLocales.GetLocale(langStr);
        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
    }
    static public void LocailzLangChange()
    {
        var clang = LocalizationSettings.SelectedLocale.Identifier.Code;
        Library_SaveFiles.SaveFile("", "Langage", clang);
    }
    static public string LocailzString(string tableStr,string keyStr,bool nonMessages = false,string noKeyStr = "")
    {
        var table = LocalizationSettings.StringDatabase.GetTable(tableStr);
        if (table == null)
        {
            if (nonMessages) return tableStr + ":" + keyStr + "(テーブルがありません)";
            else if (noKeyStr != "") return StrEx(noKeyStr);
            else return StrEx(keyStr);
        }
        var entry = table.GetEntry(keyStr);
        if (entry == null)
        {
            if (nonMessages)return tableStr + ":" + keyStr + "(テーブル内にキーがありません)";
            else if (noKeyStr != "") return StrEx(noKeyStr);
            else return StrEx(keyStr);
        }
        if (string.IsNullOrEmpty(entry.LocalizedValue))
        {
            if (nonMessages) return tableStr + ":" + keyStr + "(中身が空です)";
            else if (noKeyStr != "") return StrEx(noKeyStr);
            else return StrEx(keyStr);
        }
        return StrEx(entry.LocalizedValue);
    }
    static public bool LocailzCheck(string tableStr, string keyStr)
    {
        var table = LocalizationSettings.StringDatabase.GetTable(tableStr);
        if (table == null) return false;
        var entry = table.GetEntry(keyStr);
        if (entry == null) return false;
        if (string.IsNullOrEmpty(entry.LocalizedValue)) return false;
        return true;
    }
    static public string LocailzSCInfo(string keyStr)
    {
        return LocailzString("SCInfo", keyStr);
    }
    static public string StrEx(string str)
    {
        var clang = LocalizationSettings.SelectedLocale.Identifier.Code;
        var s = "";
        var lstrs = new string[0];
        switch (clang)
        {
            default:
                return str;
            case "Tofu":
                lstrs = str.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                for(int i = 0; i < lstrs.Length; i++)
                {
                    if (i > 0) s += "\n";
                    s += new string('ロ', lstrs[i].Length);
                }
                return s;
            case "0-9":
                lstrs = str.Split(new[] { "\r\n", "\n", "\r" }, StringSplitOptions.None);
                for (int i = 0; i < lstrs.Length; i++)
                {
                    if (i > 0) s += "\n";
                    for (int k = 0; k < lstrs[i].Length; k++) s += (k % 10).ToString();
                }
                return s;
        }
    }
}
