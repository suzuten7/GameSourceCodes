using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・プレイヤーステータス関連のUI
 */

public class UI_Player : MonoBehaviour
{
    #region 変数倉庫
    #region メインオプション
    [Header("◆メインオプション")]
    [Header("- 共通 -")]
    [SerializeField, Tooltip("通常時の文字色")] Color n_TextCol;
    [SerializeField, Tooltip("残り0の文字色")] Color z_TextCol;
    [SerializeField, Tooltip("特別な文字色")] Color s_TextCol;
    [SerializeField, Tooltip("通常時のバー色")] Color n_BarCol;
    [SerializeField, Tooltip("特別なバー色")] Color s_BarCol;
    [Header("- HP -")]
    [SerializeField] TextMeshProUGUI textHP;
    [SerializeField] Image injury_Bar;
    [SerializeField] Image overHP_Bar;
    [SerializeField] Image nowHP_Bar;
    [Header("- スコア -")]
    [SerializeField] TextMeshProUGUI textKillScore;
    [Header("- 弾 -")]
    [SerializeField] TextMeshProUGUI setcWeponTypeTx;
    [SerializeField] TextMeshProUGUI setcBulletTx;
    [SerializeField] TextMeshProUGUI set2WeponTypeTx;
    [SerializeField] TextMeshProUGUI set2BulletTx;
    [SerializeField] Image nowReloadTime_Bar;
    [SerializeField] CanvasGroup reload_canvasG;

