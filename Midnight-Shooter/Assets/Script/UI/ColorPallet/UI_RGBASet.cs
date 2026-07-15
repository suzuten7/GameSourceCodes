using TMPro;
using UnityEngine;

public class UI_RGBASet : MonoBehaviour
{
    UI_ColorPalletManager cpm;
    HexNotation now_cpNotation;

    void Awake()
    { cpm = UI_ColorPalletManager.manager; }

    void OnEnable()
    {
        now_cpNotation = cpm.set_cpNotation;

        //H(色彩)
        cpm.hue_Bar.onValueChanged.AddListener(_ => UpdateColor());
        cpm.hue_InputField.onEndEdit.AddListener(H_InputChanged);
        cpm.hue_InputField.onSelect.AddListener(_ => cpm.hue_InputField.text = "");

        //S(彩度)
        cpm.chroma_Bar.onValueChanged.AddListener(_ => UpdateColor());
        cpm.chroma_InputField.onEndEdit.AddListener(S_InputChanged);
        cpm.chroma_InputField.onSelect.AddListener(_ => cpm.chroma_InputField.text = "");

        //V(明度)
        cpm.brightness_Bar.onValueChanged.AddListener(_ => UpdateColor());
        cpm.brightness_InputField.onEndEdit.AddListener(V_InputChanged);
        cpm.brightness_InputField.onSelect.AddListener(_ => cpm.brightness_InputField.text = "");

        //A(透明度)
        cpm.alpha_Bar.onValueChanged.AddListener(OnAlphaChanged);
        cpm.alpha_InputField.onEndEdit.AddListener(A_InputChanged);
        cpm.alpha_InputField.onSelect.AddListener(_ => cpm.alpha_InputField.text = "");

        SetInputValidation();
        SetFieldText(cpm.hue_InputField, cpm.hue_Bar.value);
        SetFieldText(cpm.chroma_InputField, cpm.chroma_Bar.value);
        SetFieldText(cpm.brightness_InputField, cpm.brightness_Bar.value);
        SetFieldText(cpm.alpha_InputField, cpm.alpha_Bar.value);
    }

    //表記変更検知
    void Update()
    {
        if (now_cpNotation == cpm.set_cpNotation) return;
        now_cpNotation = cpm.set_cpNotation;

        SetInputValidation();
        SetFieldText(cpm.hue_InputField, cpm.hue_Bar.value);
        SetFieldText(cpm.chroma_InputField, cpm.chroma_Bar.value);
        SetFieldText(cpm.brightness_InputField, cpm.brightness_Bar.value);
        SetFieldText(cpm.alpha_InputField, cpm.alpha_Bar.value);
    }

    /// <summary>
    /// HexNotationに応じて入力制限を切り替え
    /// Hex     : 0-9のみ / 3文字
    /// Percent : 0-9 と . / 5文字
    /// </summary>
    void SetInputValidation()
    {
        switch (cpm.set_cpNotation)
        {
            //Hex(0～255)
            case HexNotation.Hex:

                //H(色彩)
                SetInput(cpm.hue_InputField, 3,
                    TMP_InputField.CharacterValidation.Integer);
                cpm.hue_Placeholder.text = "###";

                //V(彩度)
                SetInput(cpm.chroma_InputField, 3,
                    TMP_InputField.CharacterValidation.Integer);
                cpm.chroma_Placeholder.text = "###";

                //S(明度)
                SetInput(cpm.brightness_InputField, 3,
                    TMP_InputField.CharacterValidation.Integer);
                cpm.brightness_Placeholder.text = "###";

                //A(透明度)
                SetInput(cpm.alpha_InputField, 3,
                    TMP_InputField.CharacterValidation.Integer);
                cpm.alpha_Placeholder.text = "###";

                break;

            //Percent(0～100)
            case HexNotation.Percent:

                //H(色彩)
                SetInput(cpm.hue_InputField, 5,
                    TMP_InputField.CharacterValidation.Decimal);
                cpm.hue_Placeholder.text = "###.#";

                //V(彩度)
                SetInput(cpm.chroma_InputField, 5,
                    TMP_InputField.CharacterValidation.Decimal);
                cpm.chroma_Placeholder.text = "###.#";

                //S(明度)
                SetInput(cpm.brightness_InputField, 5,
                    TMP_InputField.CharacterValidation.Decimal);
                cpm.brightness_Placeholder.text = "###.#";

                //A(透明度)
                SetInput(cpm.alpha_InputField, 5,
                    TMP_InputField.CharacterValidation.Decimal);
                cpm.alpha_Placeholder.text = "###.#";

                break;
        }
    }

