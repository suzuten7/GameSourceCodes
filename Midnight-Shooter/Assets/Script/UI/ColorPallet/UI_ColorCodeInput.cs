using System.Text.RegularExpressions;
using UnityEngine;

/* 内容
 * ・カラーコードのシステム
 * └3/6/8文字の時に色適応
 */

public class UI_ColorCodeInput : MonoBehaviour
{
    string previousHex; //前回入れたの正常なカラーコード
    Color currentColor;
    UI_ColorPalletManager cpm;

    void Awake()
    { cpm = UI_ColorPalletManager.manager; }

    void OnEnable()
    {
        //初期値保存
        currentColor = cpm.before_ColorImage.color;
        previousHex = ColorUtility.ToHtmlStringRGBA(cpm.before_ColorImage.color);
        cpm.cc_InputField.text = previousHex;

        //入力中と確定時
        cpm.cc_InputField.onValueChanged.AddListener(OnValueChanged);
        cpm.cc_InputField.onEndEdit.AddListener(OnEndEdit);

        ApplyColor(currentColor);
    }

    /// <summary>
    /// 入力中
    /// </summary>
    void OnValueChanged(string text)
    {
        //16進数以外除去
        string filtered = Regex.Replace(text, "[^0-9a-fA-F]", "");

        if (filtered != text)
        {
            cpm.cc_InputField.text = filtered.ToUpper();
            cpm.cc_InputField.caretPosition = filtered.Length;
            return;
        }

        filtered = filtered.ToUpper();

        //3・6・8文字ならプレビュー反映
        PreviewColor(filtered);
    }

    /// <summary>
    /// 入力終了時
    /// </summary>
    void OnEndEdit(string text)
    {
        string hex = NormalizeHex(text.ToUpper());

        // 不正な文字数なら前回値へ戻す
        if (hex == null)
        {
            cpm.cc_InputField.text = previousHex;
            ApplyPreviousColor();
            return;
        }

        //カラーコードの適応
        if (ColorUtility.TryParseHtmlString("#" + hex, out Color parsedColor))
        {
            currentColor = parsedColor;
            previousHex = hex;

            cpm.cc_InputField.text = hex;
            ApplyColor(currentColor);
        }
        else
        {
            cpm.cc_InputField.text = previousHex;
            ApplyPreviousColor();
        }
    }

    /// <summary>
    /// 入力中のプレビュー反映
    /// </summary>
    void PreviewColor(string text)
    {
        string hex = NormalizeHex(text);

        if (hex == null) return;

        if (ColorUtility.TryParseHtmlString("#" + hex, out Color parsedColor))
        { ApplyColor(parsedColor); }
    }

    /// <summary>
    /// 3/6/8文字を8文字RGBAへ統一
    /// </summary>
    string NormalizeHex(string hex)
    {
        string alpha = ColorUtility.ToHtmlStringRGBA(currentColor).Substring(6, 2);

        //3文字
        if (hex.Length == 3)
        {
            return
                $"{hex[0]}{hex[0]}" +
                $"{hex[1]}{hex[1]}" +
                $"{hex[2]}{hex[2]}" +
                alpha;
        }
        //6文字
        else if (hex.Length == 6)
        { return hex + alpha; }
        //8文字
        else if (hex.Length == 8)
        { return hex; }

        return null;
    }

    /// <summary>
    /// 色反映(色確定前)
    /// </summary>
    void ApplyPreviousColor()
    {
        if (ColorUtility.TryParseHtmlString("#" + previousHex, out Color parsedColor))
        { ApplyColor(parsedColor); }
    }

    /// <summary>
    /// 色反映
    /// </summary>
    void ApplyColor(Color color)
    { cpm.after_ColorImage.color = color; }
}
