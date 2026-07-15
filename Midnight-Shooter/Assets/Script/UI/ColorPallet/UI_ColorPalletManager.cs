using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・カラーパレット全体のシステム
 */

#region 変数倉庫1
public enum HexNotation
{
    Hex,     //16進数
    Percent, //パーセント
}
#endregion

public class UI_ColorPalletManager : MonoBehaviour
{
    #region 変数倉庫2
    #region メインオプション
    [Header("◆メインオプション")]
    [Header("- 色プレビュー画像 -")]
    [Tooltip("初期の色プレビュー画像")] public Image default_ColorImage;
    [Tooltip("適応前の色プレビュー画像")] public Image before_ColorImage;
    [Tooltip("適応後の色プレビュー画像")] public Image after_ColorImage;
    [HideInInspector,Tooltip("アルファ値の最低値")]
    public float min_AlphaValue = 0f;

    [Header("- 上部の設定部分 -")]
    [Header("カラーフィールド")]
    [Tooltip("カラーフィールドの背景")] public RawImage cf_ColorFieldBG;
    [Tooltip("カラーフィールドのハンドル")] public Image cf_Handle;
    [Header("カラープリセット")]
    [SerializeField] GameObject ColorPreset_Prefab;
    [SerializeField] Transform cp_PrefabPos;
    List<GameObject> presetList = new List<GameObject>();

    [Header("- 下部の設定部分 -")]
    [Header("色彩")]
    [Tooltip("色彩のバー")] public Slider hue_Bar;
    [Tooltip("色彩のハンドル")] public Image hue_Handle;
    [Tooltip("色彩の背景")] public RawImage hue_BG;
    [Tooltip("色彩の文字変換")] public TMP_InputField hue_InputField;
    [Tooltip("色彩の仮文字")] public TextMeshProUGUI hue_Placeholder;
    [Header("彩度")]
    [Tooltip("彩度のバー")] public Slider chroma_Bar;
    [Tooltip("彩度のハンドル")] public Image chroma_Handle;
    [Tooltip("彩度の背景")] public RawImage chroma_BG;
    [Tooltip("彩度の文字変換")] public TMP_InputField chroma_InputField;
    [Tooltip("彩度の仮文字")] public TextMeshProUGUI chroma_Placeholder;
    [Header("明度")]
    [Tooltip("明度のバー")] public Slider brightness_Bar;
    [Tooltip("明度のハンドル")] public Image brightness_Handle;
    [Tooltip("明度の背景")] public RawImage brightness_BG;
    [Tooltip("明度の文字変換")] public TMP_InputField brightness_InputField;
    [Tooltip("明度の仮文字")] public TextMeshProUGUI brightness_Placeholder;
    [Header("透明度")]
    [Tooltip("透明度のバー")] public Slider alpha_Bar;
    [Tooltip("透明度のハンドル")] public Image alpha_Handle;
    [Tooltip("透明度の背景")] public RawImage alpha_BG;
    [Tooltip("透明度の文字変換")] public TMP_InputField alpha_InputField;
    [Tooltip("透明度の仮文字")] public TextMeshProUGUI alpha_Placeholder;
    [Tooltip("透明度の動かせる部分")] public Image alpha_CanMoveBar;

    [Header("- 細かい設定部分 -")]
    [Tooltip("カラーコードの入力場所")] public TMP_InputField cc_InputField;
    [Tooltip("文字変換の場所")] public TMP_Dropdown cp_Dorpdown;
    #endregion
    #region サブオプション
    [HideInInspector] public HexNotation set_cpNotation;
    #endregion
    #region その他
    [Tooltip("カラーパレットマネージャー")] static public UI_ColorPalletManager manager;
    [HideInInspector] public UI_LoadColorPallet loadColorPallet; //カラーフィールドの呼び出し
    UI_ColorField cp_ColorField; //カラーフィールド
    UI_RGBASet cp_RGBASet;       //カラーパレットのバー
    UI_ColorCodeInput cc_Input;  //カラーコード
    UI_HexNotation cp_HNChanger; //カラーパレットの数字変換
    #endregion
    #endregion

