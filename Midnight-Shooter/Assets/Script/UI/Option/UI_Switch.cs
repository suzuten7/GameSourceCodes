using TMPro;
using UnityEngine;

/* 内容
 * ・疑似スイッチ
 */

public class UI_Switch : MonoBehaviour
{
    #region 変数倉庫
    [Header("◆メインオプション")]
    [SerializeField, Tooltip("設定のタイトルテキスト")] TextMeshProUGUI title_Text;
    string title_Str;
    [SerializeField, Tooltip("全体画像サイズ")] RectTransform area;
    [SerializeField, Tooltip("セレクトされている画像")] RectTransform selectImage;
    [SerializeField, Tooltip("文字一覧")] TextMeshProUGUI[] texts;

    [Header("◆サブオプション")]
    [SerializeField, Tooltip("セレクトされている文字")] Color activeTextColor = Color.white;
    [SerializeField, Tooltip("セレクトされていない文字")] Color inactiveTextColor = Color.gray;
    [SerializeField, Tooltip("初期値")]
    public int default_Index = 0;
    [HideInInspector] public int set_Value = -1;     //保存された値(比較用)
    [HideInInspector] public int currentIndex = 0;
    float image_Width;

    [Header("◆その他")]
    [SerializeField] UI_OptionManager op;
    #endregion

    void Start()
    {
        Load();
        image_Width = area.rect.width / texts.Length;

        //サイズ変更
        selectImage.sizeDelta = new Vector2(
            image_Width, selectImage.sizeDelta.y);

        Refresh();
    }

    /// <summary>
    /// 値のロード
    /// </summary>
    public void Load()
    {
        //保存値読み込み
        int loadValue = Library_SaveFiles.LoadFileInt("Option", gameObject.name, default_Index);

        set_Value = loadValue;
        currentIndex = loadValue;
    }

    /// <summary>
    /// 値の初期化
    /// </summary>
    public void Resets()
    {
        currentIndex = default_Index;
        Refresh();
    }

    /// <summary>
    /// ボタンを押されたら位置を変更
    /// </summary>
    public void Next()
    {
        currentIndex++;

        //最後まで行ったら戻る
        if (currentIndex >= texts.Length)
        { currentIndex = 0; }

        Refresh();
    }

    /// <summary>
    /// 色と移動位置の更新
    /// </summary>
    public void Refresh()
    {
        //色の変更
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].color = (i == currentIndex)
                ? activeTextColor : inactiveTextColor;
        }

        //移動
        selectImage.anchoredPosition = new Vector2(
            image_Width * currentIndex, selectImage.anchoredPosition.y);

        //変更されたのか確認
        title_Text.text = UI_OptionManager.ChangeStr(title_Text.text, currentIndex != set_Value);
        op.CheckChanged();
    }
}
