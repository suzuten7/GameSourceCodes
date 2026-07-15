using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/* 内容
 * ・詳細ウィンドウの【表示/非表示】と追従
 */

//文字表示の種類
enum DetailsSelect
{
    Default,
    Default_ColorPreset,
    Achievement,
}

public class Details_UI : MonoBehaviour
{
    #region 変数倉庫
    #region メインオプション
    [Header("◆メインオプション")]
    [SerializeField] Canvas canvas;
    [SerializeField] RectTransform parentRect;
    [SerializeField, Tooltip("移動する物")] RectTransform movePos;
    [SerializeField, Tooltip("詳細の種類" +
        "\n├Default            ：Details_SetUIの文字を適応" +
        "\n├Default_ColorPreset：DefaultとColorPresetのカラーコード" +
        "\n└Achievement        ：実績詳細")]
    DetailsSelect detailsSelect;
    [SerializeField, Tooltip("タイトルを表示するテキスト")]
    TextMeshProUGUI titleText;
    [SerializeField, Tooltip("詳細を表示するテキスト")]
    TextMeshProUGUI mainText;
    [SerializeField, Tooltip("ピボットからずらす距離(画面左下基準)")]
    Vector2 shift_Pos = new Vector2(20f, 20f);

    int round;
    #endregion
    #region その他
    [Header("◆その他")]
    [SerializeField, Tooltip("※Achievementのみ")]
    Data_Achievement data_Achv;

    UI_DetailSet d_SetUI = null;
    UI_ColorApply cp_ColorApply = null; //Default_ColorPresetのみ
    #endregion
    #endregion

    void Start() { movePos.gameObject.SetActive(false); }

    void LateUpdate()
    {
        #region カーソル移動
        Vector2 point = Player_Input.UIPoint;
        #endregion

        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        { position = point, };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);
        bool check = false;
        d_SetUI = null;
        //必要なスクリプトがあるか確認
        foreach (var res in results)
        {
            var dui = res.gameObject.GetComponent<UI_DetailSet>();
            cp_ColorApply = res.gameObject.GetComponent<UI_ColorApply>();

            //手前でブロックされていたら終了
            if (dui == null && cp_ColorApply == null
                && res.gameObject.TryGetComponent<Image>(out _)) break;
            if(dui != null)d_SetUI = dui;
            switch (detailsSelect)
            {
                //通常 or カラープリセット
                case DetailsSelect.Default:
                    if(dui != null && dui.achv_ID < 0)
                    {
                        D_DetailSet();
                        check = true;
                    }
                    break;
                case DetailsSelect.Default_ColorPreset:
                    if (dui != null && dui.achv_ID < 0)
                    {
                         D_DetailSet();
                          check = true;
                    }
                    //カラープリセットのみ
                    else if (detailsSelect == DetailsSelect.Default_ColorPreset&& cp_ColorApply != null)
                    {
                         CP_DetailSet();
                         check = true;
                    }
                    break;

                //実績
                case DetailsSelect.Achievement:
                    if (dui != null && dui.achv_ID >= 0)
                    {
                        A_DetailSet();
                        check = true;
                    }
                    break;
            }
        }
        movePos.gameObject.SetActive(check);
        Vector2 lpos = movePos.anchoredPosition;
        if (d_SetUI != null) lpos += d_SetUI.shiftAdd_Pos;

        var size = movePos.sizeDelta;
        lpos.x = Mathf.Clamp(lpos.x, parentRect.rect.xMin + size.x + 0, parentRect.rect.xMax - 0);
        lpos.y = Mathf.Clamp(lpos.y, parentRect.rect.yMin + size.y + 0, parentRect.rect.yMax - 0);