    void Awake()
    {
        manager = this;

        cp_ColorField = GetComponent<UI_ColorField>();
        cp_RGBASet = GetComponent<UI_RGBASet>();
        cc_Input = GetComponent<UI_ColorCodeInput>();
        cp_HNChanger = GetComponent<UI_HexNotation>();

        if (cp_ColorField == null) cp_ColorField = gameObject.AddComponent<UI_ColorField>();
        if (cp_RGBASet == null) cp_RGBASet = gameObject.AddComponent<UI_RGBASet>();
        if (cc_Input == null) cc_Input = gameObject.AddComponent<UI_ColorCodeInput>();
        if (cp_HNChanger == null) cp_HNChanger = gameObject.AddComponent<UI_HexNotation>();

        LoadPresetColors();
    }

    /// <summary>
    /// ColorPalletの適応
    /// </summary>
    public void Appry_ColorPallet()
    {
        if (loadColorPallet == null) return;
        alpha_CanMoveBar.fillAmount = 1 - min_AlphaValue;

        ApplyColor(before_ColorImage.color);
    }

    /// <summary>
    /// ColorPalletの初期化
    /// </summary>
    public void Reset_ColorPallet(Color color)
    { ApplyColor(color, resetHue : true); }

    #region カラープリセット関連
    /// <summary>
    /// 色の変更
    /// </summary>
    public void ApplyColor(Color color, bool keepHue = false, bool resetHue = false)
    {
        //プレビュー更新
        after_ColorImage.color = color;
        loadColorPallet.Get_ColorPallet(color);

        //今の値を退避
        float currentH = hue_Bar.value;
        float currentS = chroma_Bar.value;

        //RGB → HSV
        Color.RGBToHSV(color, out float h, out float s, out float v);

        //リセット時
        if (resetHue) { h = 0f; }
        //色の保持
        else if (keepHue) { h = currentH; }

        //S=0（グレー）ならH維持
        if (!resetHue && s <= 0.0001f) h = currentH;

        //V=0（黒）ならH/S維持
        if (!resetHue && v <= 0.0001f)
        {
            h = currentH;
            s = currentS;
        }

        //Slider反映
        hue_Bar.SetValueWithoutNotify(h);
        chroma_Bar.SetValueWithoutNotify(s);
        brightness_Bar.SetValueWithoutNotify(v);

        float alpha = Mathf.Clamp(color.a, min_AlphaValue, 1f);
        alpha_Bar.SetValueWithoutNotify(alpha);
        color.a = alpha;
        alpha_Bar.SetValueWithoutNotify(color.a);

        //HSVA InputField更新
        cp_RGBASet.SetFieldText(hue_InputField, h);
        cp_RGBASet.SetFieldText(chroma_InputField, s);
        cp_RGBASet.SetFieldText(brightness_InputField, v);
        cp_RGBASet.SetFieldText(alpha_InputField, color.a);

        //HEX文字更新
        cc_InputField.text =
            ColorUtility.ToHtmlStringRGBA(color);

        //バー背景・ハンドル更新
        BarBGColorSet();

        //カラーフィールドハンドル位置更新
        cp_ColorField.SetHandlePosition(s, v);
        Color bgColor = cf_ColorFieldBG.color;
        bgColor.a = color.a;
        cf_ColorFieldBG.color = bgColor;
    }

    /// <summary>
    /// カラープリセットの保存
    /// </summary>
    void SavePresetColors()
    {
        Library_SaveFiles.SaveFile("Color","PresetCount", presetList.Count.ToString());

        for (int i = 0; i < presetList.Count; i++)
        {
            if (presetList[i] == null) continue;

            Image img = presetList[i].GetComponent<Image>();
            if (img == null) continue;

            string colorCode = ColorUtility.ToHtmlStringRGBA(img.color);
            Library_SaveFiles.SaveFile("Color",$"PresetColor_{i}", colorCode);
        }
    }

    /// <summary>
    /// カラープリセットのロード
    /// </summary>
    void LoadPresetColors()
    {
        int count = Library_SaveFiles.LoadFileInt("Color","PresetCount", 0);

        for (int i = 0; i < count; i++)
        {
            string colorCode = Library_SaveFiles.LoadFileStr("Color",$"PresetColor_{i}");
            GameObject preset = Instantiate(ColorPreset_Prefab, cp_PrefabPos);
            presetList.Add(preset);

            if (ColorUtility.TryParseHtmlString("#" + colorCode, out Color color))
            {
                Image img = preset.GetComponent<Image>();
                if (img != null) img.color = color;
            }
        }
    }

