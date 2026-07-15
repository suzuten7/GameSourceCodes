using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocalizDetail : MonoBehaviour
{
    [SerializeField] string tableStr = "Detail";
    UI_DetailSet dt;
    string keyStr;
    string bInfoStr;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dt = GetComponent<UI_DetailSet>();
        if (dt == null) enabled = false;
        keyStr = dt.derailTitle;
        bInfoStr = dt.derailMain;
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
        if (dt == null) return;
        if(keyStr != "")dt.derailTitle = LocalizSystem.LocailzString(tableStr, keyStr + "_Title",false,keyStr);
        if(bInfoStr != "")dt.derailMain = LocalizSystem.LocailzString(tableStr, keyStr + "_Main",false,bInfoStr);
    }
}