    /// <summary>
    /// 制限などの変更
    /// </summary>
    /// <param name="input"> 入れる文字 </param>
    /// <param name="limit"> 文字数制限 </param>
    void SetInput(TMP_InputField input,int limit,
    TMP_InputField.CharacterValidation validation)
    {
        input.characterLimit = limit;
        input.characterValidation = validation;
    }

    #region Bar変更 → Color変換 → InputField文字変換
    /// <summary>
    /// Bar変更 → Color変換
    /// </summary>
    void UpdateColor()
    {
        Color color = Color.HSVToRGB
           (cpm.hue_Bar.value,
            cpm.chroma_Bar.value,
            cpm.brightness_Bar.value);

        color.a = cpm.alpha_Bar.value;
        cpm.ApplyColor(color, true);
    }

    /// <summary>
    /// Color → InputField文字変換
    /// </summary>
    public void SetFieldText(TMP_InputField input, float value)
    {
        switch (cpm.set_cpNotation)
        {
            case HexNotation.Hex:
                input.text = Library_UI.FormatNum(value * 255f);
                break;

            case HexNotation.Percent:
                input.text = $"{Library_UI.FormatNum(value * 100f, "FFFFFF", 3 , 1)}<size=75%> %</size>";
                break;
        }
    }
    #endregion
    #region InputField変更 → Bar変換
    /// <summary>
    /// InputField変更 → Bar変換(色彩)
    /// </summary>
    void H_InputChanged(string text)
    {
        text = text.Replace("%", "").Trim();

        if (!float.TryParse(text, out float value))
            return;

        switch (cpm.set_cpNotation)
        {
            case HexNotation.Hex:
                value = Mathf.Clamp(value, 0f, 255f);
                cpm.hue_Bar.value = value / 255f;
                break;

            case HexNotation.Percent:
                value = Mathf.Clamp(value, 0f, 100f);
                //100%だけ少し手前に寄せる
                //※100%だとFF0000判定で0%になってしまうため
                if (value >= 100f)
                    value = 99.99f;

                cpm.hue_Bar.value = value / 100f;
                break;
        }

        UpdateColor();
    }

    /// <summary>
    /// InputField変更 → Bar変換(彩度)
    /// </summary>
    void S_InputChanged(string text)
    {
        cpm.chroma_Bar.value = ParseInputValue(text);
        UpdateColor();
    }

    /// <summary>
    /// InputField変更 → Bar変換(明度)
    /// </summary>
    void V_InputChanged(string text)
    {
        cpm.brightness_Bar.value = ParseInputValue(text);
        UpdateColor();
    }

    /// <summary>
    /// InputField変更 → Bar変換(透明度)
    /// </summary>
    void A_InputChanged(string text)
    {
        float value = ParseInputValue(text);
        value = Mathf.Clamp(value, cpm.min_AlphaValue, 1f);
        cpm.alpha_Bar.value = value;

        UpdateColor();
    }

    /// <summary>
    /// min_AlphaValueより小さくならない
    /// </summary>
    void OnAlphaChanged(float value)
    {
        if (value < cpm.min_AlphaValue)
        {
            cpm.alpha_Bar.SetValueWithoutNotify(cpm.min_AlphaValue);
            value = cpm.min_AlphaValue;
        }

        UpdateColor();
    }

    /// <summary>
    /// 値を文字に変換
    /// </summary>
    float ParseInputValue(string text)
    {
        text = text.Replace("%", "").Trim();

        if (!float.TryParse(text, out float value)) return 0f;

        switch (cpm.set_cpNotation)
        {
            case HexNotation.Hex:
                value = Mathf.Clamp(value, 0f, 255f);
                return value / 255f;

            case HexNotation.Percent:
                value = Mathf.Clamp(value, 0f, 100f);
                return value / 100f;
        }

        return 0f;
    }
    #endregion
}