        movePos.anchoredPosition = lpos;
    }

    void Update()
    {
        var size = movePos.sizeDelta;
        Vector2 point = Player_Input.UIPoint;
        Vector2 lpos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect, point,
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out lpos);

        //位置をずらす
        lpos += shift_Pos;
        movePos.anchoredPosition = lpos;
        return;
        Debug.Log(d_SetUI != null);
        if (d_SetUI != null) lpos += d_SetUI.shiftAdd_Pos;


        lpos.x = Mathf.Clamp(lpos.x, parentRect.rect.xMin + size.x + 0, parentRect.rect.xMax - 0);
        lpos.y = Mathf.Clamp(lpos.y, parentRect.rect.yMin + size.y + 0, parentRect.rect.yMax - 0);

        movePos.anchoredPosition = lpos;
    }
    #region 文字の更新
    /// <summary>
    /// 通常
    /// </summary>
    void D_DetailSet()
    {
        titleText.text = d_SetUI.derailTitle != null ?
            d_SetUI.derailTitle : "";
        mainText.text = d_SetUI.derailMain != null ?
            d_SetUI.derailMain : "";
    }

    /// <summary>
    /// カラーパレット
    /// </summary>
    void CP_DetailSet()
    {
        string colorCode = $"{ColorUtility.ToHtmlStringRGBA(cp_ColorApply.image.color)}";

        //ColorCodeを2文字ずつ色分け
        string colorText =
            $"<color=#FFBFBF>{colorCode.Substring(0, 2)}</color>" + //R
            $"<color=#BFFFBF>{colorCode.Substring(2, 2)}</color>" + //G
            $"<color=#BFBFFF>{colorCode.Substring(4, 2)}</color>" + //B
            $"<color=#C0C0C0>{colorCode.Substring(6, 2)}</color>";  //A

        titleText.text = "";
        mainText.text = cp_ColorApply != null ?
            $"<size=85%><mark=#{colorCode}>　  </mark></size> <i>#{colorText}</i>" : "";
    }

    /// <summary>
    /// 実績
    /// </summary>
    void A_DetailSet()
    {

        for (int i = 0; i < data_Achv.achvList.Count; i++)
        {
            if (d_SetUI.achv_ID != i) continue;

            #region 取得難易度のセット
            string level_StrColor = ColorUtility.ToHtmlStringRGB
                (data_Achv.achv_StarData[data_Achv.achvList[i].achv_Level - 1].star_Color);
            string level_Str = $"<color=#{level_StrColor}>";

            for (int count = 0; count < data_Achv.achv_MaxLevel; count++)
            {
                level_Str += (data_Achv.achvList[i].achv_Level - count) > 0 ?
                    $"{data_Achv.achv_LevelChar}" : $"{data_Achv.achv_LevelCharSpace}";
            }
            level_Str += "</color>";
            #endregion
            #region 文字の変更
            titleText.text = $"</b>No.{d_SetUI.achv_ID + 1:D2}<b> ";

            //クリア済み
            if (data_Achv.achvList[i].achv_ClearFlag)
            {
                titleText.text += data_Achv.achvList[i].achv_Name;
                mainText.text = data_Achv.achvList[i].achv_Exp;
                mainText.text += $"\n<size=10%>\n</size><color=#CCCCCC>" +
                    $"<i>{data_Achv.achvList[i].achv_GetExp}</i></color>";
            }
            else
            {
                //未クリア(通常)
                if (!data_Achv.achvList[i].achv_SecretFlag)
                {
                    titleText.text += "<color=#CCCCCC>?????</color>";
                    mainText.text = data_Achv.achvList[i].achv_Exp;
                    mainText.text += $"\n<size=10%>\n</size><color=#CCCCCC>" +
                        $"<i>{data_Achv.achvList[i].achv_GetExp}</i></color>";
                }
                else
                {
                    titleText.text += "<color=#CCCCCC>?????</color>";
                    mainText.text = $"<color=#CCCCCC>???????????????</color>";
                    mainText.text += $"\n<size=10%>\n</size><color=#CCCCCC>" +
                        $"<i>{data_Achv.achvList[i].achv_SecretGetExp}</i></color>";
                }
            }

            titleText.text += $"\n<size=75%><i>取得難易度：{level_Str}</size></i>";
            #endregion
            break;
        }
    }
    #endregion
}
