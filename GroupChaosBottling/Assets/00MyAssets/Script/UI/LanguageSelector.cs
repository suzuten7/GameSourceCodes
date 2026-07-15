using System.Collections.Generic;
using TMPro;
using UnityEngine;

/// <summary>
/// 言語切り替えボタン用のスクリプト。
/// Unity UIのButtonコンポーネントのOnClickイベントから呼び出して使用します。
/// </summary>
public class LanguageSelector : MonoBehaviour
{

    [SerializeField]
    TMP_Dropdown dropdown;
    List<string> items = new List<string>() { "日本語", "English", "中文", "한국어" };


    /// <summary>
    /// 言語を日本語に設定します
    /// </summary>
    public void SetJapanese()
    {
        Debug.Log("言語変更を試みる0");

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguage(LocalizationManager.Language.JP);
            Debug.Log("Language changed to Japanese");
        }
    }

    /// <summary>
    /// 言語を英語に設定します
    /// </summary>
    public void SetEnglish()
    {
        Debug.Log("言語変更を試みる1");

        if (LocalizationManager.Instance != null)
        {
            LocalizationManager.Instance.SetLanguage(LocalizationManager.Language.EN);
            Debug.Log("Language changed to English");
        }
    }

    /// <summary>
    /// 日本語と英語を交互に切り替えます
    /// </summary>
    public void ToggleLanguage()
    {
        if (LocalizationManager.Instance != null)
        {
            var current = LocalizationManager.Instance.GetCurrentLanguage();
            var next = (current == LocalizationManager.Language.JP) 
                       ? LocalizationManager.Language.EN 
                       : LocalizationManager.Language.JP;
            
            LocalizationManager.Instance.SetLanguage(next);
            Debug.Log("Language toggled to: " + next);
        }
    }


    public void OnChangeValue()
    {
        switch (dropdown.value)
        {
            case 0:
                SetJapanese(); break;
            case 1:
                SetEnglish(); break;
        }
    }

    private void Start()
    {
        dropdown.ClearOptions();
        dropdown.AddOptions(items);
    }
}
