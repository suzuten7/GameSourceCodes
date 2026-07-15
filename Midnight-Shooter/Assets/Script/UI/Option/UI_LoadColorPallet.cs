using TMPro;
using UnityEngine;
using UnityEngine.UI;

/* 内容
 * ・カラーパレットの呼び出し
 */

public class UI_LoadColorPallet : MonoBehaviour
{
    #region 変数倉庫
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("設定のタイトルテキスト")] TextMeshProUGUI title_Text;
    string title_Str;
    [Tooltip("デフォルト色")] public Color default_Color;
    [SerializeField, Tooltip("現在の色を使う画像")] Image now_Color;
    [HideInInspector] public string now_ColorCode;
    [SerializeField] GameObject colorPallet;

    [Header("◆サブオプション")]
    [SerializeField, Tooltip("アルファの最低値"), Range(0, 1)] float min_alpha = 0f;
    [HideInInspector] public string set_ColorCode = "FF00FFFF"; //保存された値(比較用)

    [Header("◆その他")]
    [SerializeField] UI_OptionManager op;
    UI_ColorPalletManager cmp;
    #endregion

    /// <summary>
    /// 値のロード
    /// </summary>
    public void Load()
    {
        string loadColorCode;

        //ロード
        loadColorCode = Library_SaveFiles.LoadFileStr("Option", gameObject.name, ColorUtility.ToHtmlStringRGBA(default_Color));

        set_ColorCode = loadColorCode;
        now_ColorCode = loadColorCode;
        Reset_Color();
    }

    /// <summary>
    /// 値の初期化
    /// </summary>
    public void Resets()
    {
        now_ColorCode = ColorUtility.ToHtmlStringRGBA(default_Color);
        Reset_Color();
    }

    /// <summary>
    /// カラーパレットを起動
    /// </summary>
    public void Set_ColorPallet()
    {
        cmp = colorPallet.GetComponent<UI_ColorPalletManager>();

        //開閉処理
        /* メモ
         * ・前回と同じボタンを押した時
         * ├開いている → 閉じる
         * └閉じている → 開く
         * ・前回とは別のボタンを押した時
         * └そのまま(色のみ変更)
         */
        if (cmp.loadColorPallet == this)
        {
            cmp.gameObject.SetActive(!cmp.gameObject.activeSelf);
            if (!cmp.gameObject.activeSelf) return;
        }
        else { cmp.gameObject.SetActive(true); }

        cmp.default_ColorImage.color = default_Color;
        ColorUtility.TryParseHtmlString("#" + set_ColorCode, out Color color);
        cmp.after_ColorImage.color = color;
        cmp.before_ColorImage.color = now_Color.color;

        cmp.min_AlphaValue = min_alpha;
        cmp.loadColorPallet = this;

        cmp.Appry_ColorPallet();
    }

    /// <summary>
    /// カラーパレットの変更後の色を取得
    /// </summary>
    public void Get_ColorPallet(Color color)
    {
        now_ColorCode = ColorUtility.ToHtmlStringRGBA(color);
        now_Color.color = color;

        //変更されたのか確認
        title_Text.text = UI_OptionManager.ChangeStr(title_Text.text, now_ColorCode != set_ColorCode);
        op.CheckChanged();
    }

    /// <summary>
    /// 色の初期化
    /// </summary>
    public void Reset_Color()
    {
        //色の変更
        ColorUtility.TryParseHtmlString("#" + now_ColorCode, out Color color);
        now_Color.color = color;

        //変更されたのか確認

        title_Text.text = UI_OptionManager.ChangeStr(title_Text.text, now_ColorCode != set_ColorCode);
        op.CheckChanged();

        if (cmp != null && cmp.loadColorPallet == this) { cmp.Reset_ColorPallet(color); }
    }
}