    [Header("- ガジェット -")]
    [SerializeField] TextMeshProUGUI textGudget;
    [SerializeField] Image gudget_Icon;
    [SerializeField] Image gudget_Bar;
    [Header("- 必殺 -")]
    [SerializeField] TextMeshProUGUI ultTx;
    [SerializeField] Image ultChargeBar;
    [Header("◆その他")]
    [SerializeField] List<UI_Buf> bufUIs;
    [SerializeField] TextMeshProUGUI textIndex;
    [SerializeField] Color dyingColor;
    [SerializeField] Color deathColor;
    [SerializeField] CanvasGroup dyingAlphas;
    [SerializeField] Image dyingEffect;
    [SerializeField] Image nowDrowningTime_Bar;
    [SerializeField] CanvasGroup drowning_canvasG;
    #endregion
    #endregion
    float reloadAlpha;
    float drowingAlpha;
    private void Start()
    {
        reloadAlpha = reload_canvasG.alpha;
        drowingAlpha = drowning_canvasG.alpha;
        dyingAlphas.alpha = 0;
    }
    void Update()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        if (pm == null) return;
        UIUpdate_HP();
        textKillScore.text = "Kill:" + pm.values.kill + "\nScore:" + pm.values.score.ToString("F0");
        UIUpdate_Bullet();
        UIUpdate_Gudget();
        UIUpdate_Ult();
        UIUpdate_Buf();
        UIUpdate_DyingEffect();
        //UIUpdate_Index();
        drowning_canvasG.alpha = pm.values.suffocationTime > 0 ? drowingAlpha : 0;
        nowDrowningTime_Bar.fillAmount = pm.values.suffocationTime / pm.states.suffocationTime;
    }

    /// <summary>
    /// HP関連の処理
    /// </summary>
    void UIUpdate_HP()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        injury_Bar.fillAmount = pm.values.hpInjury / pm.states.max_HP;
        overHP_Bar.fillAmount = pm.hpTotal / pm.states.max_HP;
        nowHP_Bar.fillAmount = pm.values.hpNow / pm.states.max_HP;

        string colorCode = ColorUtility.ToHtmlStringRGB(n_TextCol);

        textHP.text = $"{Library_UI.FormatNum((int)pm.hpTotal, colorCode)} / " +
            $"{Library_UI.FormatNum((int)pm.values.hpInjury, colorCode)}";
    }

    /// <summary>
    /// 弾丸関連の処理
    /// </summary>
    void UIUpdate_Bullet()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        var gund = Data_Base.DB.guns[pm.states.gun_IndexNum];
        var meleed = Data_Base.DB.melles[pm.states.melee_IndexNum];
        float treload = 0;
        switch (pm.values.now_CursorState)
        {
            default:treload = gund.tReloadTime; break;
            case CursorState.Melee:treload = meleed.tReloadTime; break;
        }
        //バーの長さ変更
        if (pm.values.now_ReloadTime > 0)
        {
            reload_canvasG.alpha = reloadAlpha;
            nowReloadTime_Bar.fillAmount =
                pm.values.now_ReloadTime / pm.values.set_ReloadTime;
        }
        else { reload_canvasG.alpha = 0f; }

        #region 色替え
        string colorCode = ColorUtility.ToHtmlStringRGB(n_TextCol);
        nowReloadTime_Bar.color =
            pm.values.set_ReloadTime == treload
            ? s_BarCol : n_BarCol;
        
        //残弾0
        if (pm.values.gun_bullet == 0)
        { colorCode = ColorUtility.ToHtmlStringRGB(z_TextCol); }
        //タクティカルリロード時
        else if (pm.values.gun_bullet > pm.magazinMax)
        { colorCode = ColorUtility.ToHtmlStringRGB(s_TextCol); }
        #endregion

        string gunCurrentstr = pm.values.gun_bullet >= 0 ? Library_UI.FormatNum(pm.values.gun_bullet, colorCode) : "<rotate=90>8</rotate>";
        string gunMaxstr = pm.magazinMax >= 0 ? Library_UI.FormatNum(pm.magazinMax, ColorUtility.ToHtmlStringRGB(n_TextCol)) : "<rotate=90>8</rotate>";

        colorCode = ColorUtility.ToHtmlStringRGB(n_TextCol);
        if (pm.values.melee_bullet == 0)
        { colorCode = ColorUtility.ToHtmlStringRGB(z_TextCol); }
        //タクティカルリロード時
        else if (pm.values.melee_bullet > pm.meleeMax)
        { colorCode = ColorUtility.ToHtmlStringRGB(s_TextCol); }
        string meleeCurrent2str = pm.values.melee_bullet >= 0 ? Library_UI.FormatNum(pm.values.melee_bullet, colorCode) : "<rotate=90>8</rotate>";
        string meleeMax2str = pm.meleeMax >= 0 ? Library_UI.FormatNum(pm.meleeMax, ColorUtility.ToHtmlStringRGB(n_TextCol)) : "<rotate=90>8</rotate>";


        switch(pm.values.now_CursorState)
        {
            default:
                setcWeponTypeTx.text = LocalizSystem.LocailzSCInfo("銃");
                setcBulletTx.text = $"{gunCurrentstr}/{gunMaxstr}";
                set2WeponTypeTx.text = LocalizSystem.LocailzSCInfo("近接");
                set2BulletTx.text = $"{meleeCurrent2str}/{meleeMax2str}";
                break;
            case CursorState.Melee:
                setcWeponTypeTx.text = LocalizSystem.LocailzSCInfo("近接");
                setcBulletTx.text = $"{meleeCurrent2str}/{meleeMax2str}";
                set2WeponTypeTx.text = LocalizSystem.LocailzSCInfo("銃");
                set2BulletTx.text = $"{gunCurrentstr}/{gunMaxstr}";
                break;
        }
        //文字変更

    }

    /// <summary>
    /// ガジェット関連の処理
    /// </summary>
    void UIUpdate_Gudget()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        //アイコンの変更処理
        //未実装
        //ガシェットデータ
        var ggd = Data_Base.DB.gadgets[pm.states.gadget_IndexNum];
        var ggmax = ggd.max_Retention + pm.passc.ggStockAdd;
        //現在の所持状況
        gudget_Bar.fillAmount
            = pm.values.now_Retention != ggmax
            ? pm.values.now_GetTime / ggd.get_Time * 0.75f : 0f;

        #region 色替え
        string colorCode = ColorUtility.ToHtmlStringRGB(n_TextCol);
        //残り0の時
        if (pm.values.now_Retention == 0)
        { colorCode = ColorUtility.ToHtmlStringRGB(z_TextCol); }
        //満タンの時
        else if (pm.values.now_Retention >= ggmax)
        { colorCode = ColorUtility.ToHtmlStringRGB(s_TextCol); }
        #endregion

        textGudget.text =
                $"<color=#{colorCode}>{Library_UI.FormatNum(pm.values.now_Retention, colorCode, 1)}</color>";
    }
    /// <summary>
    /// 必殺の処理
    /// </summary>
    void UIUpdate_Ult()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        var ultd = Data_Base.DB.ults[pm.states.ult_IndexNum];
        ultTx.text = pm.values.ultCharge.ToString("F0") + "/" + ultd.chargeValue.ToString("F0");
        ultChargeBar.fillAmount = pm.values.ultCharge / ultd.chargeValue;
        if(pm.values.ultCharge >= ultd.chargeValue)
        {
            ultChargeBar.color = Color.HSVToRGB(Mathf.Repeat(Time.time * 0.1f, 1f), 1f, 1f);
        }
        else
        {
            ultChargeBar.color = Color.yellow;
        }
    }
    /// <summary>
    /// バフの処理
    /// </summary>
    void UIUpdate_Buf()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        for (int i = 0; i < Mathf.Max(bufUIs.Count, pm.values.bufs.Count); i++)
        {
            if(i >= pm.values.bufs.Count)
            {
                bufUIs[i].gameObject.SetActive(false);
                continue;
            }
            if (i >= bufUIs.Count) bufUIs.Add(Instantiate(bufUIs[0], bufUIs[0].transform.parent));
            var bui = bufUIs[i];
            var buf = pm.values.bufs[i];
            bui.gameObject.SetActive(true);
            var bufd = Data_Base.BufDGet(buf.buf);
            bui.timeFill.fillAmount = buf.timeCurrent / buf.timeMax;
            if(bufd != null)
            {
                bui.backIm.color = bufd.col;
                bui.nameTx.text = LocalizSystem.LocailzString("BufName", bufd.name);
                bui.iconIm.texture = bufd.icon;
                bui.iconIm.color = bufd.iconColor;
            }
            else
            {
                bui.backIm.color = Color.white;
                bui.nameTx.text = buf.buf.ToString();
                bui.iconIm.texture = null;
            }

        }
    }

    void UIUpdate_DyingEffect()
    {
        var pm = Obj_LocalObjects.MyPlayer;
        if (pm.NearDeath)
        {
            dyingAlphas.alpha += Time.deltaTime;
        }
        else
        {
            dyingAlphas.alpha -= Time.deltaTime;
        }
        dyingAlphas.alpha = Mathf.Clamp01(dyingAlphas.alpha);
        var dycolor = pm.hpTotal > 0 ? dyingColor : deathColor;
        if (pm.hpTotal <= 0) dycolor.a = 1f;
        else dycolor.a = (Mathf.Sin(Time.time * 5) + 1) * 0.5f;
        dycolor.a *= UI_OptionManager.OptionGetFloat("UI_Option 34", 50) * 0.01f;
        dyingEffect.color = dycolor;
    }
    /// <summary>
    /// Debug
    /// </summary>
    void UIUpdate_Index()
    {
                var pm = Obj_LocalObjects.MyPlayer;
        int gunIndex = pm.states.gun_IndexNum;
        int melee= pm.states.melee_IndexNum;
        int gadgetIndex = pm.states.gadget_IndexNum;
        int ultIndex = pm.states.ult_IndexNum;
        textIndex.text
            = $"Guns:       {Data_Base.DB.guns[gunIndex].name}"
            + $"<size=50%>[{gunIndex + 1} / {Data_Base.DB.guns.Count}]</size>\n"
            + $"Melees: {Data_Base.DB.melles[melee].name}"
            + $"<size=50%>[{melee + 1} / {Data_Base.DB.melles.Count}]</size>\n"
            + $"Gadgets: {Data_Base.DB.gadgets[gadgetIndex].name}"
            + $"<size=50%>[{gadgetIndex + 1} / {Data_Base.DB.gadgets.Count}]</size>\n"
            + $"Ults: {Data_Base.DB.ults[ultIndex].name}"
            + $"<size=50%>[{ultIndex + 1} / {Data_Base.DB.ults.Count}]</size>";

    }
}
