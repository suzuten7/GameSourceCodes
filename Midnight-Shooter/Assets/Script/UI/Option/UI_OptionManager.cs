using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

/* 内容
 * ・各種設定変更箇所の適応 または 取り消し
 */

public class UI_OptionManager : MonoBehaviour
{
    #region 変数倉庫
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("適応ボタン")] Button apply_Button;
    [SerializeField, Tooltip("取り消しボタン")] Button remove_Button;
    [SerializeField] GameObject[] windowUIs;
    [SerializeField] CanvasGroup canvasGroup;
    //◆その他
    [HideInInspector, SerializeField] List<UI_Switch> switches;
    [HideInInspector, SerializeField] List<UI_ScrollBar> scrollBars;
    [HideInInspector, SerializeField] List<UI_LoadColorPallet> loadColors;
    [HideInInspector, SerializeField] List<UI_KeySet> keySets;
    [SerializeField] UI_ReticleSelect_Base reticleSelects;
    [SerializeField] UI_ReticleLoadImg reticleLoads;
    [SerializeField] AudioMixer audioMix;
    bool setup = false;
    bool acvites = false;

    static Dictionary<string, object> dicValues = new();
    #endregion

    void Awake()
    {
        if (setup) return;
        setup = true;
        if (!acvites) gameObject.SetActive(false);
        //各種設定項目の取得
        UI_Switch[] switch_Array = GetComponentsInChildren<UI_Switch>();
        UI_ScrollBar[] scroll_Array = GetComponentsInChildren<UI_ScrollBar>();
        UI_LoadColorPallet[] loadColor_Array = GetComponentsInChildren<UI_LoadColorPallet>();

        for (int i = 0; i < switch_Array.Length; i++)
        {
            switches.Add(switch_Array[i]);
            switch_Array[i].Load();
        }
        for (int i = 0; i < scroll_Array.Length; i++)
        {
            scrollBars.Add(scroll_Array[i]);
            scroll_Array[i].Load();
        }
        for (int i = 0; i < loadColor_Array.Length; i++)
        {
            loadColors.Add(loadColor_Array[i]);
            loadColor_Array[i].Load();
        }
        var keySet_Array = GetComponentsInChildren<UI_KeySet>();
        for (int i = 0; i < keySet_Array.Length; i++)
        {
            keySets.Add(keySet_Array[i]);
            keySet_Array[i].Load();
        }
        reticleSelects.Load();
        reticleLoads.Load();

        apply_Button.interactable = false;
        remove_Button.interactable = false;

        DictionarySet();
        VolumeSet();
    }
    public void VolumeSet()
    {
        audioMix.SetFloat("Master_Volume", Volset(OptionGetFloat("S_Option 01", 50)));
        audioMix.SetFloat("BGM_Volume", Volset(OptionGetFloat("S_Option 02", 70)));
        audioMix.SetFloat("SE_Volume", Volset(OptionGetFloat("S_Option 03", 70)));
        audioMix.SetFloat("UI_Volume", Volset(OptionGetFloat("S_Option 04", 70)));
    }
    static float Volset(float val)
    {
       return Mathf.Clamp(Mathf.Log(val * 0.01f, 10) * 60f, -80f, 20f);
    }
    #region Tabの初期化処理
    void OnEnable()
    {
        canvasGroup.alpha = 0;
        StartCoroutine(CloseTabs());
    }

    /// <summary>
    /// 各Tabを閉じる(初期化処理)
    /// </summary>
    IEnumerator CloseTabs()
    {
        yield return null;

        UI_Tab[] tab_Array = GetComponentsInChildren<UI_Tab>(true);

        for (int i = 0; i < tab_Array.Length; i++)
        {
            tab_Array[i].mainPanel.SetActive(false);

            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(
                tab_Array[i].transform.parent as RectTransform);
        }

        canvasGroup.alpha = 1;
    }
    #endregion

    #region オプションのスクリプト起動
    public void SetUp()
    {
        Awake();
        acvites = true;
    }

    public void Open()
    {
        acvites = true;
        gameObject.SetActive(true);
        for(int i=0;i<windowUIs.Length;i++) windowUIs[i].SetActive(false);
    }
    #endregion
    #region オプションの処理変更
    /// <summary>
    /// 値変更の確認
    /// </summary>
    public void CheckChanged()
    {
        bool changed = false;
        //スイッチの変更取得
        for (int i = 0; i < switches.Count; i++)
        {
            if (switches[i].set_Value != switches[i].currentIndex)
            { changed = true; break; }
        }

        //スクロールバーの変更取得
        for (int i = 0; i < scrollBars.Count; i++)
        {
            if (scrollBars[i].set_Value != scrollBars[i].scrollbar.value)
            { changed = true; break; }
        }
        //カラーの変更取得
        for (int i = 0; i < loadColors.Count; i++)
        {
            if (loadColors[i].set_ColorCode != loadColors[i].now_ColorCode)
            { changed = true; break; }
        }
        //キー設定の変更取得
        for (int i = 0; i < keySets.Count; i++)
        {
            if (keySets[i].change)
            { changed = true; break; }
        }
        if (reticleSelects.cselID != reticleSelects.bselID) changed = true;
        if (reticleLoads.cselID != reticleLoads.bselID) changed = true;

        apply_Button.interactable = changed;
        remove_Button.interactable = changed;
    }

    /// <summary>
    /// 各種設定変更箇所の適応 または 取り消し
    /// </summary>
    /// <param name="apply_Flag"> true：適応 / false：取り消し </param>
    public void OptionChanges(bool apply_Flag)
    {
        //スイッチの変更箇所取得
        for (int i = 0; i < switches.Count; i++)
        {
            if (switches[i].set_Value == switches[i].currentIndex) continue;

            //設定の適応
            if (apply_Flag)
            {
                switches[i].set_Value = switches[i].currentIndex;
                Library_SaveFiles.SaveFile("Option", switches[i].gameObject.name, switches[i].currentIndex.ToString());
            }
            //設定の取り消し
            else
            { switches[i].currentIndex = switches[i].set_Value; }
            switches[i].Refresh();
        }

        //スクロールバーの変更箇所取得
        for (int i = 0; i < scrollBars.Count; i++)
        {
            if (scrollBars[i].set_Value == scrollBars[i].scrollbar.value) continue;

            //設定の適応
            if (apply_Flag)
            {
                scrollBars[i].set_Value = scrollBars[i].scrollbar.value;
                Library_SaveFiles.SaveFile("Option", scrollBars[i].gameObject.name, scrollBars[i].scrollbar.value.ToString());
            }
            //設定の取り消し
            else
            { scrollBars[i].scrollbar.value = scrollBars[i].set_Value; }
            scrollBars[i].OnScrollChanged(scrollBars[i].scrollbar.value);
        }

        //カラーの変更箇所取得
        for (int i = 0; i < loadColors.Count; i++)
        {
            //更新がかかった時のみ
            if (loadColors[i].set_ColorCode == loadColors[i].now_ColorCode) continue;

            //設定の適応
            if (apply_Flag)
            {
                loadColors[i].set_ColorCode = loadColors[i].now_ColorCode;
                Library_SaveFiles.SaveFile("Option", loadColors[i].gameObject.name, loadColors[i].now_ColorCode);
            }
            //設定の取り消し
            else
            { loadColors[i].now_ColorCode = loadColors[i].set_ColorCode; }

            loadColors[i].Reset_Color();
        }
        //キー設定
        for (int i = 0; i < keySets.Count; i++)
        {
            //更新がかかった時のみ
            if (!keySets[i].change) continue;

            //設定の適応
            if (apply_Flag)
            {
                keySets[i].Save();
            }
            //設定の取り消し
            else
            {
                keySets[i].Chancel();
            }
        }

        if (reticleSelects.cselID != reticleSelects.bselID)
        {
            if (apply_Flag)
            {
                reticleSelects.bselID = reticleSelects.cselID;
                Library_SaveFiles.SaveFile("Option", reticleSelects.gameObject.name, reticleSelects.cselID.ToString());
            }
            else reticleSelects.cselID = reticleSelects.bselID;
        }
        if (reticleLoads.cselID != reticleLoads.bselID)
        {
            if (apply_Flag)
            {
                reticleLoads.bselID = reticleLoads.cselID;
                Library_SaveFiles.SaveFile("Option", reticleLoads.gameObject.name, reticleLoads.cselID.ToString());
            }
            else reticleLoads.cselID = reticleLoads.bselID;
        }
        apply_Button.interactable = false;
        remove_Button.interactable = false;

        DictionarySet();
        VolumeSet();
    }
    #endregion
    #region 設定の値送信
    public void DictionarySet()
    {
        for(int i = 0; i < switches.Count; i++)
        {
            var key = switches[i].gameObject.name;
            var val = switches[i].set_Value;
            if (dicValues.ContainsKey(key)) dicValues[key] = val;
            else dicValues[key] = val;
        }
        for (int i = 0; i < scrollBars.Count; i++)
        {
            var key = scrollBars[i].gameObject.name;
            var val = Mathf.Lerp(scrollBars[i].value_Range.x, scrollBars[i].value_Range.y, Mathf.Pow(scrollBars[i].set_Value, scrollBars[i].scrolle_pow));
            if (dicValues.ContainsKey(key)) dicValues[key] = val;
            else dicValues[key] = val;
        }
        for (int i = 0; i < loadColors.Count; i++)
        {
            var key = loadColors[i].gameObject.name;
            var val = loadColors[i].set_ColorCode;
            if (dicValues.ContainsKey(key)) dicValues[key] = val;
            else dicValues[key] = val;
        }
        if(reticleSelects != null)
        {
            var key = reticleSelects.gameObject.name;
            var val = reticleSelects.cselID;
            if (dicValues.ContainsKey(key)) dicValues[key] = val;
            else dicValues[key] = val;
        }
        if (reticleLoads != null)
        {
            var key = reticleLoads.gameObject.name;
            var val = reticleLoads.cselID;
            if (dicValues.ContainsKey(key)) dicValues[key] = val;
            else dicValues[key] = val;
        }
    }

    static public bool OptionGetOnOff(string key, bool defalut)
    { return dicValues.TryGetValue(key, out var val) ? ((int)val > 0) : defalut; }
    static public int OptionGetInt(string key,int defalut)
    { return dicValues.TryGetValue(key,out var val) ? (int)val : defalut; }
    static public float OptionGetFloat(string key, float defalut)
    { return dicValues.TryGetValue(key, out var val) ? (float)val : defalut; }
    static public string OptionGetString(string key, string defalut)
    { return dicValues.TryGetValue(key, out var val) ? (string)val : defalut; }
    static public Color OptionGetColor(string key, Color defalut)
    {
        var col =  defalut;
        var ccode = dicValues.TryGetValue(key, out var val) ? (string)val : "";
        if (ccode != "" && ColorUtility.TryParseHtmlString("#" + ccode, out var cols)) col = cols;
        return col;
    }
    #endregion
    #region オプションのリセット処理
    /// <summary>
    /// 各オプションの初期化
    /// </summary>
    public void OptionReset()
    {
        for (int i = 0; i < switches.Count; i++)
        { switches[i].Resets(); }
        for (int i = 0; i < scrollBars.Count; i++)
        { scrollBars[i].Resets(); }
        for (int i = 0; i < loadColors.Count; i++)
        { loadColors[i].Resets(); }
        for (int i = 0; i < keySets.Count; i++)
        { keySets[i].Resets(); }
    }

    /// <summary>
    /// 保存値の初期化
    /// </summary>
    public void SaveRem(string folder)
    {
        Library_SaveFiles.SaveDelete(folder);
        if(folder == "Option")
        {
            setup = false;
            Awake();
        }
    }
    public void SaveRemAll()
    {
        SaveRem("Option");
        SaveRem("Color");
        SaveRem("PlayerCSets");
        SaveRem("ImgSets");
    }

    static public string ChangeStr(string str,bool change)
    {
        str = str.Replace("<color=#E5E617>*</color>", "");
        if(change)str += "<color=#E5E617>*</color>";
        return str;
    }
    #endregion
}
