using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_ScrollBar : MonoBehaviour
{
    #region 変数倉庫
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("設定のタイトルテキスト")] TextMeshProUGUI title_Text;
    string title_Str;
    public Scrollbar scrollbar;
    [SerializeField, Tooltip("値を表示する文字")] TextMeshProUGUI value_Text;
    [SerializeField, Tooltip("値を表示する入力欄")] TMP_InputField value_InputField;

    [Header("◆サブオプション")]
    [SerializeField, Tooltip("値の最低値と最大値のセット\nX : 最低値\nY : 最大値")]
    public Vector2 value_Range = new Vector2(0, 100);
    [SerializeField, Tooltip("初期値")]
    public float default_Value = 50;
    [SerializeField]
    public float scrolle_pow = 1.0f;
    [HideInInspector] public float set_Value = -1;     //保存された値(比較用)
    [SerializeField] string label = " %";

    float min_Value => value_Range.x;
    float max_Value => value_Range.y;
    int max_DigitCount, min_DigitCount;

    [Header("◆その他")]
    [SerializeField] UI_OptionManager op;
    #endregion

    void Start()
    {
        //保存値読み込み
        Load();

        #region min_Valueとmax_Valueの桁数を取得
        max_DigitCount = Mathf.Max(1, Mathf.FloorToInt(max_Value).ToString().Length);
        min_DigitCount = 1;
        //小数点以下の桁数取得(上限：小数点5桁まで)
        if (min_Value < 1f)
        {
            string minStr = min_Value.ToString("0.#####");

            if (minStr.Contains("."))
            { min_DigitCount = minStr.Split('.')[1].Length; }
        }
        #endregion

        //値変更イベント登録
        scrollbar.onValueChanged.AddListener(OnScrollChanged);
        value_InputField.onEndEdit.AddListener(OnInputChanged);

        //初期表示
        OnScrollChanged(scrollbar.value);
    }

    /// <summary>
    /// 値のロード
    /// </summary>
    public void Load()
    {
        float loadValue = Library_SaveFiles.LoadFileFloat("Option", gameObject.name, Mathf.InverseLerp(min_Value, max_Value, default_Value));
        set_Value = loadValue;
        scrollbar.value = loadValue;
    }

    /// <summary>
    /// 値の初期化
    /// </summary>
    public void Resets()
    {
        scrollbar.value = Mathf.InverseLerp(min_Value, max_Value, default_Value);
        OnScrollChanged(scrollbar.value);
    }

    /// <summary>
    /// スクロールバーの値変更時
    /// </summary>
    public void OnScrollChanged(float value)
    {
        float displayValue = Mathf.Lerp(min_Value, max_Value,Mathf.Pow(value,scrolle_pow));

        value_Text.text =
            $"{Library_UI.FormatNum(displayValue, "FFFFFF", max_DigitCount, min_DigitCount)}{label}";

        //変更されたのか確認
        title_Text.text = UI_OptionManager.ChangeStr(title_Text.text, set_Value != scrollbar.value);
        op.CheckChanged();
    }

    /// <summary>
    /// インプットフィールドの値変更時
    /// </summary>
    void OnInputChanged(string text)
    {
        //数値変換
        if (float.TryParse(text, out float value))
        {
            //範囲制限
            value = Mathf.Clamp(value, min_Value, max_Value);

            //値を反映
            scrollbar.value = Mathf.Pow(Mathf.InverseLerp(min_Value, max_Value, value),1f/scrolle_pow);
            OnScrollChanged(scrollbar.value);
        }

        //入力欄を空にする
        value_InputField.text = "";
    }
}
