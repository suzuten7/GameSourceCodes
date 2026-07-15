using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/* 内容
 * ・文字をこのスクリプトが入ったオブジェクトの下に文字を出す
 * ・3段階のモードを切り替えれる
 * ├ミックスモード　　　：下の2つの機能を組み合わせたもの
 * │└カーソルが…
 * │　├合わさってない：ショートカットモードに切り替える
 * │　└合わさってる　：アイコン情報モードに切り替える
 * ├アイコン情報モード　：アイコンにカーソルを合わせると文字が出る
 * └ショートカットモード：文字をオブジェクトの下に出す(PC時のみ)
 */

#region 変数倉庫1
/// <summary>
/// 各種モードの切り替え
/// </summary>
public enum ModeSet
{
    Mix,
    IconInfo,
    ShortCut
}
#endregion

public class UI_IconInfo : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    #region 変数倉庫2
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("モードの切り替え")]
    ModeSet mode = ModeSet.Mix;

    [SerializeField, Tooltip("切り替えるまでの時間\n※アイコン情報モードがオンの時のみ")]
    float changeTime = 0.25f;
    float nowAlpha = 1f;
    [SerializeField, Tooltip("アイコン情報モード")]
    string iconInfo_Text;
    [SerializeField, Tooltip("ショートカットモードのテキスト")]
    string shortCut_Text;
    [SerializeField, Tooltip("表示先")]
    TextMeshProUGUI text;
    string targetText;

    bool point_Flag;
    bool fade_Flag;
    bool changeText_Flag;
    #endregion

    public void OnPointerEnter(PointerEventData eventData)
    { point_Flag = true; }

    public void OnPointerExit(PointerEventData eventData)
    { point_Flag = false; }

    void Update()
    {
        switch (mode)
        {
            //アイコン情報モード
            case ModeSet.IconInfo:
                if (point_Flag) { FadeText(iconInfo_Text, changeTime); }
                else { FadeText("", changeTime); }
                break;

            //ショートカットモード
            case ModeSet.ShortCut:
#if UNITY_STANDALONE || UNITY_EDITOR
                text.text = shortCut_Text;
#else
                text.text = "";
#endif
                break;

            //ミックスモード
            case ModeSet.Mix:
                if (point_Flag) { FadeText(iconInfo_Text, changeTime); }
                else
                {
#if UNITY_STANDALONE || UNITY_EDITOR
                    FadeText(shortCut_Text, changeTime);
#else
                    FadeText("", changeTime);
#endif
                }
                break;
        }

        if (text.text == "") { text.gameObject.SetActive(false); }
        else { text.gameObject.SetActive(true); }
    }

    /// <summary>
    /// 文字の変更
    /// </summary>
    /// <param name="nextText"> 変更後の文字 </param>
    /// <param name="fadeTime"> フェードの時間 </param>
    void FadeText(string nextText, float fadeTime)
    {
        Color color = text.color;

        //変更する文字が違うならフェードアウト開始
        if (targetText != nextText)
        {
            targetText = nextText;
            nowAlpha = 0f;
        }

        //alphaを目標値まで移動
        color.a = Mathf.MoveTowards(
            color.a,
            nowAlpha,
            Time.deltaTime / fadeTime);

        text.color = color;

        //完全に消えたら文字変更してフェードイン
        if (nowAlpha == 0f && color.a <= 0.001f)
        {
            text.text = targetText;
            nowAlpha = 1f;
        }
    }
}