    /// <summary>
    /// カラープリセットの保存
    /// </summary>
    public void Add_ColorPreset()
    {
        GameObject preset = Instantiate(ColorPreset_Prefab, cp_PrefabPos);

        Image img = null;

        foreach (Image child in preset.GetComponentsInChildren<Image>(true))
        {
            if (child.gameObject.name == "Color")
            { img = child; break; }
        }
        if (img != null) img.color = after_ColorImage.color;
        presetList.Add(preset);

        SavePresetColors();
    }

    /// <summary>
    /// カラープリセットの削除
    /// </summary>
    public void Delete_ColorPreset()
    {
        if (presetList.Count == 0) return;

        int lastIndex = presetList.Count - 1;
        GameObject deleteObj = presetList[lastIndex];
        presetList.RemoveAt(lastIndex);

        if (deleteObj != null) Destroy(deleteObj);
        SavePresetColors();
    }
    #endregion

    #region バーの背景グラデとハンドル色変更
    /// <summary>
    /// バーの背景グラデとハンドル色変更
    /// </summary>
    public void BarBGColorSet()
    {
        HueGradient();
        SaturationGradient();
        BrightnessGradient();
        AlphaGradient();

        HandleColorSet();

        ColorFieldGradient();
    }
    #region バーの背景グラデ表示
    /// <summary>
    /// 色彩のグラデ表示
    /// </summary>
    public void HueGradient()
    {
        Texture2D tex = new Texture2D(256, 1);

        for (int x = 0; x < 256; x++)
        {
            float h = x / 255f;
            tex.SetPixel(x, 0, Color.HSVToRGB(h, 1f, 1f));
        }

        tex.Apply();
        hue_BG.texture = tex;
    }
    /// <summary>
    /// 彩度のグラデ表示
    /// </summary>
    void SaturationGradient()
    {
        Texture2D tex = new Texture2D(256, 1);

        for (int x = 0; x < 256; x++)
        {
            float s = x / 255f;

            Color color = Color.HSVToRGB
               (hue_Bar.value,
                s,
                Mathf.Clamp(brightness_Bar.value, 0.2f, 1));

            tex.SetPixel(x, 0, color);
        }

        tex.Apply();
        chroma_BG.texture = tex;
    }
    /// <summary>
    /// 明度のグラデ表示
    /// </summary>
    void BrightnessGradient()
    {
        Texture2D tex = new Texture2D(256, 1);

        for (int x = 0; x < 256; x++)
        {
            float v = x / 255f;

            Color color = Color.HSVToRGB
               (hue_Bar.value,
                chroma_Bar.value,
                v);

            tex.SetPixel(x, 0, color);
        }

        tex.Apply();
        brightness_BG.texture = tex;
    }
    /// <summary>
    /// アルファのグラデ表示
    /// </summary>
    void AlphaGradient()
    {
        Texture2D tex = new Texture2D(256, 1);

        for (int x = 0; x < 256; x++)
        {
            float a = x / 255f;

            Color color = Color.HSVToRGB
               (hue_Bar.value,
                chroma_Bar.value,
                brightness_Bar.value);

            color.a = a;

            tex.SetPixel(x, 0, color);
        }

        tex.Apply();
        alpha_BG.texture = tex;
    }
    #endregion

    /// <summary>
    /// ハンドル色の更新
    /// </summary>
    void HandleColorSet()
    {
        //H：色彩バーの位置の色
        hue_Handle.color = Color.HSVToRGB
           (hue_Bar.value,
            1f,
            1f);

        //S：彩度バーの位置の色
        chroma_Handle.color = Color.HSVToRGB
           (hue_Bar.value,
            chroma_Bar.value,
            Mathf.Clamp(brightness_Bar.value, 0.2f, 1));

        //V：明度バーの位置の色
        brightness_Handle.color = Color.HSVToRGB
           (hue_Bar.value,
            chroma_Bar.value,
            brightness_Bar.value);

        //A：現在色 + Alpha
        Color alphaColor = brightness_Handle.color;
        alphaColor.a = alpha_Bar.value;
        alpha_Handle.color = alphaColor;
    }

    /// <summary>
    /// カラーフィールドの色の適応
    /// </summary>
    void ColorFieldGradient()
    {
        Texture2D tex = new Texture2D(256, 256);

        for (int y = 0; y < 256; y++)
        {
            for (int x = 0; x < 256; x++)
            {
                float s = x / 255f;
                float v = y / 255f;

                Color color = Color.HSVToRGB(
                        hue_Bar.value, s, v);

                tex.SetPixel(x, y, color);
            }
        }

        tex.Apply();
        cf_ColorFieldBG.texture = tex;
    }
    #endregion
}
